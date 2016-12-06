using UnityEngine;
using UnityEngine.UI;

namespace Brightstone
{
    // Prefab Setup:
    // GameObject
    //  -> Image (Image)
    //  -> Button (Button) 
    //  -> UIActionBar
    //  > Child GameObject
    //   -> Overlay(Image) -- Optional
    public class UIActionButton : UIBase
    {

        [SerializeField]
        private UITooltip mTooltip = null;
        [SerializeField]
        private ActionButtonDesc mDefaultDesc = new ActionButtonDesc();

        private ActionButtonDesc mCurrentDesc = null;
        private Button mButton = null;
        private Image mImage = null;
        private Image mOverlay = null;

        void Start()
        {
            InternalInit();
        }
        
        void OnDestroy()
        {
            InternalDestroy();
        }

        protected override void OnInit()
        {
            base.OnInit();
            mButton = GetComponent<Button>();
            mImage = GetComponent<Image>();
            mOverlay = GetComponentInChildren<Image>();

            Setup(mDefaultDesc);
        }

        // Set null to return to default.
        public void Setup(ActionButtonDesc desc)
        {
            if(desc == null)
            {
                desc = mDefaultDesc;
            }
            mCurrentDesc = desc;

            SpriteState ss = new SpriteState();
            ss.disabledSprite = desc.disabled;
            ss.highlightedSprite = desc.highlight;
            ss.pressedSprite = desc.pressed;
            mButton.spriteState = ss;
            mImage.sprite = desc.image;

            // Overlay is optional:
            if(mOverlay != null)
            {
                mOverlay.color = desc.overlayColor;
            }
            // Tooltip is optional:
            if(mTooltip != null)
            {
                mTooltip.SetTooltip(desc.tooltip);
                mTooltip.SetDescription(desc.description);
            }
        }

        public ActionButtonDesc GetCurrentDescription()
        {
            return mCurrentDesc;
        }

        public ActionButtonDesc GetDefaultDescription()
        {
            return mDefaultDesc;
        }


        public void OnClick()
        {
            // TODO: Signal UIMgr?
        }

        public void OnEnter()
        {
            if(mTooltip != null)
            {
                mTooltip.Show();
            }
        }

        public void OnExit()
        {
            if(mTooltip != null)
            {
                mTooltip.Hide();
            }
        }
    }


}

