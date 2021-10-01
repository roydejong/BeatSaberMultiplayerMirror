using System.Linq;
using IPA.Utilities;
using MultiplayerMirror.Core.Scripts;
using MultiplayerMirror.Events;
using MultiplayerMirror.Events.Models;
using Tweening;
using UnityEngine;

namespace MultiplayerMirror.Core
{
    public class HologramMirror
    {
        private IConnectedPlayer? _localPlayer;
        private MultiplayerScoreProvider.RankedPlayer? _rankedLocalPlayer;
        private MultiplayerConnectedPlayerFacade? _mirrorFacade;
        private Transform? _tfSelfBigAvatar;
        private MultiplayerBigAvatarAnimator? _bigAvatarAnimator;

        #region Setup
        public void SetUp()
        {
            ModEvents.PlayersSpawned += OnPlayersSpawned;
            ModEvents.FirstPlayerDidChange += OnFirstPlayerDidChange;
        }

        public void TearDown()
        {
            ModEvents.PlayersSpawned -= OnPlayersSpawned;
            ModEvents.FirstPlayerDidChange -= OnFirstPlayerDidChange;
        }
        #endregion

        #region Events
        private void OnPlayersSpawned(object sender, PlayersSpawnedEventArgs e)
        {
            // All players just spawned in gameplay, grab a reference to the local player
            _localPlayer = null;
            _rankedLocalPlayer = null;
            _mirrorFacade = null;
            _tfSelfBigAvatar = null;
            _bigAvatarAnimator = null;

            if (!(Plugin.Config?.EnableSelfHologram ?? false))
                // Self-hologram option is not enabled, don't do anything
                return;
            
            if (e.LocalStartState == MultiplayerPlayerStartState.Late)
                // We connected late, don't do anything
                return;
            
            foreach (var player in e.ActivePlayers)
                if (player.isMe)
                    _localPlayer = player;
            
            if (_localPlayer is null)
                // Local player is not active
                return;
            
            // Create a facade for ourselves - which is normally used to represent remote players
            // This effectively renders us in our own place with big avatar support
            CreatePlayerMirrorFacade(e.PlayersManager, _localPlayer);
        }

        private void OnFirstPlayerDidChange(object sender, FirstPlayerDidChangeEventArgs e)
        {
            // A player has moved to 1st place, and the hologram is about to change
            
            if (!(Plugin.Config?.EnableSelfHologram ?? false))
                // Self-hologram option is not enabled, we don't need to do anything
                return;

            if (_localPlayer == null)
                // Players haven't spawned yet, or local player was not active at start
                return;

            // Get reference to "ranked" local player if we don't have it yet
            if (_rankedLocalPlayer is null)
            {
                var scoreProvider =
                    e.LeadPlayerProvider.GetField<MultiplayerScoreProvider, MultiplayerLeadPlayerProvider>(
                        "_scoreProvider");

                if (scoreProvider is not null)
                {
                    _rankedLocalPlayer =
                        scoreProvider.rankedPlayers.FirstOrDefault(p => p.userId == _localPlayer.userId);
                }
            }

            var weAreLeading = e.FirstPlayer is not null && e.FirstPlayer.userId == _localPlayer.userId;

            if (Plugin.Config.ForceSelfHologram && _rankedLocalPlayer is not null && e.FirstPlayer != _rankedLocalPlayer)
            {
                // Force mode: set local player to always be in 1st place, even when they're not
                e.LeadPlayerProvider.HandleFirstPlayerDidChange(_rankedLocalPlayer);
                weAreLeading = true;
            }
            
            // Toggle self hologram if we are leading currently (forced or otherwise)
            if (_tfSelfBigAvatar is not null)
                _tfSelfBigAvatar.gameObject.SetActive(true);
            
            if (_bigAvatarAnimator is not null)
                if (weAreLeading)
                    _bigAvatarAnimator.Animate(true, 0.3f, EaseType.OutBack);
                else
                    _bigAvatarAnimator.Animate(false, 0.15f, EaseType.OutQuad);
        }
        #endregion

        #region Mirror
        private void CreatePlayerMirrorFacade(MultiplayerPlayersManager playersManager, IConnectedPlayer localPlayer)
        {
            var connectedPlayerFactory =
                playersManager.GetField<MultiplayerConnectedPlayerFacade.Factory, MultiplayerPlayersManager>(
                    "_connectedPlayerFactory");

            if (connectedPlayerFactory is null)
                return;
            
            // Create a "remote player" facade for the local player 
            _mirrorFacade = connectedPlayerFactory.Create(_localPlayer, MultiplayerPlayerStartState.InSync);
            Plugin.Log?.Debug($"[HologramMirror] Created mirror facade for local player");
            
            // Get reference to the big avatar; disable every other object
            _tfSelfBigAvatar = null;
            
            foreach (Transform t in _mirrorFacade.transform)
            {
                if (t.name == "MultiplayerGameBigAvatar")
                {
                    _tfSelfBigAvatar = t;
                    t.gameObject.SetActive(true);
                    continue;
                }

                t.gameObject.SetActive(false);
            }
            
            _bigAvatarAnimator = null;

            if (_tfSelfBigAvatar is not null)
            {
                _bigAvatarAnimator = _tfSelfBigAvatar.GetComponent<MultiplayerBigAvatarAnimator>();
                
                // Rotate big avatar so it faces the player
                _tfSelfBigAvatar.Rotate(0f, 180f, 0f);
                _tfSelfBigAvatar.position = new Vector3(0f, -1.5f, 50f);

                // Add mirror script to the pose controller
                var multiplayerAvatarPoseController =
                    _tfSelfBigAvatar.GetComponent<MultiplayerAvatarPoseController>();

                var internalAvatarPoseController =
                    multiplayerAvatarPoseController.GetField<AvatarPoseController, MultiplayerAvatarPoseController>(
                        "_avatarPoseController");

                var avatarPoseMirror = _tfSelfBigAvatar.gameObject.AddComponent<CustomAvatarPoseMirror>();
                avatarPoseMirror.Init(internalAvatarPoseController);
                avatarPoseMirror.enabled = !(Plugin.Config?.InvertMirror ?? false);
            }

            if (_bigAvatarAnimator is not null)
            {
                _bigAvatarAnimator.HideInstant();
                
                if (Plugin.Config?.ForceSelfHologram ?? false)
                {
                    _bigAvatarAnimator.Animate(true, 1f, EaseType.OutBack);
                }
            }
        }
        #endregion
    }
}