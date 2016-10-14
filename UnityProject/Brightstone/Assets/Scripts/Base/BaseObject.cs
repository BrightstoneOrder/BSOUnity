namespace Brightstone
{
	public class BaseObject 
	{
        protected string mName = string.Empty;

        /** Called to serialize the object's properties.*/
        public virtual void Serialize(BaseStream stream) { } 

        public string GetName() { return mName; }
        public void SetName(string name) { mName = name; }
	}
}