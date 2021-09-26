namespace MultiplayerMirror
{
    public class PluginConfig
    {
        /// <summary>
        /// If enabled, show avatar mirror in lobby.
        /// </summary>
        public bool EnableLobbyMirror;
        /// <summary>
        /// If enabled, allow self-hologram in multiplayer matches, if you are in 1st places.
        /// </summary>
        public bool EnableSelfHologram;
        /// <summary>
        /// If enabled, force self-hologram in multiplayer matches, never showing other player's avatars.
        /// </summary>
        public bool ForceSelfHologram;
    }
}