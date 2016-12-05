using System.Collections.Generic;
using UnityEngine;

namespace Brightstone
{
    // Data class used for serilaizing type information.
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
        // Prefab System...
        // Init.. Load typemap.. eg Strings / ID

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
        public class Flyweight
        {
            private string mName = string.Empty;
            private int mNameId = -1;
            private GameObject mInstance = null;
            private List<Actor> mInstances = new List<Actor>();
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
                if (mInstance != null)
                {
                    Log.Sys.Error("Cannot overwrite Flyweight instance.");
                    return;
                }
                if (instance == null)
                {
                    Log.Sys.Error("Cannot set null instance. Use Unload instead!");
                    return;
                }
                mInstance = instance;
                mNameId = mInstance.GetInstanceID();
            }

            public void CompleteLoad()
            {
                SetInstance(mLoadRequest.asset as GameObject);
            }

            public void RegisterInstance(Actor instance)
            {
                mInstances.Add(instance);
            }

            public void UnregisterInstance(Actor instance)
            {
                mInstances.Remove(instance);
            }

            /** Read only please, no Add/Remove */
            public List<Actor> GetInstances() { return mInstances; }

            /** Update the flyweight.. Check on loading asset. */
            public void Update()
            {
                if (mInstance != null && mLoadRequest != null)
                {
                    Log.Sys.Error("Instance and Load request exist! Only one can survive!");
                    Purge();
                    Object.Destroy(mInstance);
                    mInstance = null;
                }

                if (mLoadRequest != null && mLoadRequest.isDone)
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
                if (mInstance != null)
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
                for (int i = 0; i < mInstances.Count; ++i)
                {
                    Actor instance = mInstances[i];
                    if (instance != null)
                    {
                        World.ActiveWorld.DestroyActor(instance);
                        Object.Destroy(instance);
                    }
                }

                mInstances.Clear();
            }

            /** Remove any null instances that weren't cleared out properly.*/
            private void CleanInstances()
            {
                for (int i = mInstances.Count - 1; i >= 0; --i)
                {
                    if (mInstances[i] == null)
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

        
        // TypeMgr:

        public const string IMPORT_DATA_LOCATION = "/TypeMap2.txt";
        // Consider using dictoinary when dealing with 5k + types.
        private TypeMap mTypeMap = new TypeMap();
        //private List<ObjectType> mTypes = new List<ObjectType>();
        private ObjectType[] mTypes = null;
        private Flyweight[] mFlyweights = null;
        private List<Flyweight> mPendingLoads = new List<Flyweight>();


        public void Init()
        {
            // Load Type Map:
            mTypeMap.Load();
            
            // Create Type & Flyweights:
            List<TypeData> mapTypes = mTypeMap.GetTypes();
            mTypes = new ObjectType[mapTypes.Count];
            mFlyweights = new Flyweight[mapTypes.Count];
            for (int i = 0; i < mapTypes.Count; ++i)
            {
                TypeData data = mapTypes[i];
                InternalCreateType(data.baseName, data.name, i);
            }

            // Link Types:
            for(int i = 0; i < mTypes.Length; ++i)
            {
                InternalLinkType(i);
            }
        }

        private void InternalCreateType(string derived, string type, int id)
        {
            ObjectType objType = new ObjectType();
            objType.InternalInit(derived, type, id);
            mTypes[id] = objType;

            Flyweight flyweight = new Flyweight();
            flyweight.SetName(type);
            flyweight.SetNameId(id);
            mFlyweights[id] = flyweight;
        }

        private void InternalLinkType(int id)
        {
            ObjectType objType = mTypes[id];
            if(string.IsNullOrEmpty(objType.GetBaseName()))
            {
                ObjectType baseType = FindType(objType.GetBaseName());
                if(baseType != null)
                {
                    objType.Link(baseType);
                }
            }
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
                for(int i = 0; i < mTypes.Length; ++i)
                {
                    if(mTypes[i].GetFullName() == typename)
                    {
                        return mTypes[i];
                    }
                }
            }
            else
            {
                for(int i = 0; i <mTypes.Length; ++i)
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
            if (id >= 0 && id < mTypes.Length)
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
            if (id < 0 || id >= mFlyweights.Length)
            {
                id = -1;
            }
            if(id == -1)
            {
                for(int i = 0; i < mFlyweights.Length; ++i)
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


        private Flyweight GetFlyweight(int id)
        {
            if (id >= 0 && id < mFlyweights.Length)
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

        public Actor CreateInstance(Prefab prefab)
        {
            // Valid Check:
            if(prefab == null || !prefab.IsLoaded())
            {
                return null;
            }
            Flyweight flyweight = GetFlyweight(prefab);
            if(flyweight == null || flyweight.GetInstance() == null)
            {
                return null;
            }
            // TODO: Check if flyweight is broken, ie doesnt have actor.

            // Create Clone:
            GameObject clone = Object.Instantiate<GameObject>(flyweight.GetInstance());
            if(clone == null)
            {
                return null;
            }
            clone.name = clone.name.Substring(0, clone.name.Length - 7);
            
            Actor actor = clone.GetComponent<Actor>();
            flyweight.RegisterInstance(actor);
            actor.InternalInitType(FindType(prefab.GetFlyweightId()));
            actor.InternalInit();
            return actor;
        }

        public void DestroyInstance(Actor instance)
        {
            if(instance != null && !instance.IsGarbage() && instance.GetObjectType() != null)
            {
                ObjectType type = instance.GetObjectType();
                Flyweight flyweight = GetFlyweight(type.GetID());
                if (flyweight != null)
                {
                    flyweight.UnregisterInstance(instance);
                }
                instance.InternalDestroy();
                Object.Destroy(instance.gameObject);
            }
        }

        // Called when an actor is created in an unconventional way. 
        // (Ie level start)
        public void RegisterInstance(Actor instance)
        {
            if(instance != null && instance.GetObjectPrefabType() != null && instance.GetObjectType() == null)
            {
                Prefab prefab = instance.GetObjectPrefabType();
                Flyweight flyweight = GetFlyweight(prefab);
                if(flyweight != null)
                {
                    instance.InternalInitType(FindType(flyweight.GetNameId()));
                }
            }
        }

        // Called when an actor is destroyed in an unconventional way.
        // (Ie level shutdown)
        public void UnregisterInstance(Actor instance)
        {
            if(instance != null)
            {
                ObjectType type = instance.GetObjectType();
                if(type == null)
                {
                    int c = 0;
                    if (c == 1) { }
                }
                Flyweight flyweight = GetFlyweight(type.GetID());
                if (flyweight != null)
                {
                    flyweight.UnregisterInstance(instance);
                }
            }
        }


        public void InternalUpdate()
        {
            // Check Pending Loads..
            for(int i = mPendingLoads.Count-1; i >=0; --i)
            {
                Flyweight flyweight = mPendingLoads[i];
                if (!flyweight.IsLoading() && flyweight.IsAsyncLoading())
                {
                    mPendingLoads.RemoveAt(i);
                    Log.Sys.Info("Finished loading " + flyweight.GetName());
                    flyweight.CompleteLoad();
                }
            }

        }
	}
}