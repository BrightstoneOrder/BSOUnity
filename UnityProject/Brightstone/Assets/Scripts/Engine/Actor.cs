using System.Collections.Generic;
using UnityEngine;
using System;

namespace Brightstone
{
	public class Actor : BaseComponent 
	{
        [PrefabView]
        [SerializeField]
        private Prefab mType = null;
        private List<SubComponent> mSubComponents = new List<SubComponent>();
        private ObjectType mObjectType = null;
        /**
        * Get the root Actor on the given 'transform'
        */
        public static Actor GetRootActor(Transform transform)
        {
            Actor root = null;
            Type type = typeof(Actor);
            while (transform)
            {
                Actor actor = transform.GetComponent(type) as Actor;
                if (actor != null)
                {
                    root = actor;
                }
                transform = transform.parent;
            }
            return root;
        }

        protected override void OnInit()
        {
            if(mWorld)
            {
                mWorld.GetTypeMgr().RegisterInstance(this);
            }
        }

        public virtual void OnUpdate(float delta)
        {
            for(int i = 0; i < mSubComponents.Count; ++i)
            {
                mSubComponents[i].OnUpdate(delta);
            }
        }

        protected override void OnRecycle()
        {
            mObjectType = null;
        }

        protected override void OnDestroyed()
        {
            if(mWorld != null)
            {
                mWorld.GetTypeMgr().UnregisterInstance(this);
            }
        }

        public bool IsType(ObjectType type)
        {
            return type == GetObjectType();
        }

        public List<SubComponent> GetSubComponents() { return mSubComponents; }
        public ObjectType GetObjectType() { return mObjectType; }
        public Prefab GetObjectPrefabType() { return mType; }

        /** Called internally by editor and game*/
        public void InternalInitType(ObjectType type) { mObjectType = type; }

        public void SetPosition(Vector3 position)
        {
            GetTransform().position = position;
        }

        public Vector3 GetPosition()
        {
            return GetTransform().position;
        }

        public void SetRotation(Quaternion rotation)
        {
            GetTransform().rotation = rotation;
        }

        public Quaternion GetRotation()
        {
            return GetTransform().rotation;
        }
    }
}