namespace MultiplayerMirror
{
    public class PluginConfig
    {
        /// <summary>
        /// If enabled, show avatar mirror in lobby.
        /// </summary>
        public bool EnableLobbyMirror = true;
        /// <summary>
        /// If enabled, allow self-hologram in multiplayer matches, if you are in 1st places.
        /// </summary>
        public bool EnableSelfHologram = true;
        /// <summary>
        /// If enabled, force self-hologram in multiplayer matches, never showing other player's avatars.
        /// </summary>
        public bool ForceSelfHologram = true;
    }
}