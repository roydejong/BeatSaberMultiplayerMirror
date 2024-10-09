using System;

namespace MultiplayerMirror
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class PluginConfig
    {
        internal event EventHandler<EventArgs>? ChangeEvent;
        internal void TriggerChangeEvent(object sender) => ChangeEvent?.Invoke(sender, EventArgs.Empty);

        /// <summary>
        /// If enabled, show avatar mirror in lobby.
        /// </summary>
        public virtual bool EnableLobbyMirror { get; set; } = true;

        /// <summary>
        /// If enabled, allow self-hologram in multiplayer matches, if you are in 1st places.
        /// </summary>
        public virtual bool EnableSelfHologram { get; set; } = true;

        /// <summary>
        /// If enabled, force self-hologram in multiplayer matches, never showing other player's avatars.
        /// </summary>
        public virtual bool ForceSelfHologram { get; set; } = false;

        /// <summary>
        /// If enabled, allow self-hologram in duel (1v1) multiplayer layout.
        /// </summary>
        public virtual bool EnableDuelHologram { get; set; } = true;

        /// <summary>
        /// If true, do not apply mirror script, so your left and right are flipped from the mirror.
        /// </summary>
        public virtual bool InvertMirror { get; set; } = false;

        /// <summary>
        /// Enables mirror mode when changing your avatar in the menu.
        /// </summary>
        public virtual bool EnablePreviewMirror { get; set; } = true;
    }
}