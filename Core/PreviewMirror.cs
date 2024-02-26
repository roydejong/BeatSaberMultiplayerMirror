using BeatSaber.BeatAvatarAdapter.AvatarEditor;
using BeatSaber.BeatAvatarSDK;
using MultiplayerMirror.Core.Scripts;
using SiraUtil.Affinity;
using UnityEngine;
using Zenject;

namespace MultiplayerMirror.Core
{
    public class PreviewMirror : IAffinity
    {
        [Inject] private readonly DiContainer _diContainer = null!;
        
        private BeatAvatarEditorFlowCoordinator? _editorFlowCoordinator;
        private BeatAvatarEditorViewController? _editorViewController;
        private GameObject? _animatedAvatar = null;

        [AffinityPatch(typeof(BeatAvatarEditorFlowCoordinator), nameof(BeatAvatarEditorFlowCoordinator.DidActivate))]
        [AffinityPostfix]
        public void PostfixFlowDidActivate(BeatAvatarEditorFlowCoordinator __instance)
        {
            if (!Plugin.Config.EnablePreviewMirror)
                return;
            
            _editorFlowCoordinator = __instance;
        }

        [AffinityPatch(typeof(BeatAvatarEditorViewController), nameof(BeatAvatarEditorViewController.RefreshUi))]
        [AffinityPostfix]
        public void PostfixRefreshUi(BeatAvatarEditorViewController __instance)
        {
            if (!Plugin.Config.EnablePreviewMirror)
                return;

            _editorViewController = __instance;
            
            if (_animatedAvatar == null && _editorFlowCoordinator != null)
                _animatedAvatar = _editorFlowCoordinator._avatarContainerGameObject;

            EnableMirrorPreview();
        }

        private void SetMirrorPreview(bool mirrorMode)
        {
            if (_animatedAvatar is null)
                return;
            
            // Toggle default animation
            var animator = _animatedAvatar.GetComponent<Animator>();
            animator.enabled = !mirrorMode;
            
            // Try get avatar object
            var playerAvatar = _animatedAvatar.transform.Find("PlayerAvatar").gameObject;
            if (playerAvatar == null)
                return;
            
            var poseController = playerAvatar.GetComponent<BeatAvatarPoseController>();
            if (poseController == null)
                return;
            
            var menuUpdater = playerAvatar.GetComponent<MenuPoseScript>();
            if (menuUpdater == null)
            {
                menuUpdater = playerAvatar.AddComponent<MenuPoseScript>();
                _diContainer.Inject(menuUpdater);
            }
            menuUpdater.TargetPoseController = poseController;
            menuUpdater.enabled = mirrorMode;
        }

        public void EnableMirrorPreview() => SetMirrorPreview(true);
    }
}