using UnityEngine;
using System.Collections;

namespace Brightstone
{
    /**
     * Scale & Scroll textures down over time.
     * @mTime we make the effect invislbe...
     * 
     */
    public class ClickEffect : BaseComponent
    {
        
        [SerializeField]
        private float mLoopTime = 3.0f; // debug
        [SerializeField]
        private bool mLoop = false;
        [SerializeField]
        private bool mDestroyAtEnd = true;
        [SerializeField]
        private float mScaleTime = 0.66f;
        [SerializeField]
        private float mOffsetTime = 0.33f;
        [SerializeField]
        private float mTargetScale = 0.0f;

        private MeshRenderer[] mChildRenderers = null;
        private Material[] mChildMaterialInstances = null;
        private float mStartScale = 0.0f;
        private float mElapsedTime = 0.0f;
        private bool mVisible = true;

        // Use this for initialization
        void Start()
        {
            InternalInit();
            mChildRenderers = GetComponentsInChildren<MeshRenderer>();
            if (mChildRenderers != null)
            {
                mChildMaterialInstances = new Material[mChildRenderers.Length];
                for (int i = 0; i < mChildMaterialInstances.Length; ++i)
                {
                    mChildMaterialInstances[i] = mChildRenderers[i].material;
                }
            }
            mStartScale = GetTransform().localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            float dt = World.ActiveWorld.GetGameDelta();
            mElapsedTime += dt;

            float scale = Mathf.Lerp(mStartScale, mTargetScale, Mathf.Clamp01(mElapsedTime / mScaleTime));
            float offset = Mathf.Lerp(1.0f, 0.0f, Mathf.Clamp01(mElapsedTime / mOffsetTime));

            GetTransform().localScale = new Vector3(scale, scale, scale);
            for(int i = 0; i < mChildMaterialInstances.Length; ++i)
            {
                mChildMaterialInstances[i].mainTextureOffset = new Vector2(offset, 0.0f);
            }

            if(mElapsedTime >= mLoopTime)
            {
                if(mLoop)
                {
                    mElapsedTime = 0.0f;
                    SetVisible(true);
                }
            }
            else if(mElapsedTime >= mScaleTime)
            {
                SetVisible(false);
                if (!mLoop && mDestroyAtEnd)
                { 
                    Destroy(gameObject);
                }
            }
        }

        void SetVisible(bool vis)
        {
            if(vis != mVisible)
            {
                if (vis)
                {
                    GetTransform().localScale = new Vector3(mStartScale, mStartScale, mStartScale);
                    for (int i = 0; i < mChildMaterialInstances.Length; ++i)
                    {
                        mChildMaterialInstances[i].mainTextureOffset = new Vector2(1.0f, 0.04f);
                    }
                }

                for (int i = 0; i < mChildRenderers.Length; ++i)
                {
                    mChildRenderers[i].enabled = vis;
                    
                }
                
            }
            mVisible = vis;
        }
    }
}


