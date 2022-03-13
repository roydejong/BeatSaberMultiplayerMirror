using System.Reflection;
using BeatSaberMarkupLanguage.GameplaySetup;
using IPA;
using IPA.Config.Stores;
using MultiplayerMirror.Core;
using MultiplayerMirror.Core.Installers;
using MultiplayerMirror.UI;
using SiraUtil.Web.SiraSync;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace MultiplayerMirror
{
    [Plugin(RuntimeOptions.DynamicInit)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Plugin
    {
        public const string HarmonyId = "com.hippomade.multiplayermirror";
        
        internal static PluginConfig Config { get; private set; } = null!;

        internal static IPALogger? Log { get; private set; }

        private HarmonyLib.Harmony? _harmony;

        public Plugin() : base()
        {
            Config = new PluginConfig();
        }
        
        [Init]
        public void Init(IPALogger logger, Zenjector zenjector, IPA.Config.Config config)
        {
            Log = logger;
            _harmony = new HarmonyLib.Harmony(HarmonyId);
            Config = config.Generated<PluginConfig>();

            zenjector.UseMetadataBinder<Plugin>();
            zenjector.UseLogger(logger);
            zenjector.UseSiraSync(SiraSyncServiceType.GitHub, "roydejong", "BeatSaberMultiplayerMirror");
            
            zenjector.Install<MpMirrorAppInstaller>(Location.App);
            zenjector.Install<MpMirrorMenuInstaller>(Location.Menu);
            zenjector.Install<MpMirrorMultiPlayerInstaller>(Location.MultiPlayer);
        }
        
        [OnEnable]
        public void OnEnable()
        {
            // Install Harmony patches
            _harmony?.PatchAll(Assembly.GetExecutingAssembly());

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
            
            // Remove Harmony patches
            _harmony?.UnpatchSelf();
        }
    }
}
