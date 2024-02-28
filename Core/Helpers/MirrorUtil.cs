using UnityEngine;

namespace MultiplayerMirror.Core.Helpers
{
    public static class MirrorUtil
    {
        public static Quaternion MirrorRotation(Quaternion rotation) =>
            new(rotation.x, -rotation.y, -rotation.z, rotation.w);

        public static Vector3 MirrorPosition(Vector3 position) =>
            new(-position.x, position.y, position.z);
    }
}