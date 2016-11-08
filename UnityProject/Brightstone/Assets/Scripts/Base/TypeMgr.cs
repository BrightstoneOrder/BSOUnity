using System.Collections.Generic;
using UnityEngine;

namespace Brightstone
{
    public class TypeData : BaseObject
    {
        public string name = string.Empty;
        public string baseName = string.Empty;

        public override void Serialize(BaseStream stream)
        {
            stream.SerializeString("Name", ref name);
            stream.SerializeString("BaseName", ref baseName);
        }
    }

    public class TypeMap : BaseObject
    {
        private List<TypeData> mTypes = new List<TypeData>();
        private bool mLoaded = false;
        private bool mDirty = false;

        public override void Serialize(BaseStream stream)
        {
            stream.Serialize("mTypes", ref mTypes);
        }

        public void Load()
        {
            string path = Util.GetUserDataDirectory(TypeMgr.IMPORT_DATA_LOCATION);

            if(System.IO.File.Exists(path))
            {
                string allText = System.IO.File.ReadAllText(path);
                TextStream stream = new TextStream();
                stream.SetReadingMode(true);
                stream.ParseText(allText);
                stream.NextContext();
                Serialize(stream);
                stream.StopContext();
                mLoaded = true;
            }
        }

        public void Save()
        {
            if(!mDirty)
            {
                return;
            }
            string path = Util.GetUserDataDirectory(TypeMgr.IMPORT_DATA_LOCATION);

            TextStream stream = new TextStream();
            stream.SetReadingMode(false);
            stream.StartContext("TypeMap", "BaseObject");
            Serialize(stream);
            stream.StopContext();
            System.IO.File.WriteAllText(path, stream.WriteText());
            mDirty = false;
        }

        public string GetBaseName(string typeName)
        {
            for(int i = 0; i < mTypes.Count; ++i)
            {
                if(mTypes[i].name == typeName)
                {
                    return mTypes[i].baseName;
                }
            }
            return string.Empty;
        }

        public void SetName(string oldName, string newName)
        {
            for (int i = 0; i < mTypes.Count; ++i)
            {
                if (mTypes[i].name == oldName)
                {
                    SetDirty();
                    mTypes[i].name = newName;
                }
                else if(mTypes[i].baseName == oldName)
                {
                    mTypes[i].baseName = newName;
                    SetDirty();
                }
            }

        }

        public void SetBaseName(string oldName, string newName)
        {
            for(int i = 0; i < mTypes.Count; ++i)
            {
                if(mTypes[i].baseName == oldName)
                {
                    SetDirty();
                    mTypes[i].baseName = newName;
                    break;
                }
            }
        }

        public List<TypeData> GetTypes() { return mTypes; }
        public void SetTypes(List<TypeData> types) { mTypes = types; }
        public bool IsLoaded() { return mLoaded; }
        public bool IsDirty() { return mDirty; }
        public void SetDirty() { mDirty = true; }
    }

    public class TypeMgr 
	{
        public const string IMPORT_DATA_LOCATION = "/TypeMap2.txt";
        // Consider using dictoinary when dealing with 5k + types.
        private TypeMap mTypeMap = new TypeMap();
        private List<ObjectType> mTypes = new List<ObjectType>();

