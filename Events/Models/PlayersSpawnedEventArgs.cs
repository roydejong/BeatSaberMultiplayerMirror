using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMirror.Events.Models
{
    public class PlayersSpawnedEventArgs
    {
        public MultiplayerPlayersManager PlayersManager;
        public MultiplayerPlayerStartState LocalStartState;
        public IEnumerable<IConnectedPlayer> ActivePlayers;

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