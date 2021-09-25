﻿using System;
using MultiplayerMirror.Events.Models;

namespace MultiplayerMirror.Events
{
    internal static class ModEvents
    {
        #region LobbyAvatarCreated
        /// <summary>
        /// This event is raised when an avatar has been created in the lobby.
        /// </summary>
        internal static event EventHandler<LobbyAvatarCreatedEventArgs>? LobbyAvatarCreated;

        internal static void RaiseLobbyAvatarCreated(object sender, LobbyAvatarCreatedEventArgs e) =>
            LobbyAvatarCreated.RaiseEventSafe(sender, e);
        #endregion
        
        #region Helper code
        private static void RaiseEventSafe<TArgs>(this EventHandler<TArgs>? e, object sender, TArgs args)
        {
            if (e is null)
                return;
            
            Plugin.Log?.Info($"[ModEvents] {e.Method.Name} ({args})");
            
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