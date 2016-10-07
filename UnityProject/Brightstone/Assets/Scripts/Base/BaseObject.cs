namespace Brightstone
{
	public class BaseObject 
	{
        protected string mName = string.Empty;

        public string GetName() { return mName; }
        public void SetName(string name) { mName = name; }
	}
}