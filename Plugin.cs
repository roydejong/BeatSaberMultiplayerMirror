using System.Reflection;
using BeatSaberMarkupLanguage.GameplaySetup;
using IPA;
using IPA.Config.Stores;
using MultiplayerMirror.Core;
using MultiplayerMirror.UI;
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

        public Plugin() : base()
        {
            Config = new PluginConfig();
        }
        
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
            
            // Add gameplay setup tab
            GameplaySetup.instance.AddTab(
                name: "Multiplayer Mirror", 
                resource: "MultiplayerMirror.UI.BSML.GameplaySetupPanel.bsml",
                host: GameplaySetupPanel.instance,
                menuType: MenuType.Online
            );
        }
        
        [OnDisable]
        public void OnDisable()
        {
            // Remove gameplay setup tab
            GameplaySetup.instance.RemoveTab("Multiplayer Mirror");
            
            // Shut down core components
            _hologramMirror?.TearDown();
            _lobbyMirror?.TearDown();
            
            // Remove Harmony patches
            _harmony?.UnpatchSelf();
        }
    }
}
