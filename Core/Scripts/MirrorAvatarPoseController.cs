using BeatSaber.AvatarCore;
using MultiplayerMirror.Core.Helpers;
using UnityEngine;

namespace MultiplayerMirror.Core.Scripts
{
    public class MirrorAvatarPoseController : MonoBehaviour
    {
        public bool EnableMirror { get; set; }
        public bool RestrictPose { get; set; }

        public IConnectedPlayer? LocalPlayer { get; set; }
        public INodePoseSyncStateManager? NodePoseSyncStateManager { get; set; }
        public IAvatarPoseRestriction? AvatarPoseRestriction { get; set; }
        public Transform? LeftSaberTransform { get; set; }
        public Transform? RightSaberTransform { get; set; }
        public Transform? HeadTransform { get; set; }

        public void Init(IConnectedPlayer localPlayer,
            INodePoseSyncStateManager nodePoseSyncStateManager,
            IAvatarPoseRestriction avatarPoseRestriction, Transform leftSaberTransform, Transform rightSaberTransform,
            Transform headTransform)
        {
            EnableMirror = true;
            RestrictPose = true;

            LocalPlayer = localPlayer;
            NodePoseSyncStateManager = nodePoseSyncStateManager;
            AvatarPoseRestriction = avatarPoseRestriction;
            LeftSaberTransform = leftSaberTransform;
            RightSaberTransform = rightSaberTransform;
            HeadTransform = headTransform;

            LocalPlayer = localPlayer;
        }

        public void Init(IConnectedPlayer localPlayer, MultiplayerAvatarPoseController baseController)
        {
            Init(localPlayer, baseController._nodePoseSyncStateManager, baseController._avatarPoseRestriction,
                baseController._leftSaberTransform, baseController._rightSaberTransform, baseController._headTransform);
        }

        public void Update()
        {
            if (NodePoseSyncStateManager == null || LocalPlayer == null)
                return;
            
            var syncStateForPlayer = NodePoseSyncStateManager.GetSyncState(0); // 0 returns local state fast
            if (syncStateForPlayer == null || syncStateForPlayer.player != LocalPlayer)
                // local state not (yet) available
                return;

            var offsetTime = LocalPlayer.offsetSyncTime;
            var headPose = syncStateForPlayer.GetState(NodePoseSyncState.NodePose.Head, offsetTime);
            var leftPose = syncStateForPlayer.GetState(NodePoseSyncState.NodePose.LeftController, offsetTime);
            var rightPose = syncStateForPlayer.GetState(NodePoseSyncState.NodePose.RightController, offsetTime);

            Vector3 headPos = headPose.position;
            Vector3 leftPos = leftPose.position;
            Vector3 rightPos = rightPose.position;
            Quaternion headRot = headPose.rotation;
            Quaternion leftRot = leftPose.rotation;
            Quaternion rightRot = rightPose.rotation;

            if (EnableMirror)
            {
                headPos = MirrorUtil.MirrorPosition(headPos);
                leftPos = MirrorUtil.MirrorPosition(leftPos);
                rightPos = MirrorUtil.MirrorPosition(rightPos);
                headRot = MirrorUtil.MirrorRotation(headRot);
                leftRot = MirrorUtil.MirrorRotation(leftRot);
                rightRot = MirrorUtil.MirrorRotation(rightRot);
            }

            if (RestrictPose)
            {
                AvatarPoseRestriction?.RestrictPose(headRot, headPos, leftPos, rightPos,
                    out headPos, out leftPos, out rightPos);
            }
            
            if (LeftSaberTransform != null)
                LeftSaberTransform.SetLocalPositionAndRotation(leftPos, leftRot);
            if (RightSaberTransform != null)
                RightSaberTransform.SetLocalPositionAndRotation(rightPos, rightRot);
            if (HeadTransform != null)
                HeadTransform.SetLocalPositionAndRotation(headPos, headRot);
        }
    }
}