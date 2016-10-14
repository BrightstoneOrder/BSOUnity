using System.Collections.Generic;

namespace Brightstone
{
    public class StreamContext
    {
        private List<StreamObject> mObjects = new List<StreamObject>();
        private string mTypeName = string.Empty;
        private string mParentTypeName = string.Empty;

        public StreamContext()
        {
            StreamObject root = new StreamObject();
            root.SetRoot(true);
            mObjects.Add(root); // Add root
        }

        public StreamObject GetTop()
        {
            return mObjects[mObjects.Count - 1];
        }


        public void Init(string typeName, string parentTypeName)
        {
            mTypeName = typeName;
            mParentTypeName = parentTypeName;
        }

        public void Push(StreamObject obj)
        {
            mObjects.Add(obj);
        }
        public void Pop()
        {
            if(GetTop().IsRoot())
            {
                return;
            }
            mObjects.RemoveAt(mObjects.Count - 1);
        }

        public string GetTypeName() { return mTypeName; }
        public string GetParentTypeName() { return mParentTypeName; }

    }
}
