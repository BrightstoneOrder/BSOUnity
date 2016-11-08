using UnityEngine;

namespace Brightstone
{
    public class Unit : Actor
    {
        [PrefabView]
        [SerializeField]
        private Prefab mItemType = new Prefab();

        public Vector3 GetPosition()
        {
            return GetTransform().position;
        }
        public Quaternion GetRotation()
        {
            return GetTransform().rotation;
        }
        public void SetPosition(Vector3 position)
        {
            GetTransform().position = position;
        }
        public void SetRotation(Quaternion rotation)
        {
            GetTransform().rotation = rotation;
        }
    }

}

