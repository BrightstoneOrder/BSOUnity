﻿using UnityEngine;
namespace Brightstone
{
	public class World : BaseComponent 
	{
        public static World ActiveWorld = null;

        // Semi-Global stuff
        InputMgr mInputMgr = null;
        OptionMgr mOptionMgr = null;
        PhysicsMgr mPhysicsMgr = null;
        TypeMgr mTypeMgr = null;
        UIMgr mUIMgr = null;

        private Timer mClock = new Timer();
        private const float MAX_DELTA = 0.1f;
        private float mLastTimeStamp = 0.0f;
        private float mGameDelta = 0.0f;
        private float mGameDeltaScale = 1.0f;
        private float mGameElapsedTime = 0.0f;
        private float mGameLevelElapsedTime = 0.0f;
        private float mApplicationDelta = 0.0f;
        private float mApplicationElapsedTime = 0.0f;
        private bool mGamePaused = false;

        

        private void Awake()
        {
            if(ActiveWorld != null)
            {
                Log.Game.Error("Multiple worlds are not allowed!");
            }
            ActiveWorld = this;
        }
        private void Start()
        {
            InternalInit();
            DontDestroyOnLoad(this); // World persists..
        }
        private void OnDestroy()
        {
            InternalDestroy();
        }

        protected override void OnInit()
        {
            Log.Sys.Info("Starting Options Mgr...");
            mOptionMgr = new OptionMgr();
            mOptionMgr.Init();

            Log.Sys.Info("Starting Type Mgr...");
            mTypeMgr = new TypeMgr();
            mTypeMgr.Init();

            Log.Sys.Info("Starting Input Mgr...");
            mClock.Start();
            mInputMgr = new InputMgr();
            mInputMgr.Init(this);

            Log.Sys.Info("Starting Physics Mgr...");
            mPhysicsMgr = new PhysicsMgr();
            mPhysicsMgr.Init();

            Log.Sys.Info("Starting UI Mgr...");
            mUIMgr = new UIMgr();
            mUIMgr.Init();

        }

        private void Update()
        {
            UpdateTime();
            mInputMgr.Update(this);
            mPhysicsMgr.Update(this);
            mTypeMgr.InternalUpdate();
            mUIMgr.Update(this);
        }

        private void UpdateTime()
        {
            mLastTimeStamp = mApplicationElapsedTime;
            mApplicationElapsedTime = mClock.GetElapsedSeconds();
            mApplicationDelta = mApplicationElapsedTime - mLastTimeStamp;
            if (mGamePaused)
            {
                mGameDelta = 0.0f;
            }
            else
            {
                mGameDelta = Mathf.Min(MAX_DELTA, mApplicationDelta) * mGameDeltaScale;
            }
            mGameLevelElapsedTime += mGameDelta;
            mGameElapsedTime += mGameDelta;
        }

        public float GetGameDelta() { return mGameDelta; }
        public float GetApplicationDelta() { return mApplicationDelta; }
        public float GetTime() { return mGameElapsedTime; }
        public Camera GetGameCamera() { return Camera.main; }
        public InputMgr GetInputMgr() { return mInputMgr; }
        public OptionMgr GetOptionsMgr() { return mOptionMgr; }
        public PhysicsMgr GetPhysicsMgr() { return mPhysicsMgr; }
        public TypeMgr GetTypeMgr() { return mTypeMgr; }
        public UIMgr GetUIMgr() { return mUIMgr; }

        // TODO: Maybe entity list bookkeeping n stuff.
        public Actor CreateActor(Prefab prefab)
        {
            return mTypeMgr.CreateInstance(prefab);
        }

        public T Create<T>(Prefab prefab) where T : Actor
        {
            return mTypeMgr.CreateInstance(prefab) as T;
        }

        // public Actor CloneActor(Actor actor)
        // {
        //     return mTypeMgr.CreateInstance(actor.GetObjectPrefabType());
        // }

        public void DestroyActor(Actor actor)
        {
            mTypeMgr.DestroyInstance(actor);
        }
	}
}