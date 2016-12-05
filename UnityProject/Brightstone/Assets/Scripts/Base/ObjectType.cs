using UnityEngine;
using System;

namespace Brightstone
{

    [Serializable]
	public class ObjectType : BaseObject
	{
        public const char SCOPE_CHAR = '/';

        private ObjectType mBaseType = null;

        // Editor Only Data String...
#if UNITY_EDITOR
        [SerializeField]
        private string mDataString = string.Empty;
#endif

        private string mBaseName = string.Empty;
        private string mScope = string.Empty;
        private int mID = 0;

#if UNITY_EDITOR
        public void InternalInitFromDataString()
        {
            if(string.IsNullOrEmpty(mDataString))
            {
                return;
            }

            // corrupt
            if(mDataString[0] == ':')
            {
                mDataString = mDataString.Substring(1);
            }

            int typeNameStart = 0;
            int baseNameStart = 0;
            for(int i = 0; i < mDataString.Length; ++i)
            {
                if(typeNameStart == 0 && mDataString[i] == ':')
                {
                    typeNameStart = i;
                }
                else if(baseNameStart == 0 && mDataString[i] == ':')
                {
                    baseNameStart = i;
                    break;
                }
            }

            if (typeNameStart == 0 && baseNameStart == 0)
            {
                mDataString = "BROKEN:BROKEN:0";
                InternalInitFromDataString();
                return;
            }
            else if (baseNameStart == 0)
            {
                mDataString = "BROKEN:" + mDataString;
                InternalInitFromDataString();
                return;
            }
            
            string fullName = mDataString.Substring(0, typeNameStart);
            string baseName = mDataString.Substring(typeNameStart + 1, baseNameStart - typeNameStart - 1);
            string idString = mDataString.Substring(baseNameStart + 1);
            int id = 0;
            int.TryParse(idString, out id);
            InternalInit(baseName, fullName, id);
        }
#endif

        public void InternalInit(string baseType, string fullname, int id)
        {
            if(fullname == string.Empty)
            {
                Log.Sys.Info("ObjectType.InternalInit has invalid fullname. Fullname=" + fullname);
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
            mBaseName = baseType;
            mID = id;
#if UNITY_EDITOR
            mDataString = GetFullName() + ":" + GetBaseName() + ":" + GetID();
#endif
        }

        public override void Serialize(BaseStream stream)
        {
            string fullName = GetFullName();
            stream.SerializeString("Name", ref fullName);
            stream.SerializeString("Base", ref mBaseName);
            stream.SerializeInt("Id", ref mID);
            if(stream.IsReading())
            {
                InternalInit(mBaseName, fullName, mID);
            }
        }

        public void Link(ObjectType baseType)
        {
            mBaseType = baseType;
        }

        public string GetBaseName() { return mBaseName; }
        public string GetScope() { return mScope; }
        public string GetFullName() { return string.IsNullOrEmpty(mScope) && string.IsNullOrEmpty(mName) ? string.Empty : SCOPE_CHAR + mScope + SCOPE_CHAR + mName; }
        public int GetID() { return mID; }
        public ObjectType GetBaseType() { return mBaseType; }
	}
}