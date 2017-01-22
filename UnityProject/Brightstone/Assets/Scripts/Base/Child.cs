using UnityEngine;
using System.Collections;

namespace Brightstone
{
    // Use this to attach a child-gameobject at runtime. Uses `Prefab` so the data is safe.
    public class Child : MonoBehaviour
    {
        [SerializeField]
        private bool mEnabledByDefault = false;
        [SerializeField]
        [PrefabView(typeof(Actor))]
        private Prefab mPrefab = null;

        private void Start()
        {
            if(transform.parent == null)
            {
                Log.Sys.Error("Child attached to a gameobject with no parent! " + gameObject.name + "[" + gameObject.GetInstanceID() + "]");
                Destroy(gameObject);
                return;
            }
            Actor parent = transform.parent.GetComponent<Actor>();
            if(parent == null)
            {
                Log.Sys.Error("Child attached to a non-actor gameobject with no parent! " + gameObject.name + "[" + gameObject.GetInstanceID() + "]");
                Destroy(gameObject);
                return;
            }

            if(mPrefab == null)
            {
                Destroy(gameObject);
                return;
            }


            CreationData creationData = new CreationData();
            creationData.prefab = mPrefab;
            creationData.localPosition = transform.localPosition;
            creationData.localRotation = transform.localRotation;
            creationData.parent = parent;
            creationData.enabledByDefault = mEnabledByDefault;

            World.ActiveWorld.CreateChild(creationData);
            Destroy(gameObject);
        }
    }
}
