﻿using UnityEngine;
namespace Brightstone
{
	public class World : BaseComponent 
	{
        public static World ActiveWorld = null;

        // Semi-Global stuff
        InputMgr mInputMgr = null;
        OptionMgr mOptionMgr = null;

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
            mOptionMgr = new OptionMgr();
            mOptionMgr.Init();

            mClock.Start();
            mInputMgr = new InputMgr();
            mInputMgr.Init(this);

        }

        private void Update()
        {
            UpdateTime();
            mInputMgr.Update(this);
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
        public InputMgr GetInputMgr() { return mInputMgr; }
	}
}