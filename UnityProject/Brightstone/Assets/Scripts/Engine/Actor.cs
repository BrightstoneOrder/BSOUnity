using System.Collections.Generic;

namespace Brightstone
{
	public class Actor : BaseComponent 
	{
        private ObjectType mType = null;
        private List<SubComponent> mSubComponents = new List<SubComponent>();


        protected override void OnRecycle()
        {
            mType = null;
        }

        public List<SubComponent> GetSubComponents() { return mSubComponents; }
        public ObjectType GetObjectType() { return mType; }
    }
}