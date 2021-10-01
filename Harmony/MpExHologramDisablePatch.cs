using HarmonyLib;

namespace MultiplayerMirror.Harmony
{
    [HarmonyPatch(typeof(MultiplayerBigAvatarAnimator), nameof(MultiplayerBigAvatarAnimator.InitIfNeeded),
        MethodType.Normal)]
    [HarmonyAfter("com.github.Goobwabber.MultiplayerExtensions")]
    internal class MpExHologramDisablePatch
    {
        internal static void Postfix(MultiplayerBigAvatarAnimator __instance)
        {
            // MultiplayerExtensions will set the game object to inactive if Holograms are toggled off
            
            // If this is our animator, identified by name, re-enable immediately to work around this
            // Rationale: an enabled feature here should take priority over "regular" holograms being disabled in MpEx 
            
            if (__instance.name == "MultiplayerMirrorHologramAnimator")
            {
                __instance.gameObject.SetActive(true);
            }
        }
    }
}