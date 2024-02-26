using System;
using BeatSaber.AvatarCore;
using UnityEngine;

namespace MultiplayerMirror.Core.Scripts
{
    public class MirrorAvatarPoseDataProvider : IAvatarPoseDataProvider
    {
        public bool EnableMirror { get; set; }
        public bool RestrictPose { get; set; }

        public IConnectedPlayer LocalPlayer { get; set; }
        public INodePoseSyncStateManager NodePoseSyncStateManager { get; set; }
        public IAvatarPoseRestriction AvatarPoseRestriction { get; set; }

        public AvatarPoseData currentPose { get; set; }
        
        public event Action<AvatarPoseData> poseDidChangeEvent = null!;
        
        public MirrorAvatarPoseDataProvider(IConnectedPlayer localPlayer, INodePoseSyncStateManager nodePoseSyncStateManager, 
            IAvatarPoseRestriction avatarPoseRestriction)
        {
            EnableMirror = true;
            RestrictPose = true;
            
            LocalPlayer = localPlayer;
            NodePoseSyncStateManager = nodePoseSyncStateManager;
            AvatarPoseRestriction = avatarPoseRestriction;
            
            LocalPlayer = localPlayer;
        }

        public MirrorAvatarPoseDataProvider(IConnectedPlayer localPlayer, ConnectedPlayerAvatarPoseDataProvider baseProvider)
            : this(localPlayer, baseProvider._nodePoseSyncStateManager, baseProvider._avatarPoseRestriction)
        {
        }

        public void Tick()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (poseDidChangeEvent == null)
                // No one is listening :(
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
                headPos = MirrorPosition(headPos);
                leftPos = MirrorPosition(leftPos);
                rightPos = MirrorPosition(rightPos);
                headRot = MirrorRotation(headRot);
                leftRot = MirrorRotation(leftRot);
                rightRot = MirrorRotation(rightRot);
            }

            if (RestrictPose)
            {
                AvatarPoseRestriction.RestrictPose(headRot, headPos, leftPos, rightPos,
                    out headPos, out leftPos, out rightPos);
            }

            currentPose = new AvatarPoseData
            (
                new Pose(headPos, headRot),
                new Pose(leftPos, leftRot),
                new Pose(rightPos, rightRot)
            );
            poseDidChangeEvent(currentPose);
        }

        private static Quaternion MirrorRotation(Quaternion rotation) =>
            new(rotation.x, -rotation.y, -rotation.z, rotation.w);

        private static Vector3 MirrorPosition(Vector3 position) =>
            new(-position.x, position.y, position.z);
    }
}