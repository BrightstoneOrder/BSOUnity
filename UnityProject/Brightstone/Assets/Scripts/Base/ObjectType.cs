

namespace Brightstone
{
	public class ObjectType 
	{
        private const char SCOPE_CHAR = '/';

        private string mBaseType = string.Empty;
        private string mScope = string.Empty;
        private string mName = string.Empty;
        private int mID = 0;

        public void InternalInit(string baseType, string fullname, int id)
        {
            if(fullname == string.Empty || fullname[0] != SCOPE_CHAR)
            {
                Log.Game.Info("ObjectType.InternalInit has invalid fullname. Fullname=" + fullname);
                return;
            }
            
            int lastScopeIndex = fullname.FindLast(SCOPE_CHAR);
            // "/Typename"
            if(lastScopeIndex == 0)
            {
                mScope = fullname.Substring(1);
                mName = mScope;
            }
            else // "/Scope/TypeName"
            {
                mScope = fullname.Substring(1, lastScopeIndex - 1);
                mName = fullname.Substring(lastScopeIndex + 1);
            }
            mBaseType = baseType;
            mID = id;
        }

        public string GetBaseType() { return mBaseType; }
        public string GetName() { return mName; }
        public string GetScope() { return mScope; }
        public string GetFullName() { return SCOPE_CHAR + mScope + SCOPE_CHAR + mName; }
        public int GetID() { return mID; }
	}
}