        private void LoadTypeMap()
        {
            /*
            string path = Util.GetUserDataDirectory(IMPORT_DATA_LOCATION);
            if(System.IO.File.Exists(path))
            {
                string allText = System.IO.File.ReadAllText(path);
                TextStream stream = new TextStream();
                stream.SetReadingMode(true);
                stream.ParseText(allText);
                stream.NextContext();
                mTypeMap.Serialize(stream);
                stream.StopContext();
            }
            // TODO: Linking & Verifying
            List<ObjectType> types = mTypeMap.GetTypes();
            mTypes = new List<ObjectType>(types);
            
            // Verify
            for(int i = 0; i < mTypes.Count; ++i)
            {
                mTypes[i] = null;
            }
            
            for (int i = 0; i < mTypes.Count; ++i)
            {
                int index = types[i].GetID();
                if(index < 0 || index >= mTypes.Count)
                {
                    Log.Sys.Error("Failed to verify type because id is out of range! Type=" + types[i].GetFullName() + ", Id=" + types[i].GetID() + ", ID should be " + i);
                }
                else if(mTypes[i] != null)
                {
                    Log.Sys.Error("Failed to verify type because a type with the id already exists! Existing Type=" + mTypes[i].GetFullName() + ", Type=" + types[i].GetFullName() + ", Id=" + i);
                }
                else
                {
                    mTypes[i] = types[i];
                }
            }

#if !BS_TYPE_VERIFY
            for(int i = 0; i < mTypes.Count; ++i)
            {
                for(int j = 0; j < mTypes.Count; ++j)
                {
                    if(i == j)
                    {
                        continue;
                    }
                    if(mTypes[i].GetFullName() == mTypes[j].GetFullName())
                    {
                        Log.Sys.Error("Two types exist with the same name. Lhs=" + i + ", Rhs=" + j + ", Name=" + mTypes[i].GetFullName());
                    }
                }
            }
#endif

            // Link
            for(int i = 0; i < mTypes.Count; ++i)
            {
                string baseName = mTypes[i].GetBaseName();
                if(mTypes[i].GetFullName() == "/Engine/Actor")
                {
                    continue;
                }
                if(string.IsNullOrEmpty(baseName))
                {
                    Log.Sys.Error("Failed to link type " + mTypes[i].GetFullName() + "[" + mTypes[i].GetID() + "]. Missing base name.");
                    continue;
                }
                ObjectType type = FindType(baseName);
                if(type == null)
                {
                    Log.Sys.Error("Failed to link type " + mTypes[i].GetFullName() + "[" + mTypes[i].GetID() + "]. Missing base type.");
                    continue;
                }

                mTypes[i].Link(type);
            }*/

        }

        /**
         * Finds a type using their name.
         * eg. FindType("ClickEffect")
         *     FindType("/Effects/ClickEffect/ClickEffect")
        */
        public ObjectType FindType(string typename)
        {
            bool findWithScope = typename.IndexOf(ObjectType.SCOPE_CHAR) != -1;
            if(findWithScope)
            {
                for(int i = 0; i < mTypes.Count; ++i)
                {
                    if(mTypes[i].GetFullName() == typename)
                    {
                        return mTypes[i];
                    }
                }
            }
            else
            {
                for(int i = 0; i <mTypes.Count; ++i)
                {
                    if(mTypes[i].GetName() == typename)
                    {
                        return mTypes[i];
                    }
                }
            }
            return null;
        }

        /**
         * Finds a type using their id.
         */
        public ObjectType FindType(int id)
        {
            if (id >= 0 && id < mTypes.Count)
            {
                return mTypes[id];
            }
            return null;
        }
        
        // Is a derived from b
        public bool IsDerived(ObjectType a, ObjectType b)
        {
            ObjectType typeA = FindType(a.GetFullName());
            ObjectType typeB = FindType(b.GetFullName());
            ObjectType it = typeB;
            while(it != null && it.GetBaseType() != null)
            {
                if(it == typeA)
                {
                    return true;
                }
                it = typeB.GetBaseType();
            }
            return false;
        }

        private void SaveTypeMap()
        {
            string path = Util.GetUserDataDirectory(IMPORT_DATA_LOCATION);
            TextStream stream = new TextStream();
            stream.SetReadingMode(false);
            stream.StartContext("TypeMap", "BaseObject");
            mTypeMap.Serialize(stream);
            stream.StopContext();
            string allText = stream.WriteText();
            System.IO.File.WriteAllText(path, allText);
        }

        public void Init()
        {
            LoadTypeMap();
        }

        public void Shutdown(bool save)
        {
            if(save)
            {
                // SaveTypeMap();
            }
            mTypes = null;
            mTypeMap = new TypeMap();
        }
        

        

        private void InternalCreateType(string derived, string type, int id)
        {
            
        }

