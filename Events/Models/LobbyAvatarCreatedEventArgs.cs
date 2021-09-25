using System.Collections.Generic;
using IPA.Utilities;

namespace MultiplayerMirror.Events.Models
{
    public class LobbyAvatarCreatedEventArgs
    {
        public readonly MultiplayerLobbyAvatarManager AvatarManager;
        public readonly IConnectedPlayer Player;
        public readonly MultiplayerLobbyAvatarController? AvatarController;

        public LobbyAvatarCreatedEventArgs(MultiplayerLobbyAvatarManager avatarManager, IConnectedPlayer player)
        {
            AvatarManager = avatarManager;
            Player = player;
            
            var avatarMap = avatarManager.GetField<Dictionary<string, MultiplayerLobbyAvatarController>,
                MultiplayerLobbyAvatarManager>("_playerIdToAvatarMap");

            if (avatarMap is not null && avatarMap.ContainsKey(player.userId))
                AvatarController = avatarMap[player.userId];
        }

        public override string ToString()
        {
            return $"LobbyAvatarCreatedEventArgs (userId={Player.userId}, userName={Player.userName}, " +
                   $"hasAvatarController={(AvatarController is not null)})";
        }
    }
}