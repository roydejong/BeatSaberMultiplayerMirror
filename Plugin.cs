using System.Reflection;
using IPA;
using IPA.Config.Stores;
using MultiplayerMirror.Core;
using IPALogger = IPA.Logging.Logger;

namespace MultiplayerMirror
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public const string HarmonyId = "mod.multiplayermirror";

        internal static IPALogger? Log { get; private set; }
        internal static PluginConfig? Config { get; private set; }

        private HarmonyLib.Harmony? _harmony;
        
        private LobbyMirror? _lobbyMirror;
        private HologramMirror? _hologramMirror;
        
        [Init]
        public void Init(IPALogger logger, IPA.Config.Config config)
        {
            Log = logger;
            Config = config.Generated<PluginConfig>();

            _harmony = new HarmonyLib.Harmony(HarmonyId);
            
            _lobbyMirror = new LobbyMirror();
            _hologramMirror = new HologramMirror();
        }

        [OnEnable]
        public void OnEnable()
        {
            // Install Harmony patches
            _harmony?.PatchAll(Assembly.GetExecutingAssembly());

            // Setup core components
            _hologramMirror?.SetUp();
            _lobbyMirror?.SetUp();
        }

        [OnDisable]
        public void OnDisable()
        {
            // Shut down core components
            _hologramMirror?.TearDown();
            _lobbyMirror?.TearDown();
            
            // Remove Harmony patches
            _harmony?.UnpatchSelf();
        }
    }
}
