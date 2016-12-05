using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    // ====================================================
    // A class used to spawn units? 
    // Max Instances
    // Max Spawned
    // Spawn Delay
    // Spawn Interval
    // 
    // ====================================================
    public class UnitSpawner : BaseComponent
    {
        // Serialized:

        [Tooltip("Maximum number of units that can be spawned at any given time!")]
        [Range(1,999)]
        [SerializeField]
        private int mMaxInstances = 1;
        [Tooltip("Maximum number of units that can be spawned. 0=Infinite")]
        [Range(0, 10000)]
        private int mMaxSpawned = 0;
        [Tooltip("Time before spawning begins. Triggered when this becomes active. (Seconds)")]
        [Range(0.0f, 100000.0f)]
        [SerializeField]
        private float mSpawnDelay = 0.0f;
        [Tooltip("Time between each spawn AFTER spawn delay. (Seconds)")]
        [Range(0.0f, 100000.0f)]
        [SerializeField]
        private float mSpawnInterval = 0.0f;
        [Tooltip("Resets the spawner as if it was recreated when enabled.")]
        [SerializeField]
        private bool mResetOnEnable = true;
        [Tooltip("Type of unit to spawn.")]
        [PrefabView(typeof(Unit))]
        [SerializeField]
        private Prefab mSpawnType = null;

        // Private Properties:

        private bool mInitalSpawned = false;
        private int mUnitsSpawned = 0;
        private Timer mSpawnTimer = new Timer();
        private List<Unit> mSpawnedUnits = new List<Unit>();

        // Accessor:

        public int maxInstances { get { return mMaxInstances; } set { mMaxInstances = value; } }
        public int maxSpawned { get { return mMaxSpawned; } set { mMaxSpawned = value; } }
        public float spawnDelay { get { return mSpawnDelay; } set { mSpawnDelay = value; } }
        public float spawnInterval { get { return mSpawnInterval; } set { mSpawnInterval = value; } }
        public bool resetOnEnable { get { return mResetOnEnable; } set { mResetOnEnable = value; } }

        public Prefab GetSpawnType() { return mSpawnType; }
        public int GetUnitsSpawned() { return mUnitsSpawned; }
        public List<Unit> GetSpawnedUnits() { return mSpawnedUnits; }


        // Functions:

        private void Start()
        {
            InternalInit();
        }

        private void OnDestroy()
        {
            InternalDestroy();
        }

        private void OnEnable()
        {
            mInitalSpawned = mSpawnDelay <= 0.0f;
            if(mResetOnEnable)
            {
                ResetSpawns();
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            Log.Game.Info("Initializing Spawner...");
            if(mSpawnType != null)
            {
                mSpawnType.Prepare();
            }
        }

        private void Update()
        {
            // Exit early, not enabled or have nothing to spawn.
            if(!enabled || mSpawnType == null || !mSpawnType.IsLoaded())
            {
                return;
            }

            // Exit early, already spawned the limit.
            if(mUnitsSpawned == mMaxSpawned && mMaxSpawned != 0)
            {
                return;
            }

            // Exit early, already at the max instance limit.
            if(mSpawnedUnits.Count == mMaxInstances)
            {
                if(mSpawnTimer.IsRunning())
                {
                    mSpawnTimer.Reset();
                }
                return;
            }

            // Start timer:
            if(!mSpawnTimer.IsRunning())
            {
                mSpawnTimer.Start();
            }

            // Check if spawn:
            if(!mInitalSpawned && mSpawnTimer.GetElapsedSeconds() > mSpawnDelay)
            {
                Spawn();
                mSpawnTimer.Reset();
            }
            else if(mInitalSpawned && mSpawnTimer.GetElapsedSeconds() > mSpawnInterval)
            {
                Spawn();
                mSpawnTimer.Reset();
            }
        }

        public void ResetSpawns()
        {
            mInitalSpawned = false;
            mUnitsSpawned = 0;
            mSpawnTimer = new Timer();
            mSpawnedUnits = new List<Unit>();
        }

        private void Spawn()
        {
            Unit unit = mWorld.Create<Unit>(mSpawnType);
            unit.SetPosition(GetTransform().position);
            unit.SetRotation(GetTransform().rotation);
            mSpawnedUnits.Add(unit);
            ++mUnitsSpawned;
        }
        
    }
}


