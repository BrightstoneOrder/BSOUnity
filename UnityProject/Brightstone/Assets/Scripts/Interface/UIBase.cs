using UnityEngine;

namespace Brightstone
{
    public class UIBase : Actor
    {
        [SerializeField]
        private string mElementName = string.Empty;

        private int mHideRequests = 0;
        
        
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

        // Show hide functions to show / hide ui graphics.
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

        public void SetHideRequests(int value)
        {
            mHideRequests = Mathf.Max(0, value);
        }

        public int GetHideRequests()
        {
            return mHideRequests;
        }

        public void SetElementName(string name)
        {
            mElementName = name;
        }

        public string GetElementName()
        {
            return mElementName;
        }

        public virtual UIElement GetElementType()
        {
            return UIElement.UE_NONE;
        }
    }
}

