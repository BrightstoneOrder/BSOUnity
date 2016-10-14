using System.Collections.Generic;

namespace Brightstone
{
    public class StreamObject
    {
        private bool mRoot = false;
        private StreamPropertyType mType = StreamPropertyType.SPT_OBJECT;
        private string mName = string.Empty;
        private int mNameID = 0;
        private List<StreamProperty> mProperties = new List<StreamProperty>();
        private List<StreamObject> mChildren = new List<StreamObject>();
        private List<object> mStreamObjects = new List<object>();
        private StreamObject mParent = null;

        

        public bool IsRoot() { return mRoot; }
        public void SetRoot(bool value) { mRoot = value; }
        public StreamPropertyType GetPropertyType() { return mType; }
        public void SetType(StreamPropertyType type) { mType = type; }
        public int GetArraySize() { return mStreamObjects.Count; }
        public string GetName() { return mName; }
        public void SetName(string name) { mName = name; }
        public int GetNameID() { return mNameID; }
        public void SetNameID(int name) { mNameID = name; }

        public StreamObject GetParent() { return mParent; }
        public StreamObject FindChild(string name)
        {
            for(int i = 0; i < mChildren.Count; ++i)
            {
                if(mChildren[i].mName == name)
                {
                    return mChildren[i];
                }
            }
            return null;
        }
        public StreamObject FindChild(int name)
        {
            for (int i = 0; i < mChildren.Count; ++i)
            {
                if (mChildren[i].mNameID == name)
                {
                    return mChildren[i];
                }
            }
            return null;
        }
        public StreamObject FindChild(string name, bool create)
        {
            StreamObject child = FindChild(name);
            if (child != null)
            {
                return child;
            }
            if (!create)
            {
                return null;
            }
            child = new StreamObject();
            child.SetName(name);
            child.mParent = this;
            mChildren.Add(child);
            mStreamObjects.Add(child);
            return child;
        }
        public StreamProperty FindProperty(string name)
        {
            for(int i = 0; i < mProperties.Count; ++i)
            {
                if(mProperties[i].GetName() == name)
                {
                    return mProperties[i];
                }
            }
            return null;
        }
        public StreamProperty FindProperty(int name)
        {
            for (int i = 0; i < mProperties.Count; ++i)
            {
                if (mProperties[i].GetNameID() == name)
                {
                    return mProperties[i];
                }
            }
            return null;
        }

        public StreamProperty FindProperty(string name, bool create)
        {
            StreamProperty property = FindProperty(name);
            if(property != null)
            {
                return property;
            }
            if(!create)
            {
                return null;
            }
            property = new StreamProperty();
            property.SetName(name);
            mProperties.Add(property);
            mStreamObjects.Add(property);
            return property;
        }

        public StreamProperty AddProperty()
        {
            StreamProperty property = null;
            property = new StreamProperty();
            mProperties.Add(property);
            mStreamObjects.Add(property);
            return property;
        }

        public StreamProperty AddProperty(StreamProperty prop)
        {
            mProperties.Add(prop);
            mStreamObjects.Add(prop);
            return prop;
        }

        public StreamObject AddChild()
        {
            StreamObject child = new StreamObject();
            child.mParent = this;
            mChildren.Add(child);
            mStreamObjects.Add(child);
            return child;
        }

        public object GetPropertyAtIndex(int index)
        {
            return mStreamObjects[index];
        }

        public void Clear()
        {
            mChildren.Clear();
            mProperties.Clear();
            mStreamObjects.Clear();
        }

        public List<object> GetProperties()
        {
            return mStreamObjects;
        }
    }
}
