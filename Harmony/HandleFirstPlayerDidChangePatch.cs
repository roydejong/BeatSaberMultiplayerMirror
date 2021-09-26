using HarmonyLib;
using MultiplayerMirror.Events;
using MultiplayerMirror.Events.Models;

namespace MultiplayerMirror.Harmony
{
    [HarmonyPatch(typeof(MultiplayerLeadPlayerProvider),
        nameof(MultiplayerLeadPlayerProvider.HandleFirstPlayerDidChange), MethodType.Normal)]
    internal class HandleFirstPlayerDidChangePatch
    {
        internal static void Postfix(MultiplayerScoreProvider.RankedPlayer firstPlayer,
            MultiplayerLeadPlayerProvider __instance)
        {
            var args = new FirstPlayerDidChangeEventArgs(__instance, firstPlayer);
            ModEvents.RaiseFirstPlayerDidChange(__instance, args);
        }
    }
}