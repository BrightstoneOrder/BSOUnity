using System.Collections.Generic;

namespace Brightstone
{
	public class Actor : BaseComponent 
	{
        private ObjectType mType = null;
        private List<SubComponent> mSubComponents = new List<SubComponent>();


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

        public List<SubComponent> GetSubComponents() { return mSubComponents; }
        public ObjectType GetObjectType() { return mType; }
    }
}