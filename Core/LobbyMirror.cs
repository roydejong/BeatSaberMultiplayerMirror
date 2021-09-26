using System;
using IPA.Utilities;
using MultiplayerMirror.Events;
using MultiplayerMirror.Events.Models;
using UnityEngine;

namespace MultiplayerMirror.Core
{
    public class LobbyMirror
    {
        private static readonly Vector3 MirrorSpawnPos = new Vector3(0.0f, 0.0f, 3.0f);
        private static readonly Quaternion MirrorSpawnRot = new Quaternion(0f, 180f, 0f, 0f);
        
        private IConnectedPlayer? _localPlayer;
        private MockPlayer? _mockPlayer;
        private MultiplayerLobbyAvatarController? _mockPlayerAvatarController;

        #region Setup
        public void SetUp()
        {
            ModEvents.ConfigChanged += OnConfigChanged;
            ModEvents.LobbyAvatarCreated += OnLobbyAvatarCreated;
        }

        public void TearDown()
        {
            ModEvents.ConfigChanged -= OnConfigChanged;
            ModEvents.LobbyAvatarCreated -= OnLobbyAvatarCreated;
            
            CleanUpMockPlayer();
        }
        #endregion

        #region Events
        private void OnConfigChanged(object sender, string propertyName)
        {
            SyncConfigSetting();
        }
        
        private void OnLobbyAvatarCreated(object sender, LobbyAvatarCreatedEventArgs e)
        {
            if (e.Player.isMe)
            {
                // Local player was just added to the lobby 
                _localPlayer = e.Player;
                
                // Local player normally doesn't get an avatar; so spawn a mock player for them
                _mockPlayer = CreateMockPlayer(_localPlayer);
                e.AvatarManager.AddPlayer(_mockPlayer);
            }
            else if (_localPlayer is not null && e.Player == _mockPlayer && e.AvatarController is not null)
            {
                // Mock player / clone avatar was just created by us
                _mockPlayerAvatarController = e.AvatarController;
                ConfigureMockPlayer(_localPlayer, _mockPlayer, e.AvatarController);
            }
        }
        #endregion
        
        #region Avatar Logic
        private void SyncConfigSetting()
        {
            if (_mockPlayerAvatarController is null)
                return;

            var wasActive = _mockPlayerAvatarController.gameObject.activeSelf;
            var makeActive = Plugin.Config is not null && Plugin.Config.EnableLobbyMirror;

            if (!wasActive && makeActive)
            {
                _mockPlayerAvatarController.gameObject.SetActive(true);
                _mockPlayerAvatarController.ShowSpawnAnimation(MirrorSpawnPos, MirrorSpawnRot);
            }
            else if (wasActive && !makeActive)
            {
                _mockPlayerAvatarController.gameObject.SetActive(false);
            }
        }
        
        private MockPlayer CreateMockPlayer(IConnectedPlayer basePlayer)
        {
            CleanUpMockPlayer(); 
            
            var mockPlayerSettings = new MockPlayerSettings()
            {
                userId = basePlayer.userId,
                userName = $"{basePlayer.userName}-Mirror",
                sortIndex = basePlayer.sortIndex,
                latency = 0.0f,
                movementType = MockPlayerMovementType.MirrorPlayer,
                autoConnect = true,
                inactiveByDefault = false
            };
                
            _mockPlayer = new MockPlayer(mockPlayerSettings, false);
            _mockPlayer.SetProperty("multiplayerAvatarData", basePlayer.multiplayerAvatarData);
            
            return _mockPlayer;
        }

        private void ConfigureMockPlayer(IConnectedPlayer basePlayer, MockPlayer mockPlayer,
            MultiplayerLobbyAvatarController avatarController)
        {
            var tfAvatar = avatarController.gameObject.transform;

            // Tweak position and rotation so it faces the local player
            avatarController.ShowSpawnAnimation(MirrorSpawnPos, MirrorSpawnRot);

            // Disable name tag (to avoid timing issues with spawn coroutine, just disable all its children)
            foreach (Transform tfChild in tfAvatar.Find("AvatarCaption"))
                tfChild.gameObject.SetActive(false);

            // Connect player movement
            var multiplayerAvatarPoseController =
                avatarController.gameObject.GetComponent<MultiplayerAvatarPoseController>();
            multiplayerAvatarPoseController.connectedPlayer = basePlayer;

            var internalAvatarPoseController =
                multiplayerAvatarPoseController.GetField<AvatarPoseController, MultiplayerAvatarPoseController>(
                    "_avatarPoseController");

            // Enable actual mirror effect - this script mirrors position and rotation
            var avatarPoseMirror = avatarController.gameObject.AddComponent<AvatarPoseMirror>();
            avatarPoseMirror.SetField("_avatarPoseController", internalAvatarPoseController);
            
            SyncConfigSetting();
        }

        private void CleanUpMockPlayer()
        {
            if (_mockPlayerAvatarController is not null)
            {
                // We already have a mock player, make sure it gets destroyed
                try
                {
                    _mockPlayerAvatarController.DestroySelf();
                }
                catch (Exception)
                {
                    // May blow up if the game object is already dead
                }

                _mockPlayerAvatarController = null;
                _mockPlayer = null;
            }
        }
        #endregion
    }
}