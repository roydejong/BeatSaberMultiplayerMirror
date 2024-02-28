using System;
using System.Linq;
using BeatSaber.AvatarCore;
using IPA.Utilities;
using MultiplayerMirror.Core.Helpers;
using MultiplayerMirror.Core.Scripts;
using SiraUtil.Affinity;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace MultiplayerMirror.Core
{
    public class HologramMirror : IInitializable, IDisposable, IAffinity, ITickable
    {
        private const string MirroredAnimatorName = "MultiplayerMirrorHologramAnimator";
        private static readonly Vector3 MirrorPositionCircle = new(0f, -1.5f, 50f);
        private static readonly Vector3 MirrorPositionDuel = new(0f, -1.5f, 60f);

        [Inject] private readonly SiraLog _log = null!;
        [Inject] private readonly PluginConfig _config = null!;
        [Inject] private readonly DiContainer _container = null!;
        [Inject] private readonly MultiplayerPlayersManager _playersManager = null!;
        [Inject] private readonly MultiplayerLeadPlayerProvider _leadPlayerProvider = null!;

        private MultiplayerConnectedPlayerFacade _connectedPlayerPrefab = null!;
        private IConnectedPlayer? _selfPlayer = null;
        private GameObject? _mirrorPlayerGO = null;
        private GameObject? _mirrorBigAvatarGO = null;
        private AvatarController? _newAvatarController;
        private MultiplayerBigAvatarAnimator? _mirrorBigAvatarAnimator = null;
        private MultiplayerPlayerLayout _layout = MultiplayerPlayerLayout.NotDetermined;
        private MirrorAvatarPoseController? _poseController;
        private MirrorAvatarPoseDataProvider? _poseDataProvider;
        private bool _pendingAvatarLoad;

        public void Initialize()
        {
            _connectedPlayerPrefab =
                _playersManager.GetField<MultiplayerConnectedPlayerFacade, MultiplayerPlayersManager>(
                    "_connectedPlayerControllerPrefab");

            _playersManager.playerSpawningDidFinishEvent += HandlePlayersSpawned;
            _leadPlayerProvider.newLeaderWasSelectedEvent += HandleNewPlayerInLead;
        }

        public void Dispose()
        {
            _playersManager.playerSpawningDidFinishEvent -= HandlePlayersSpawned;
            _leadPlayerProvider.newLeaderWasSelectedEvent -= HandleNewPlayerInLead;

            DestroyMirrorPlayerIfActive();
        }

        #region Events

        private void HandlePlayersSpawned()
        {
            if (!_config.EnableSelfHologram || (_layout == MultiplayerPlayerLayout.Duel && !_config.EnableDuelHologram))
                // Self-hologram option is not enabled
                return;

            _selfPlayer = _playersManager.allActiveAtGameStartPlayers.FirstOrDefault(p => p.isMe);

            if (_selfPlayer is null)
                // Local player not found / was not active at game start
                return;

            InstantiateMirrorPlayer();
        }

        private void HandleNewPlayerInLead(string newLeadingUserId)
        {
            // The hologram for the leader is about to be activated
            // We are responsible for our own hologram activation/deactivation

            if (_selfPlayer == null || _mirrorBigAvatarGO == null || _mirrorBigAvatarAnimator == null)
                // Player or mirror avatar not spawned, no-op
                return;

            if (_config.ForceSelfHologram)
                // Force mode enabled, we are always visible, do not animate
                return;

            if (newLeadingUserId == _selfPlayer.userId)
            {
                // We are the new lead, animate big avatar in
                _mirrorBigAvatarAnimator.Animate(true, 0.3f, EaseType.OutBack);
            }
            else
            {
                // We are NOT leading (anymore)
                _mirrorBigAvatarAnimator.Animate(false, 0.15f, EaseType.OutQuad);
            }
        }

        #endregion

        #region Patches

        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerBigAvatarAnimator), "InitIfNeeded")]
        [AffinityAfter("com.github.Goobwabber.MultiplayerExtensions")]
        private void PostfixBigAvatarInit(MultiplayerBigAvatarAnimator __instance)
        {
            // MultiplayerExtensions will set the game object to inactive if Holograms are toggled off

            // If this is our animator, identified by name, re-enable immediately to work around this
            // Rationale: an enabled feature here should take priority over "regular" holograms being disabled in MpEx 

            if (__instance.name == MirroredAnimatorName)
            {
                __instance.gameObject.SetActive(true);
            }
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerBigAvatarAnimator), nameof(MultiplayerBigAvatarAnimator.Animate))]
        private bool PrefixBigAvatarAnimate(MultiplayerBigAvatarAnimator __instance)
        {
            if (!_config.EnableSelfHologram || !_config.ForceSelfHologram)
                // Not enabled or not in forced mode, let code run as normal
                return true;

            if (__instance.gameObject.name == MirroredAnimatorName)
                // This is our own animator, do not interfere (managed in HandleNewPlayerInLead)
                return true;

            // Force hide and disable animations for other player's big avatars while in forced mode
            __instance.HideInstant();
            return false;
        }


        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerPlayersManager), "BindPlayerFactories")]
        private void HandleBindPlayerFactories(MultiplayerPlayerLayout layout)
        {
            _log.Info($"Multiplayer layout was determined (MultiplayerPlayerLayout={layout})");
            _layout = layout;
        }
        #endregion

        #region Mirror facade

        private void InstantiateMirrorPlayer()
        {
            if (_selfPlayer is null)
                return;

            DestroyMirrorPlayerIfActive();

            // Create a "connected player" prefab based on our local player instance
            // This will appear in our place and will effectively render us as a remote player on top of ourselves
            var subContainer = _container.CreateSubContainer();
            subContainer.BindInterfacesAndSelfTo<IConnectedPlayer>().FromInstance(_selfPlayer);
            subContainer.BindInterfacesAndSelfTo<MultiplayerPlayerStartState>()
                .FromInstance(MultiplayerPlayerStartState.InSync);
            _mirrorPlayerGO = subContainer.InstantiatePrefab(_connectedPlayerPrefab);

            // Get reference to the big avatar; disable every other object
            _mirrorBigAvatarGO = null;
            _mirrorBigAvatarAnimator = null;

            foreach (Transform t in _mirrorPlayerGO.transform)
            {
                var go = t.gameObject;

                if (go.name == "MultiplayerGameBigAvatar")
                {
                    _mirrorBigAvatarGO = go;
                    go.SetActive(true);
                    continue;
                }

                go.SetActive(false);
            }

            if (_mirrorBigAvatarGO == null)
                return;

            ConfigureBigAvatar();
        }

        private void ConfigureBigAvatar()
        {
            if (_mirrorBigAvatarGO == null || _selfPlayer == null)
                return;

            // Rotate big avatar so it faces the player
            var baseTransform = _mirrorBigAvatarGO.transform;
            baseTransform.Rotate(0f, 180f, 0f);
            if (_layout == MultiplayerPlayerLayout.Duel)
                baseTransform.position = MirrorPositionDuel;
            else
                baseTransform.position = MirrorPositionCircle;

            // Replace pose controller (mostly for sabers)
            var multiplayerAvatarPoseController = _mirrorBigAvatarGO.GetComponent<MultiplayerAvatarPoseController>();
            multiplayerAvatarPoseController.enabled = false;
            
            _poseController = _mirrorBigAvatarGO.GetComponent<MirrorAvatarPoseController>();
            if (_poseController == null)
                _poseController = _mirrorBigAvatarGO.AddComponent<MirrorAvatarPoseController>();
            _poseController.Init(_selfPlayer, multiplayerAvatarPoseController);
            
            // Replace pose data provider (for beat avatars)
            _newAvatarController = _mirrorBigAvatarGO.GetComponent<AvatarController>();
            if (_newAvatarController._poseDataProvider is ConnectedPlayerAvatarPoseDataProvider poseProvider)
            {
                _poseDataProvider = new MirrorAvatarPoseDataProvider(_selfPlayer, poseProvider);
                _newAvatarController.SetField<AvatarController, IAvatarPoseDataProvider>("_poseDataProvider", _poseDataProvider); // SetField because readonly
            }            
            
            // The underlying avatar is probably not loaded; but just in case, ensure it uses our pose provider
            if (_newAvatarController.avatar != null)
                _newAvatarController.avatar.SetPoseDataProvider(_poseDataProvider);
            else
                _pendingAvatarLoad = true;
            
            ApplyInvertAndSwap();

            // Animate hide or appear
            _mirrorBigAvatarAnimator = _mirrorBigAvatarGO.GetComponent<MultiplayerBigAvatarAnimator>();
            _mirrorBigAvatarAnimator.name = MirroredAnimatorName;
            _mirrorBigAvatarAnimator.HideInstant();

            if (_config.ForceSelfHologram)
                _mirrorBigAvatarAnimator.Animate(true, 1f, EaseType.OutBack);
        }
        
        private void ApplyInvertAndSwap()
        {
            if (_poseDataProvider is not null)
                _poseDataProvider.EnableMirror = !_config.InvertMirror;
            
            if (_newAvatarController != null && _newAvatarController.avatar != null)
                HandSwapper.ApplySwap(_newAvatarController.avatar.gameObject, !_config.InvertMirror);
        }

        private void DestroyMirrorPlayerIfActive()
        {
            if (_mirrorPlayerGO == null)
                return;

            UnityEngine.Object.Destroy(_mirrorPlayerGO);

            _mirrorPlayerGO = null;
            _mirrorBigAvatarGO = null;
            _mirrorBigAvatarAnimator = null;
            _poseController = null;
            _poseDataProvider = null;
            _pendingAvatarLoad = false;
        }

        #endregion

        public void Tick()
        {
            if (_poseDataProvider == null || _newAvatarController == null)
                return;

            if (_pendingAvatarLoad && _newAvatarController.avatar != null)
            {
                _newAvatarController.avatar.SetPoseDataProvider(_poseDataProvider);
                ApplyInvertAndSwap();
                _pendingAvatarLoad = false;
            }
            
            _poseDataProvider.Tick();
        }
    }
}