        private void InitActor(GameObject gameObject)
        {

        }

        private void DestroyActor(GameObject gameObject)
        {

        }

        // Prefab System...
        // Init.. Load typemap.. eg Strings / ID

        public class Flyweight
        {
            private string mName = string.Empty;
            private int mNameId = -1;
            private GameObject mInstance = null;
            private List<GameObject> mInstances = new List<GameObject>();
            private ResourceRequest mLoadRequest = null;


            public bool IsLoaded()
            {
                return mInstance != null;
            }

            /** A flyweight is async loading until processed and taken off queue. */
            public bool IsAsyncLoading()
            {
                return mLoadRequest != null;
            }

            /** Returns true if there is a load request and it hasn't finished yet.*/
            public bool IsLoading()
            {
                return mLoadRequest != null && !mLoadRequest.isDone;
            }

            /** Returns the progress % of the loaded asset. */
            public float GetProgress()
            {
                return IsLoading() ? mLoadRequest.progress : 0.0f;
            }

            public string GetName() { return mName; }
            public void SetName(string name) { mName = name; }

            public int GetNameId() { return mNameId; }
            public void SetNameId(int id) { mNameId = id; }

            public GameObject GetInstance() { return mInstance; }
            public void SetInstance(GameObject instance)
            {
                if(mInstance != null)
                {
                    Log.Sys.Error("Cannot overwrite Flyweight instance.");
                    return;
                }
                if(instance == null)
                {
                    Log.Sys.Error("Cannot set null instance. Use Unload instead!");
                    return;
                }
                mInstance = instance;
                mNameId = mInstance.GetInstanceID();
            }
            
            public void RegisterInstance(GameObject instance)
            {
                mInstances.Add(instance);
            }

            public void UnregisterInstance(GameObject instance)
            {
                mInstances.Remove(instance);
            }

            /** Read only please, no Add/Remove */
            public List<GameObject> GetInstances() { return mInstances; }

            /** Update the flyweight.. Check on loading asset. */
            public void Update()
            {
                if(mInstance != null && mLoadRequest != null)
                {
                    Log.Sys.Error("Instance and Load request exist! Only one can survive!");
                    Purge();
                    Object.Destroy(mInstance);
                    mInstance = null;
                }

                if(mLoadRequest != null && mLoadRequest.isDone)
                {
                    mInstance = mLoadRequest.asset as GameObject;
                    mLoadRequest = null;
                }

                CleanInstances();
            }

            public void Unload()
            {
                Purge();
                mNameId = -1;
                Resources.UnloadAsset(mInstance);
                if(mInstance != null)
                {
                    Object.Destroy(mInstance);
                    mInstance = null;
                }
            }

            public void SetResourceRequest(ResourceRequest request) { mLoadRequest = request; }

            public bool HasInstances()
            {
                return mInstances.Count > 0;
            }

            /** Destroy all instances! */
            private void Purge()
            {
                for(int i = 0; i < mInstances.Count; ++i)
                {
                    GameObject instance = mInstances[i];
                    if(instance != null)
                    {
                        World.ActiveWorld.GetTypeMgr().DestroyActor(instance);
                        Object.Destroy(instance);
                    }
                }

                mInstances.Clear();
            }

            /** Remove any null instances that weren't cleared out properly.*/
            private void CleanInstances()
            {
                for(int i = mInstances.Count -1; i>=0; --i)
                {
                    if(mInstances[i] == null)
                    {
                        mInstances.RemoveAt(i);
                    }
                }
            }

            
            
        }

        public enum LoadMode
        {
            /** Don't load if its not loaded yet. */
            LM_ACQUIRE,
            /** Load the resource RIGHT now. */
            LM_LOAD,
            /** Prepare the resource to be loaded in the future.*/
            LM_ASYNC_LOAD,
        }

        private List<Flyweight> mFlyweights = new List<Flyweight>();
        private List<Flyweight> mPendingLoads = new List<Flyweight>();


