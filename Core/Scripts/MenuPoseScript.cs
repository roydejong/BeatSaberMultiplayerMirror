using System;
using UnityEngine;
using Zenject;

namespace MultiplayerMirror.Core.Scripts
{
    public class MenuPoseScript : MonoBehaviour
    {
        [Inject] private readonly MenuPlayerController _menuPlayerController = null!;

        public AvatarPoseController? TargetPoseController { get; set; } = null;

        public void LateUpdate()
        {
            if (TargetPoseController == null || _menuPlayerController == null)
                return;

            TargetPoseController.UpdateTransforms(
                _menuPlayerController.headPos,
                _menuPlayerController.leftController.position,
                _menuPlayerController.rightController.position,
                _menuPlayerController.headRot,
                _menuPlayerController.leftController.rotation,
                _menuPlayerController.rightController.rotation
            );
        }
    }
}