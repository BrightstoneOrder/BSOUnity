using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class Spawner : BaseComponent
    {
        // TODO: Change this to a ObjectType and create with "World"
        [SerializeField] 
        private GameObject mPrefab = null;
        [SerializeField]
        private float mSpawnDelay = 0.0f;
        [SerializeField]
        private float mSpawnInterval = 0.0f;
        [SerializeField]
        private int mMaxSpawn = 0;

        private bool mSpawnedFirst = false;
        private Timer mSpawnTimer = new Timer();
        private List<Actor> mSpawnedActors = new List<Actor>();

        private void Start()
        {
            InternalInit();
        }

        private void OnDestroy()
        {
            InternalDestroy();
        }

        private void Update()
        {
            if(!mSpawnTimer.IsRunning() && mSpawnedActors.Count < mMaxSpawn)
            {
                mSpawnTimer.Start();
            }

            if(!mSpawnedFirst && mSpawnTimer.GetElapsedSeconds() > mSpawnDelay)
            {
                Spawn();
                mSpawnTimer.Reset();
            }
            else if(mSpawnedFirst && mSpawnTimer.GetElapsedSeconds() > mSpawnInterval)
            {
                Spawn();
                mSpawnTimer.Reset();
            }
        }

        private void Spawn()
        {
            if(mSpawnedActors.Count < mMaxSpawn)
            {
                GameObject go = Instantiate<GameObject>(mPrefab);
                Transform xform = go.transform;
                xform.position = GetTransform().position;
                xform.rotation = GetTransform().rotation;
                Actor actor = go.GetComponent(typeof(Actor)) as Actor;
                if(actor != null)
                {
                    actor.InternalInit();
                }
            }
        }
        
    }
}


