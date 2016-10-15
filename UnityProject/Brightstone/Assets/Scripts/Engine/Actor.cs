using System.Collections.Generic;
using UnityEngine;
using System;

namespace Brightstone
{
	public class Actor : BaseComponent 
	{
        private ObjectType mType = null;
        private List<SubComponent> mSubComponents = new List<SubComponent>();

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

        public virtual void OnUpdate(float delta)
        {
            for(int i = 0; i < mSubComponents.Count; ++i)
            {
                mSubComponents[i].OnUpdate(delta);
            }
        }

        protected override void OnRecycle()
        {
            mType = null;
        }

        public bool IsType(ObjectType type)
        {
            return type == GetObjectType();
        }

        public List<SubComponent> GetSubComponents() { return mSubComponents; }
        public ObjectType GetObjectType() { return mType; }
    }
}