using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMirror.Events.Models
{
    public class PlayersSpawnedEventArgs
    {
        public readonly MultiplayerPlayersManager PlayersManager;
        public readonly MultiplayerPlayerStartState LocalStartState;
        public readonly IEnumerable<IConnectedPlayer> ActivePlayers;
        
        public bool IsDuel => ActivePlayers.Count() == 2;

        public PlayersSpawnedEventArgs(MultiplayerPlayersManager playersManager,
            MultiplayerPlayerStartState localStartState, IEnumerable<IConnectedPlayer> activePlayers)
        {
            PlayersManager = playersManager;
            LocalStartState = localStartState;
            ActivePlayers = activePlayers;
        }

        public override string ToString()
        {
            return $"PlayersSpawnedEventArgs (localStartState={LocalStartState}, activePlayers={ActivePlayers.Count()})";
        }
    }
}