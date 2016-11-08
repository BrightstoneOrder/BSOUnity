using UnityEngine;
using System;

namespace Brightstone
{
    /** 
     * The idea of the prefab is to allow the runtime loading of assets.
     * 
     * Eg Lets say you have a class
     * 
     * class UnitSpawner
     * {
     *     [PrefabView]
     *     [SerializedField]
     *     Prefab mPrefab = new Prefab();
     *     
     *     void OnInit()
     *     {
     *        // If I want to use this asset right away
     *        mPrefab.ForceLoad();
     *        // If I want to use this asset in the future but having it loaded now is not critical.
     *        mPrefab.Prepare();
     *        // If the asset isn't very critical *eg typecompare* I can use Acquire
     *        mPrefab.Acquire();
     *        
     *        // Now if I want to check if the prefab is valid for creating types
     *        mPrefab.IsLoaded();
     *        
     *        // Now if I want to create something
     *        GameObject gameObject = World.ActiveWorld.CreateActor(mPrefab)
     *     }
     * }
     * 
     * -- Prefab View --
     * [ Set Type] / [ Set Base ] -- Depends if type of actor or type of resource
     * { Type Name:  }
     * { Base Name:  }
     * 
     * TODO: 
     * * Fix up other code relying on ObjectType.. Were going to remove it completely
     * * Give TypeMap its own place.. Maybe in editor only land?
     * * Add IsA to TypeMap to prevent cycles..
     * * Add version ID tracking to prevent loss of data when switching types.
     * * Add a list view to show broken types (ie missing parent or type with name doesn't exist or type id mismatch).
     * * WRITE TESTS!!!
     */
    [Serializable]
    public class Prefab
    {
#if UNITY_EDITOR
        

        /** Instance for in editor use. That way types can be verified.*/
        [SerializeField]
        private GameObject mEditorInstance = null;
#endif
        /** Name string assigned to the prefab.*/
        [SerializeField]
        private string mName = string.Empty;
        [SerializeField]
        private string mBaseName = string.Empty;
        /** Flyweight type id handle.*/
        private int mFlyweightId = -1;

        /** Prepare an asset for use in the future.*/
        public void Prepare()
        {
            World.ActiveWorld.GetTypeMgr().LoadType(TypeMgr.LoadMode.LM_ASYNC_LOAD, this);
        }

        /** Force an asset to be loaded now.*/
        public void ForceLoad()
        {
            World.ActiveWorld.GetTypeMgr().LoadType(TypeMgr.LoadMode.LM_LOAD, this);
        }

        /** Acquire a type handle for comparison. */
        public void Acquire()
        {
            World.ActiveWorld.GetTypeMgr().LoadType(TypeMgr.LoadMode.LM_ACQUIRE, this);
        }

        public bool IsLoaded()
        {
            return World.ActiveWorld.GetTypeMgr().IsLoaded(this);
        }

#if UNITY_EDITOR
        public GameObject GetEditorInstance() { return mEditorInstance; }
        public void SetEditorInstance(GameObject instance) { mEditorInstance = instance; }
#endif

        /** Get and set the name of the type. Note: Set is reserved for internal use only!*/
        public string GetName() { return mName; }
        public void SetName(string name) { mName = name; }

        /** Get and set the base name of the type. Note: Set is reserved for internal use only!*/
        public string GetBaseName() { return mBaseName; }
        public void SetBaseName(string name) { mBaseName = name; }

        /** Get and set the flyweight id. Note: Set is reserved for internal use only!*/
        public int GetFlyweightId() { return mFlyweightId; }
        public void SetFlyweightId(int id) { mFlyweightId = id; }
    }
}