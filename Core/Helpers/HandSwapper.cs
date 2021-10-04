using UnityEngine;

namespace MultiplayerMirror.Core.Helpers
{
    public static class HandSwapper
    {
        public static void ApplySwap(GameObject goPlayerAvatar, bool mirrorSwap)
        {
            var tfLeftHandBase = goPlayerAvatar.transform.Find("LeftHand/Hand");
            tfLeftHandBase.localScale = new Vector3((mirrorSwap ? +1f : -1f), 1f, 1f);
                
            var tfRightHandBase = goPlayerAvatar.transform.Find("RightHand/Hand");
            tfRightHandBase.localScale = new Vector3((mirrorSwap ? -1f : +1f), 1f, 1f);
        }
    }
}