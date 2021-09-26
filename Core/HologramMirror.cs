using System.Linq;
using IPA.Utilities;
using MultiplayerMirror.Events;
using MultiplayerMirror.Events.Models;

namespace MultiplayerMirror.Core
{
    public class HologramMirror
    {
        private IConnectedPlayer? _localPlayer;
        private MultiplayerScoreProvider.RankedPlayer? _rankedLocalPlayer;

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
            
            foreach (var player in e.ActivePlayers)
            {
                if (player.isConnectionOwner)
                    continue;

                if (player.isMe) // TODO remove
                    continue;  
                
                _localPlayer = player;
            } 
        }

        private void OnFirstPlayerDidChange(object sender, FirstPlayerDidChangeEventArgs e)
        {
            // A player has moved to 1st place, and the hologram is about to change
            
            if (!(Plugin.Config?.EnableSelfHologram ?? false))
                // Self-hologram option is not enabled, we don't need to do anything
                return;

            if (_localPlayer == null || !_localPlayer.IsActiveOrFinished())
                // Players haven't spawned yet, or the local player is not active
                return;

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

            if (_rankedLocalPlayer == null)
                // Failed to get ranked local player - this should not happen
                return;
            
            if (Plugin.Config.ForceSelfHologram && e.FirstPlayer != _rankedLocalPlayer)
            {
                // Force mode: set local player to always be in 1st place, even when they're not
                Plugin.Log?.Warn($"Force HandleFirstPlayerDidChange - {_rankedLocalPlayer.userName}");
                e.LeadPlayerProvider.HandleFirstPlayerDidChange(_rankedLocalPlayer);
            }
        }
        #endregion
    }
}