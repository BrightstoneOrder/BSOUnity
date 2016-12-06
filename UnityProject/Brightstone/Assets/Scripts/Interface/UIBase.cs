using UnityEngine;

namespace Brightstone
{
    public class UIBase : Actor
    {
        private int mHideRequests = 0;
        // Show hide functions to show / hide ui graphics.

        protected override void OnInit()
        {
            base.OnInit();
            mWorld.GetUIMgr().Register(this);
        }

        protected override void OnDestroyed()
        {
            mWorld.GetUIMgr().Unregister(this);
            base.OnDestroyed();
        }

        public virtual void Show()
        {
            if(mHideRequests == 0)
            {
                return;
            }
            --mHideRequests;
        }
        public virtual void Hide()
        {
            ++mHideRequests;
        }

        public int GetHideRequests()
        {
            return mHideRequests;
        }
    }
}

