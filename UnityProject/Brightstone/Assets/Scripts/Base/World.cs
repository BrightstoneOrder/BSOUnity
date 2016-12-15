using UnityEngine;
using System.Collections.Generic;
namespace Brightstone
{
	public class World : BaseComponent 
	{
        // Batch stages in order.
        private const int BATCH_STAGE_IDLE = 0;
        private const int BATCH_STAGE_REGISTER = 1; // Allowed to register
        private const int BATCH_STAGE_QUEUE = 2;    // Gets resources to load
        private const int BATCH_STAGE_LOADING = 3;  // Currently waiting for resources to load, no more registering
        private const int BATCH_STAGE_COMPLETE = 4; // Resources are loaded, let everyone know.

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

        private int mBatchStage = BATCH_STAGE_IDLE;
        private int mBatchRegisterFrameDelay = 0;
        private LinkedList<BaseComponent> mBatchQueued= new LinkedList<BaseComponent>();
        private LinkedList<BaseComponent> mBatchSubmitted = null;
        // Use a table to quickly identify if a load request has been submitted.
        // Then tally up unique loads requested and wait for that number to be 0 while updating.
        private bool[] mBatchTable = null;
        private LinkedList<int> mBatchLoadList = null;
        private ProfileTimer mProfileTimer = new ProfileTimer();

#if UNITY_EDITOR
        [SerializeField]
        private EditorConfig mEditorConfig = new EditorConfig();
#endif


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
#if UNITY_EDITOR
            EditorConfig.current = mEditorConfig;
#else
            EditorConfig.current = new EditorConfig();
#endif

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

            // Allow prefabs to load in batch.
            GotoBatchStage(BATCH_STAGE_REGISTER);
        }

        private void Update()
        {
            UpdateTime();
            mInputMgr.Update(this);
            mPhysicsMgr.Update(this);
            mTypeMgr.InternalUpdate();
            mUIMgr.Update(this);
            UpdateBatchLoading();
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


        private void UpdateBatchLoading()
        {
            switch(mBatchStage)
            {
                // Allow external input
                case BATCH_STAGE_IDLE:
                case BATCH_STAGE_REGISTER:
                    mProfileTimer.Start();
                    
                    --mBatchRegisterFrameDelay;
                    if(mBatchRegisterFrameDelay == 0)
                    {
                        GotoBatchStage(BATCH_STAGE_QUEUE);
                    }
                    break;
                // Allow only Prefab.Prepare
                case BATCH_STAGE_QUEUE:
                    GetBatchLoadRequests();
                    break;
                case BATCH_STAGE_LOADING:
                    CheckBatchLoad();
                    break;
                case BATCH_STAGE_COMPLETE:
                    CompleteBatchLoad();
                    mProfileTimer.Stop("BatchLoad");
                    break;
            }
        }

        // Called internally to register a component for batch loading.
        public void InternalRegisterBatchLoad(BaseComponent component)
        {
            if (mBatchStage == BATCH_STAGE_REGISTER)
            {
                mBatchQueued.AddLast(component);
            }
        }

        private void GotoBatchStage(int stage)
        {
            string stageString = string.Empty;
            switch (stage)
            {
                case BATCH_STAGE_IDLE:
                    stageString = "Idle";
                    break;
                case BATCH_STAGE_REGISTER:
                    mBatchRegisterFrameDelay = 5; // Use frame delay because sometimes components come in late..
                    stageString = "Register";
                    break;
                case BATCH_STAGE_QUEUE:
                    stageString = "Queue";
                    break;
                case BATCH_STAGE_LOADING:
                    stageString = "Loading";
                    break;
                case BATCH_STAGE_COMPLETE:
                    stageString = "Complete";
                    break;
            }
            Log.Sys.Info("World.GotoBatchStage " + stageString);
            mBatchStage = stage;
        }

        private void GetBatchLoadRequests()
        {
            GotoBatchStage(BATCH_STAGE_QUEUE);
            mBatchSubmitted = mBatchQueued;
            mBatchQueued = new LinkedList<BaseComponent>();
            mBatchTable = new bool[mTypeMgr.GetTypeCount()];
            for (int i = 0; i < mBatchTable.Length; ++i)
            {
                mBatchTable[i] = false;
            }
            // Get Load requests.. These should call InternalBatchLoad via Prefab.Prepare()
            for (LinkedListNode<BaseComponent> it = mBatchSubmitted.First; it != null; it = it.Next)
            {
                if(it.Value != null)
                {
                    it.Value.OnBatchSubmit();
                }
            }

            // Make list of typeIds we need to load.
            mBatchLoadList = new LinkedList<int>();
            for (int i =0; i < mBatchTable.Length; ++i)
            {
                if(mBatchTable[i])
                {
                    mBatchLoadList.AddLast(i);
                }
            }
            // Goto Loading
            mBatchTable = null;
            GotoBatchStage(BATCH_STAGE_LOADING);
        }

        private void CheckBatchLoad()
        {
            // Check and remove loaded types
            for(LinkedListNode<int> it = mBatchLoadList.First; it != null; )
            {
                if(mTypeMgr.IsLoaded(mTypeMgr.GetTypeAtIndex(it.Value)))
                {
                    LinkedListNode<int> garbage = it;
                    it = it.Next;
                    mBatchLoadList.Remove(garbage);
                }
            }

            if (mBatchLoadList.Count == 0)
            {
                GotoBatchStage(BATCH_STAGE_COMPLETE);
            }
        }

        private void CompleteBatchLoad()
        {
            // Notify Batch Is Complete
            for (LinkedListNode<BaseComponent> it = mBatchSubmitted.First; it != null; it = it.Next)
            {
                if (it.Value != null)
                {
                    it.Value.OnBatchComplete();
                    it.Value.SetBatchLoadComplete();
                }
            }

            mBatchLoadList = null;
            mBatchSubmitted = null;
            GotoBatchStage(BATCH_STAGE_IDLE);
        }

        public void InternalBatchLoad(Prefab prefab)
        {
            if(mBatchStage != BATCH_STAGE_QUEUE)
            {
                return;
            }
            // Only Valid if BATCH_STAGE_QUEUE 
            mTypeMgr.LoadType(TypeMgr.LoadMode.LM_ASYNC_LOAD, prefab);
            if(Util.Valid(prefab.GetFlyweightId()))
            {
                mBatchTable[prefab.GetFlyweightId()] = true;
            }
        }

        public bool IsBatchLoading()
        {
            return mBatchStage == BATCH_STAGE_QUEUE;
        }

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