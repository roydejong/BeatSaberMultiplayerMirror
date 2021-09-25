using HarmonyLib;
using MultiplayerMirror.Events;
using MultiplayerMirror.Events.Models;

namespace MultiplayerMirror.Harmony
{
    [HarmonyPatch(typeof(MultiplayerLobbyAvatarManager), nameof(MultiplayerLobbyAvatarManager.AddPlayer),
        MethodType.Normal)]
    internal class LobbyAvatarCreatedEventPatch
    {
        internal static void Postfix(IConnectedPlayer connectedPlayer, MultiplayerLobbyAvatarManager __instance)
        {
            var args = new LobbyAvatarCreatedEventArgs(__instance, connectedPlayer);
            ModEvents.RaiseLobbyAvatarCreated(__instance, args);
        }
    }
}