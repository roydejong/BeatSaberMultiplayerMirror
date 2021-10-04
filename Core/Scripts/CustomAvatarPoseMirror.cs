using UnityEngine;

namespace MultiplayerMirror.Core.Scripts
{
    public class CustomAvatarPoseMirror : MonoBehaviour
    {
        private AvatarPoseController? _avatarPoseController;

        public void Init(AvatarPoseController avatarPoseController)
        {
            _avatarPoseController = avatarPoseController;
            
            if (enabled)
                OnEnable();
            else
                OnDisable();
        }
        
        public void OnEnable()
        {
            OnDisable();
            
            if (_avatarPoseController is null)
                return;
            
            _avatarPoseController.earlyPositionsWillBeSetCallback += HandleAvatarPoseControllerPositionsWillBeSet;
            _avatarPoseController.earlyRotationsWillBeSetCallback += HandleAvatarPoseControllerRotationsWillBeSet;
        }

        public void OnDisable()
        {
            if (_avatarPoseController is null)
                return;
            
            _avatarPoseController.earlyPositionsWillBeSetCallback -= HandleAvatarPoseControllerPositionsWillBeSet;
            _avatarPoseController.earlyRotationsWillBeSetCallback -= HandleAvatarPoseControllerRotationsWillBeSet;
        }

        private static void HandleAvatarPoseControllerPositionsWillBeSet(
            Vector3 headPosition,
            Vector3 leftHandPosition,
            Vector3 rightHandPosition,
            out Vector3 newHeadPosition,
            out Vector3 newLeftHandPosition,
            out Vector3 newRightHandPosition)
        {
            newHeadPosition = MirrorPosition(headPosition);
            newLeftHandPosition = MirrorPosition(leftHandPosition);
            newRightHandPosition = MirrorPosition(rightHandPosition);
        }

        private static void HandleAvatarPoseControllerRotationsWillBeSet(
            Quaternion headRotation,
            Quaternion leftHandRotation,
            Quaternion rightHandRotation,
            out Quaternion newHeadRotation,
            out Quaternion newLeftHandRotation,
            out Quaternion newRightHandRotation)
        {
            newHeadRotation = MirrorRotation(headRotation);
            newLeftHandRotation = MirrorRotation(leftHandRotation);
            newRightHandRotation = MirrorRotation(rightHandRotation);
        }

        private static Quaternion MirrorRotation(Quaternion rotation)
        {
            return new Quaternion(rotation.x, -rotation.y, -rotation.z, rotation.w);
        }

        private static Vector3 MirrorPosition(Vector3 position)
        {
            return new Vector3(-position.x, position.y, position.z);
        }
    }
}