using Zenject;

namespace MultiplayerMirror.Core.Installers
{
    public class MpMirrorMultiPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HologramMirror>().AsSingle();
        }
    }
}