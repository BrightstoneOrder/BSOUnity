using UnityEngine;
namespace Brightstone
{
    public struct InputMouseData
    {
        public enum Target
        {
            Interface,
            World
        }
        public Target target;
        public Vector3 screenPosition;
        public Vector3 screenToWorldPosition;
        public Vector3 screenToWorldDirection;
        public Vector3 worldPosition;
        public Actor hitActor;
        public GameObject hitGameObject;
        // These were made for 'RPG like' gameplay where the cursor could hit the edge of the screen.
        //public bool boarderLeft;
        //public bool boarderRight;
        //public bool boarderTop;
        //public bool boarderBottom;
    }
}