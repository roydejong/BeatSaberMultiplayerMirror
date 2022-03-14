using Zenject;

namespace MultiplayerMirror.Core.Installers
{
    public class MpMirrorAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<PluginConfig>().FromInstance(Plugin.Config).AsSingle();
        }
    }
}