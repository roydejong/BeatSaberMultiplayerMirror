using System.Reflection;
using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace MultiplayerMirror
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public const string HarmonyId = "mod.multiplayermirror";

        internal static Plugin? Instance { get; private set; }
        internal static IPALogger? Log { get; private set; }
        internal static PluginConfig? Config { get; private set; }

        internal static HarmonyLib.Harmony? Harmony { get; private set; }

        [Init]
        public void Init(IPALogger logger, IPA.Config.Config config)
        {
            Instance = this;
            Log = logger;
            Config = config.Generated<PluginConfig>();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Harmony = new HarmonyLib.Harmony(HarmonyId);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnExit]
        public void OnApplicationQuit()
        {
   
        }
    }
}
