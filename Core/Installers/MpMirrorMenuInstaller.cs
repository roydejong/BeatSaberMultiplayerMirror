using Zenject;

namespace MultiplayerMirror.Core.Installers
{
    public class MpMirrorMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LobbyMirror>().AsSingle();
            Container.BindInterfacesAndSelfTo<PreviewMirror>().AsSingle();
        }
    }
}