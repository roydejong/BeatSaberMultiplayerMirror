using System;
using IPA.Utilities;
using MultiplayerMirror.Core.Scripts;
using SiraUtil.Affinity;
using UnityEngine;
using Zenject;

namespace MultiplayerMirror.Core
{
    public class PreviewMirror : IAffinity
    {
        [Inject] private readonly DiContainer _diContainer = null!;
        [Inject] private readonly EditAvatarFlowCoordinator _editAvatarFlowCoordinator = null!;
        
        private GameObject? _animatedAvatar = null;

        [AffinityPatch(typeof(EditAvatarViewController), "RefreshUi")]
        [AffinityPostfix]
        public void PostfixRefreshUi(EditAvatarViewController __instance)
        {
            if (!Plugin.Config.EnablePreviewMirror)
                return;
            
            if (_animatedAvatar == null)
                _animatedAvatar = _editAvatarFlowCoordinator.GetField<GameObject, EditAvatarFlowCoordinator>
                    ("_avatarContainerGameObject");

            EnableMirrorPreview();
        }

        private void SetMirrorPreview(bool mirrorMode)
        {
            if (_animatedAvatar is null)
                return;

            // Get components and references
            var animator = _animatedAvatar.GetComponent<Animator>();
            var animatorPoseController = _animatedAvatar.GetComponent<AnimatedAvatarPoseController>();

            var avatarPoseController =
                animatorPoseController.GetField<AvatarPoseController, AnimatedAvatarPoseController>(
                    "_avatarPoseController");

            var menuPoseScript = _animatedAvatar.GetComponent<MenuPoseScript>();
            var poseMirrorScript = _animatedAvatar.GetComponent<PoseMirrorScript>();

            // Toggle default animation
            animator.enabled = !mirrorMode;
            animatorPoseController.enabled = !mirrorMode;
            
            // Add/toggle custom scripts for mirror mode
            if (mirrorMode)
            {
                if (menuPoseScript == null)
                {
                    menuPoseScript = _animatedAvatar.AddComponent<MenuPoseScript>();
                    menuPoseScript.TargetPoseController = avatarPoseController;
                    _diContainer.Inject(menuPoseScript);
                }

                if (poseMirrorScript == null)
                {
                    poseMirrorScript = _animatedAvatar.AddComponent<PoseMirrorScript>();
                }

                menuPoseScript.enabled = true;
                poseMirrorScript.enabled = true;
            }
            else
            {
                menuPoseScript.enabled = false;
                poseMirrorScript.enabled = false;
            }
        }

        public void EnableMirrorPreview() => SetMirrorPreview(true);
    }
}