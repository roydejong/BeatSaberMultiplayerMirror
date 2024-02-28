using BeatSaber.BeatAvatarSDK;
using MultiplayerMirror.Core.Helpers;
using UnityEngine;
using Zenject;

namespace MultiplayerMirror.Core.Scripts
{
    public class MenuPoseScript : MonoBehaviour
    {
        [Inject] private readonly MenuPlayerController _menuPlayerController = null!;

        public BeatAvatarPoseController? TargetPoseController { get; set; } = null;

        public void LateUpdate()
        {
            if (TargetPoseController == null || _menuPlayerController == null)
                return;

            TargetPoseController.UpdateTransforms(
                MirrorUtil.MirrorPosition(_menuPlayerController.headPos),
                MirrorUtil.MirrorPosition(_menuPlayerController.leftController.position),
                MirrorUtil.MirrorPosition(_menuPlayerController.rightController.position),
                MirrorUtil.MirrorRotation(_menuPlayerController.headRot),
                MirrorUtil.MirrorRotation(_menuPlayerController.leftController.rotation),
                MirrorUtil.MirrorRotation(_menuPlayerController.rightController.rotation)
            );
        }
    }
}