using System;
using System.Collections.Generic;
using IPA.Utilities;
using MultiplayerMirror.Core.Helpers;
using MultiplayerMirror.Core.Scripts;
using SiraUtil.Affinity;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace MultiplayerMirror.Core
{
    public class LobbyMirror : IInitializable, IDisposable, IAffinity
    {
        private static readonly Vector3 MirrorSpawnPos = new(0.0f, 0.0f, 3.0f);
        private static readonly Quaternion MirrorSpawnRot = new(0f, 180f, 0f, 0f);

        [Inject] private SiraLog _log = null!;
        [Inject] private PluginConfig _config = null!;
        [Inject] private MultiplayerLobbyAvatarManager _lobbyAvatarManager = null!;

        private IConnectedPlayer? _selfPlayer;
        private IConnectedPlayer? _mockPlayer;
        private MultiplayerLobbyAvatarController? _mockAvatarController;
        private PoseMirrorScript? _mockMirrorScript;
        private GameObject? _mockPlayerAvatarGO;

        public void Initialize()
        {
            _config.ChangeEvent += HandleConfigChange;
        }

        public void Dispose()
        {
            _config.ChangeEvent -= HandleConfigChange;
            
            DestroyMirrorAvatarIfActive();
        }

        private void HandleConfigChange(object sender, EventArgs e)
        {
            var hasAvatar = _mockPlayerAvatarGO != null;

            if (_config.EnableLobbyMirror)
                if (hasAvatar)
                    RefreshInvertMirror();
                else
                    CreateMirrorAvatarIfPossible();
            else
                DestroyMirrorAvatarIfActive();
        }

        #region Patches

        [AffinityPatch(typeof(MultiplayerLobbyAvatarManager), nameof(MultiplayerLobbyAvatarManager.AddPlayer))]
        [AffinityPostfix]
        private void HandleLobbyAvatarAdded(IConnectedPlayer connectedPlayer)
        {
            if (connectedPlayer.isMe)
            {
                // Local player avatar spawned
                _selfPlayer = connectedPlayer;
                CreateMirrorAvatarIfPossible();
                return;
            }

            if (connectedPlayer == _mockPlayer)
            {
                // Mirror mock player avatar spawned (triggered by CreateMirrorAvatarIfPossible)
                HandleMirrorAvatarCreated();
                return;
            }
        }

        #endregion

        #region Avatar management

        /// <summary>
        /// Creates and spawns a mock player avatar in the lobby, as a copy of the local player.
        /// Will only complete if lobby avatars are enabled in config and player data is available.
        /// </summary>
        private bool CreateMirrorAvatarIfPossible()
        {
            if (!_config.EnableLobbyMirror)
                // Lobby mirror disabled in config
                return false;

            if (_selfPlayer is null)
                // Player data not yet available
                return false;

            DestroyMirrorAvatarIfActive();

            // We'll create a mock player that is visually identical to the current player
            _mockPlayer = CreateMockPlayer(_selfPlayer);

            // Next, we'll ask the lobby avatar manager to spawn its avatar
            // This will run do everything including playing the spawn animation
            _lobbyAvatarManager.AddPlayer(_mockPlayer);
            // Once complete our HandleLobbyAvatarAdded() postfix will run again for the 2nd step
            return true;
        }

        /// <summary>
        /// Performs finishing touches on the spawned mirror avatar.
        /// </summary>
        private void HandleMirrorAvatarCreated()
        {
            if (_selfPlayer is null || _mockPlayer is null)
                return;

            _mockAvatarController = TryGetAvatarController(_mockPlayer.userId);

            if (_mockAvatarController is null)
                return;

            var mockTransform = _mockAvatarController.gameObject.transform;

            // Tweak position and rotation so it faces the local player
            _mockAvatarController.ShowSpawnAnimation(MirrorSpawnPos, MirrorSpawnRot);

            // Disable name tag (to avoid timing issues with spawn coroutine, just disable all its children)
            foreach (Transform tfChild in mockTransform.Find("AvatarCaption"))
                tfChild.gameObject.SetActive(false);

            // Connect mock player movement to our real player
            var poseController = _mockAvatarController.gameObject.GetComponent<MultiplayerAvatarPoseController>();
            poseController.connectedPlayer = _selfPlayer;

            // Enable actual mirror effect - this script mirrors position and rotation
            _mockMirrorScript = _mockAvatarController.gameObject.AddComponent<PoseMirrorScript>();
            var internalAvatarPoseController =
                poseController.GetField<AvatarPoseController, MultiplayerAvatarPoseController>("_avatarPoseController");
            _mockPlayerAvatarGO = internalAvatarPoseController.gameObject;
            _mockMirrorScript.Init(internalAvatarPoseController);
            
            // Apply "InvertMirror"
            RefreshInvertMirror();
        }

        private void RefreshInvertMirror()
        {
            if (_mockMirrorScript is not null)
                _mockMirrorScript.enabled = !_config.InvertMirror;
            
            if (_mockPlayerAvatarGO != null)
                HandSwapper.ApplySwap(_mockPlayerAvatarGO, !_config.InvertMirror);
        }

        private void DestroyMirrorAvatarIfActive()
        {
            if (_mockPlayer is null)
                return;

            _lobbyAvatarManager.RemovePlayer(_mockPlayer);

            _mockPlayer = null;
            _mockAvatarController = null;
            _mockMirrorScript = null;
            _mockPlayerAvatarGO = null;
        }
        #endregion

        #region Utils

        private MultiplayerLobbyAvatarController? TryGetAvatarController(string userId)
        {
            var playerIdToAvatarMap =
                _lobbyAvatarManager
                    .GetField<Dictionary<string, MultiplayerLobbyAvatarController>, MultiplayerLobbyAvatarManager>(
                        "_playerIdToAvatarMap");

            if (playerIdToAvatarMap != null && playerIdToAvatarMap.TryGetValue(userId, out var avatarController))
                return avatarController;

            return null;
        }

        private static IConnectedPlayer CreateMockPlayer(IConnectedPlayer basePlayer)
        {
            var mockPlayer = new MockPlayer(new MockPlayerSettings()
            {
                userId = $"Mirror#{basePlayer.userId}",
                userName = $"Mirror#{basePlayer.userName}",
                sortIndex = basePlayer.sortIndex
            }, false);
            mockPlayer.SetProperty("multiplayerAvatarData", basePlayer.multiplayerAvatarData);
            return mockPlayer;
        }

        #endregion
    }
}