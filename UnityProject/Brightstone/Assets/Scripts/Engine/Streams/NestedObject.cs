using System;

namespace Brightstone
{
    public class NestedObject<T> : BaseObject where T : BaseObject
    {
        private string mTypename = string.Empty;
        private T mInstance = null;

        public override void Serialize(BaseStream stream)
        {
            if (!stream.IsReading() && mInstance != null)
            {
                mTypename = mInstance.GetType().FullName;
            }
            stream.SerializeString("Typename", ref mTypename);
            if (mInstance == null && mTypename != string.Empty)
            {
                Type type = Type.GetType(mTypename);
                if (type != null)
                {
                    mInstance = Activator.CreateInstance(type) as T;
                }
            }
            if (mInstance != null)
            {
                stream.SerializeObject("Instance", mInstance);
            }
        }

        public T GetInstance() { return mInstance; }
        public void SetInstance(T instance) { mInstance = instance; }
    }
}
