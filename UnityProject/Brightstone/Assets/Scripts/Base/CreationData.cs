using UnityEngine;

namespace Brightstone
{
    public struct CreationData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Actor parent;
        public Prefab prefab;
        public bool enabledByDefault;
    }
}