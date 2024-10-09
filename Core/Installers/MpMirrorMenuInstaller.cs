using JetBrains.Annotations;
using MultiplayerMirror.UI;
using Zenject;

namespace MultiplayerMirror.Core.Installers
{
    [UsedImplicitly]
    public class MpMirrorMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LobbyMirror>().AsSingle();
            Container.BindInterfacesAndSelfTo<PreviewMirror>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplaySetupPanel>().AsSingle();
        }
    }
}