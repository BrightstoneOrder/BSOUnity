using UnityEngine;
using UnityEngine.UI;

namespace Brightstone
{
    // Prefab Setup:
    // GameObject
    //  -> Background (Image) -- Optional
    //  -> UITooltip
    //  > Child GameObject
    //   -> Text (Tooltip)
    //  > Child GameObject
    //   -> Text (Description) 
    public class UITooltip : UIBase
    {
        private Text mTooltipComponent = null;
        private Text mDescriptionComponent = null;

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
            Text[] textComponents = GetComponentsInChildren<Text>();
            for(int i = 0; i < textComponents.Length; ++i)
            {
                if(textComponents[i].name == "Tooltip")
                {
                    mTooltipComponent = textComponents[i];
                }
                else if(textComponents[i].name == "Description")
                {
                    mDescriptionComponent = textComponents[i];
                }
            }
            if(!HasComponents())
            {
                Log.Game.Error("UITooltip does not have valid children! Requires Tooltip && Description with Text");
            }
            Hide();
        }

        public string GetTooltip()
        {
            if(mTooltipComponent != null)
            {
                return mTooltipComponent.text;
            }
            return string.Empty;
        }
        public void SetTooltip(string tooltip)
        {
            if(mTooltipComponent != null)
            {
                mTooltipComponent.text = tooltip;
            }
        }

        public string GetDescription()
        {
            if(mDescriptionComponent != null)
            {
                return mDescriptionComponent.text;
            }
            return string.Empty;
        }

        public void SetDescription(string description)
        {
            if(mDescriptionComponent != null)
            {
                mDescriptionComponent.text = description;
            }
        }

        public override void Show()
        {
            base.Show();
            if(GetHideRequests() == 0 && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public override void Hide()
        {
            base.Hide();
            if(gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        public bool HasComponents()
        {
            return mTooltipComponent != null && mDescriptionComponent != null;
        }
    }

}
