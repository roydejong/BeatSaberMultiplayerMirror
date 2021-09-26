using System;
using System.Runtime.CompilerServices;
using MultiplayerMirror.Events.Models;

namespace MultiplayerMirror.Events
{
    internal static class ModEvents
    {
        #region ConfigChanged
        /// <summary>
        /// This event is raised when a property in the mod's config was changed.
        /// </summary>
        internal static event EventHandler<string>? ConfigChanged;

        internal static void RaiseConfigChanged(object sender, [CallerMemberName] string propertyName = "") =>
            ConfigChanged.RaiseEventSafe(sender, propertyName);
        #endregion
        
        #region LobbyAvatarCreated
        /// <summary>
        /// This event is raised when an avatar has been created in the lobby.
        /// </summary>
        internal static event EventHandler<LobbyAvatarCreatedEventArgs>? LobbyAvatarCreated;

        internal static void RaiseLobbyAvatarCreated(object sender, LobbyAvatarCreatedEventArgs e) =>
            LobbyAvatarCreated.RaiseEventSafe(sender, e);
        #endregion
        
        #region PlayersSpawned
        /// <summary>
        /// This event is raised when the players were spawned in gameplay.
        /// </summary>
        internal static event EventHandler<PlayersSpawnedEventArgs>? PlayersSpawned;

        internal static void RaisePlayersSpawned(object sender, PlayersSpawnedEventArgs e) =>
            PlayersSpawned.RaiseEventSafe(sender, e);
        #endregion
        
        #region FirstPlayerDidChange
        /// <summary>
        /// This event is raised when the player in 1st position changes during a multiplayer match.
        /// </summary>
        internal static event EventHandler<FirstPlayerDidChangeEventArgs>? FirstPlayerDidChange;

        internal static void RaiseFirstPlayerDidChange(object sender, FirstPlayerDidChangeEventArgs e) =>
            FirstPlayerDidChange.RaiseEventSafe(sender, e);
        #endregion
        
        #region Helper code
        private static void RaiseEventSafe<TArgs>(this EventHandler<TArgs>? e, object sender, TArgs args)
        {
            if (e is null)
                return;
            
            Plugin.Log?.Warn($"[ModEvents] {e.Method.Name} ({args})");
            
            foreach (var invocation in e.GetInvocationList())
            {
                try
                {
                    ((EventHandler<TArgs>) invocation).Invoke(sender, args);
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Error($"[ModEvents] Error in event handler ({e.Method.Name}): {ex}");
                }
            }
        }
        #endregion
    }
}