using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Brightstone
{
    // Prefab Setup:
    // GameObject
    //  -> Image (Image)
    //  -> Button (Button) 
    //  -> UIActionBar
    //  > Child GameObject
    //   -> Overlay(Image) -- Optional
    //
    // --
    // Each button has their own keybinding / hotkey.. Can be overridden in code.
    public class UIActionButton : UIBase
    {
        [SerializeField]
        private UITooltip mTooltip = null;
        [SerializeField]
        private ActionButtonDesc mDefaultDesc = new ActionButtonDesc();
        [SerializeField]
        private InputCode mHotKey = InputCode.IC_NONE;

        private ActionButtonDesc mCurrentDesc = null;
        private Button mButton = null;
        private Image mImage = null;
        private Image mOverlay = null;
        private UIActionHandler mActionHandler = null;
        

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

            InputCode hotKey = mHotKey;
            mHotKey = InputCode.IC_NONE;
            SetHotKey(hotKey);
        }

        public override UIElement GetElementType()
        {
            return UIElement.UE_ACTION_BUTTON;
        }

        /// <summary>
        /// Set to numm to reset to null.
        /// </summary>
        public void Setup(ActionButtonDesc desc)
        {
            if(desc == null)
            {
                desc = mDefaultDesc;
                mActionHandler = null;
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

        public void SetHotKey(InputCode hotKey)
        {
            if(mHotKey != hotKey)
            {
                InputCode old = mHotKey;
                mHotKey = hotKey;
                // Unregister -- if old != IC_NONE
                if(old != InputCode.IC_NONE)
                {
                    mWorld.GetInputMgr().UnregisterCallback(OnInput, old);
                }
                // Register -- if new != IC_NONE
                if(mHotKey != InputCode.IC_NONE)
                {
                    mWorld.GetInputMgr().RegisterCallback(OnInput, mHotKey);
                }
            }
            
        }

        private void OnInput(ref InputEventData data)
        {
            if(data.release)
            {
                if(mActionHandler != null)
                {
                    mActionHandler.OnAction(UIAction.UA_CLICK, this);
                }
            }
        }

        public InputCode GetHotKey()
        {
            return mHotKey;
        }

        public ActionButtonDesc GetCurrentDescription()
        {
            return mCurrentDesc;
        }

        public ActionButtonDesc GetDefaultDescription()
        {
            return mDefaultDesc;
        }

        public void SetActionHandler(UIActionHandler handler)
        {
            mActionHandler = handler;
        }

        public UIActionHandler GetActionHandler()
        {
            return mActionHandler;
        }

        // Called via Unity Button Events. @See GameObject setup.
        public void OnClick()
        {
            if(mActionHandler != null)
            {
                mActionHandler.OnAction(UIAction.UA_CLICK, this);
            }
            mWorld.GetUIMgr().GlobalNotify(UIAction.UA_CLICK, this);
        }

        // Called via Unity UI Events. @See GameObject setup. 
        public void OnEnter()
        {
            if(mTooltip != null)
            {
                mTooltip.Show();
            }
            if (mActionHandler != null)
            {
                mActionHandler.OnAction(UIAction.UA_MOUSE_ENTER, this);
            }
            mWorld.GetUIMgr().GlobalNotify(UIAction.UA_MOUSE_ENTER, this);
        }

        // Called via Unity UI Events. @See GameObject setup.
        public void OnExit()
        {
            if(mTooltip != null)
            {
                mTooltip.Hide();
            }
            if (mActionHandler != null)
            {
                mActionHandler.OnAction(UIAction.UA_MOUSE_LEAVE, this);
            }
            mWorld.GetUIMgr().GlobalNotify(UIAction.UA_MOUSE_LEAVE, this);
        }

    }


}

