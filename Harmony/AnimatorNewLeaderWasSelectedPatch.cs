using HarmonyLib;
using MultiplayerMirror.Events;
using MultiplayerMirror.Events.Models;

namespace MultiplayerMirror.Harmony
{
    [HarmonyPatch(typeof(MultiplayerGameplayAnimator), "HandleNewLeaderWasSelected", MethodType.Normal)]
    internal class AnimatorNewLeaderWasSelectedPatch
    {
        internal static void Postfix(string userId, MultiplayerGameplayAnimator __instance)
        {
            var args = new NewLeaderWasSelectedEventArgs(__instance, userId);
            ModEvents.RaiseNewLeaderWasSelected(__instance, args);
        }
    }
}