        /**
         * Searches for a 'Flyweight' based on the prefabs Name
         * If this prefab was used before to acquire a flyweight it may have
         * the flyweight id cached in which case it will be used.
         * Otherwise does a O(n) search to find flyweight.
         * 
        */
        private Flyweight GetFlyweight(Prefab prefab)
        {
            int id = prefab.GetFlyweightId();
            if (id < 0 || id >= mFlyweights.Count)
            {
                id = -1;
            }
            if(id == -1)
            {
                for(int i = 0; i < mFlyweights.Count; ++i)
                {
                    if(mFlyweights[i].GetName() == prefab.GetName())
                    {
                        prefab.SetFlyweightId(i);
                        return mFlyweights[i];
                    }
                }
            }
            else
            {
                return mFlyweights[id];
            }
            return null;
        }

        private void InternalLoadType(Flyweight flyweight, Prefab prefab)
        {
            if(flyweight.GetInstance() != null)
            {
                Log.Sys.Error("Failed to InternalLoadType! Flyweight is already loaded! Name=" + prefab.GetName());
                return;
            }
            if(flyweight.HasInstances())
            {
                Log.Sys.Error("Failed to InternalLoadType! Flyweight still has instances! They must be destroyed before loading. Name=" + prefab.GetName());
                return;
            }
            if(flyweight.IsAsyncLoading())
            {
                Log.Sys.Error("Failed to InternalLoadType! Flyweight is already being loaded! Name=" + prefab.GetName());
                return;
            }

            flyweight.SetInstance(Resources.Load(flyweight.GetName()) as GameObject);
        }

        private void InternalLoadAsyncType(Flyweight flyweight, Prefab prefab)
        {
            if (flyweight.GetInstance() != null)
            {
                Log.Sys.Error("Failed to InternalLoadType! Flyweight is already loaded! Name=" + prefab.GetName());
                return;
            }
            if (flyweight.HasInstances())
            {
                Log.Sys.Error("Failed to InternalLoadType! Flyweight still has instances! They must be destroyed before loading. Name=" + prefab.GetName());
                return;
            }
            if (flyweight.IsAsyncLoading())
            {
                Log.Sys.Error("Failed to InternalLoadType! Flyweight is already being loaded! Name=" + prefab.GetName());
                return;
            }

            flyweight.SetResourceRequest(Resources.LoadAsync(flyweight.GetName()));
            if(!flyweight.IsAsyncLoading())
            {
                Log.Sys.Error("Failed to InternalLoadType! Asset doesn't exist. Name=" + flyweight.GetName());
                return;
            }
            mPendingLoads.Add(flyweight);
        }

        public bool IsLoaded(Prefab prefab)
        {
            Flyweight flyweight = GetFlyweight(prefab);
            if (flyweight == null)
            {
                Log.Sys.Error("Failed to acquire a flyweight for prefab. Name=" + prefab.GetName());
                return false;
            }
            return flyweight.IsLoaded();
        }

        public void LoadType(LoadMode mode, Prefab prefab)
        {
            if(prefab == null)
            {
                Log.Sys.Error("Failed to LoadType because prefab is not initialized!");
                return;
            }
            if(string.IsNullOrEmpty(prefab.GetName()))
            {
                Log.Sys.Error("Failed to LoadType because prefab string is empty.");
                return;
            }

            Flyweight flyweight = GetFlyweight(prefab);
            if(flyweight == null)
            {
                Log.Sys.Error("Failed to acquire a flyweight for prefab. Name=" + prefab.GetName());
                return;
            }
            if(flyweight.GetInstance() != null)
            {
                return; // Resource is already loaded.
            }
            // Don't have an instance...
            switch(mode)
            {
                case LoadMode.LM_ACQUIRE:
                    {
                        // No instance, do nothing
                    }
                    break;
                case LoadMode.LM_LOAD:
                    {
                        InternalLoadType(flyweight, prefab);
                    }
                    break;
                case LoadMode.LM_ASYNC_LOAD:
                    {
                        InternalLoadAsyncType(flyweight, prefab);
                    }
                    break;
            }
        }
	}
}