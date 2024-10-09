using JetBrains.Annotations;
using Zenject;

namespace MultiplayerMirror.Core.Installers
{
    [UsedImplicitly]
    public class MpMirrorMultiPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HologramMirror>().AsSingle();
        }
    }
}