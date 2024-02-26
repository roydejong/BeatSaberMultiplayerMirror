using BeatSaber.BeatAvatarSDK;
using UnityEngine;

namespace MultiplayerMirror.Core.Scripts
{
    public class PoseMirrorScript : MonoBehaviour
    {
        private BeatAvatarPoseController? _avatarPoseController;

        public void Init(BeatAvatarPoseController avatarPoseController)
        {
            _avatarPoseController = avatarPoseController;
            
            // TODO Figure out a patch to actually make this work, this event hook is no longer in the base game
        }

        public void MirrorPose(
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

        private void MirrorRotation(
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