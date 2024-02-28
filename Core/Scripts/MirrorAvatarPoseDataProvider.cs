using System;
using BeatSaber.AvatarCore;
using MultiplayerMirror.Core.Helpers;
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
        }

        public MirrorAvatarPoseDataProvider(IConnectedPlayer localPlayer, ConnectedPlayerAvatarPoseDataProvider baseProvider)
            : this(localPlayer, baseProvider._nodePoseSyncStateManager, baseProvider._avatarPoseRestriction)
        {
        }

        public void Tick()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (poseDidChangeEvent == null)
                // No one is listening (yet) :(
                return;

            var localState = NodePoseSyncStateManager.localState;
            if (localState == null)
                return;

            var offsetTime = LocalPlayer.offsetSyncTime;
            var headPose = localState.GetState(NodePoseSyncState.NodePose.Head, offsetTime);
            var leftPose = localState.GetState(NodePoseSyncState.NodePose.LeftController, offsetTime);
            var rightPose = localState.GetState(NodePoseSyncState.NodePose.RightController, offsetTime);

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
    }
}