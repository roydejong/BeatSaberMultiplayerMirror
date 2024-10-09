using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using JetBrains.Annotations;
using MultiplayerMirror.Core.Installers;
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

        private Harmony? _harmony;

        public Plugin() : base()
        {
            Config = new PluginConfig();
        }
        
        [Init]
        [UsedImplicitly]
        public void Init(IPALogger logger, Zenjector zenjector, Config config)
        {
            Log = logger;
            _harmony = new Harmony(HarmonyId);
            Config = config.Generated<PluginConfig>();

            zenjector.UseMetadataBinder<Plugin>();
            zenjector.UseLogger(logger);
            zenjector.UseSiraSync(SiraSyncServiceType.GitHub, "roydejong", "BeatSaberMultiplayerMirror");
            
            zenjector.Install<MpMirrorAppInstaller>(Location.App);
            zenjector.Install<MpMirrorMenuInstaller>(Location.Menu);
            zenjector.Install<MpMirrorMultiPlayerInstaller>(Location.MultiplayerCore);
        }
        
        [OnEnable]
        [UsedImplicitly]
        public void OnEnable()
        {
            // Install Harmony patches
            _harmony!.PatchAll();
        }
        
        [OnDisable]
        [UsedImplicitly]
        public void OnDisable()
        {
            // Remove Harmony patches
            _harmony?.UnpatchSelf();
        }
    }
}
