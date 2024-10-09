using JetBrains.Annotations;
using Zenject;

namespace MultiplayerMirror.Core.Installers
{
    [UsedImplicitly]
    public class MpMirrorAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<PluginConfig>().FromInstance(Plugin.Config).AsSingle();
        }
    }
}