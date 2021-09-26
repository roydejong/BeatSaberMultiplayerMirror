using System.Collections.Generic;
using HarmonyLib;
using MultiplayerMirror.Events;
using MultiplayerMirror.Events.Models;

namespace MultiplayerMirror.Harmony
{
    [HarmonyPatch(typeof(MultiplayerPlayersManager),
        nameof(MultiplayerPlayersManager.SpawnPlayers), MethodType.Normal)]
    internal class MultiplayerSpawnPlayersEventPatch
    {
        internal static void Postfix(MultiplayerPlayerStartState localPlayerStartState,
            IReadOnlyList<IConnectedPlayer> allActiveAtGameStartPlayers, MultiplayerPlayersManager __instance)
        {
            var args = new PlayersSpawnedEventArgs(__instance,localPlayerStartState, allActiveAtGameStartPlayers);
            ModEvents.RaisePlayersSpawned(__instance, args);
        }
    }
}