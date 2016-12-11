using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class UIActionBar : UIBase
    {
        [PrefabView(typeof(UIActionButton))]
        [SerializeField]
        private Prefab mButtonType = null;
        [SerializeField]
        private int mNumButtonsX = 4;
        [SerializeField]
        private int mNumButtonsY = 4;
        [SerializeField]
        private float mEdgeSpace = 4.0f;
        [SerializeField]
        private float mGapSpace = 8.0f;
        [SerializeField]
        private float mButtonSize = 64.0f;
        [SerializeField]
        private UIActionButton[] mButtons = null;


        private void Start()
        {
            InternalInit();
        }

        private void OnDestroy()
        {
            InternalDestroy();
        }

        protected override void OnInit()
        {
            base.OnInit();
            QueueBatchLoad();
            mButtons = new UIActionButton[mNumButtonsX * mNumButtonsY];
        }

        public override void OnBatchSubmit()
        {
            base.OnBatchSubmit();
            mButtonType.Prepare();
        }

        public override void OnBatchComplete()
        {
            base.OnBatchComplete();
            // Place Buttons in a grid like fashion.
            Vector2 pivot = new Vector2(0.0f, 1.0f);
            Vector2 anchorMin = new Vector2(0.0f, 1.0f);
            Vector2 anchorMax = new Vector2(0.0f, 1.0f);
            Vector2 size = new Vector2(mButtonSize, mButtonSize);
            for (int y = 0, i = 0; y < mNumButtonsY; ++y)
            {
                for (int x = 0; x < mNumButtonsX; ++x, ++i)
                {
                    UIActionButton button = mWorld.Create<UIActionButton>(mButtonType);
                    RectTransform xform = button.GetTransform() as RectTransform;
                    xform.SetParent(GetTransform());
                    xform.localScale = Vector3.one;
                    xform.anchorMin = anchorMin;
                    xform.anchorMax = anchorMax;
                    xform.pivot = pivot;
                    xform.sizeDelta = size;
                    // edgeSpace + ((x * size) + (x * gapSpace))
                    // edgeSpace + ((y * size) + (y * gapSpace))
                    Vector2 pos = new Vector2(
                        mEdgeSpace + ((x * mButtonSize) + (x * mGapSpace)),
                        mEdgeSpace + ((y * mButtonSize) + (y * mGapSpace))
                        );
                    pos.y = pos.y * -1.0f;
                    xform.anchoredPosition = pos;
                    mButtons[i] = button;
                }
            }
        }

        public override UIElement GetElementType()
        {
            return UIElement.UE_ACTION_BAR;
        }

        public bool IsButtonFree(int index)
        {
            return index >= 0 && index < mButtons.Length && mButtons[index].GetCurrentDescription() == mButtons[index].GetDefaultDescription();
        }

        public void SetupButton(int index, ActionButtonDesc desc, UIActionHandler handler)
        {
            if (index >= 0 && index < mButtons.Length)
            {
                mButtons[index].SetActionHandler(handler);
                mButtons[index].Setup(desc);
            }
        }


        public int GetFreeButtonIndex()
        {
            for(int i =0; i < mButtons.Length; ++i)
            {
                if(IsButtonFree(i))
                {
                    return i;
                }
            }
            return Util.INVALID_INT;
        }

        public void Clear()
        {
            for(int i = 0; i < mButtons.Length; ++i)
            {
                mButtons[i].Setup(null);
            }
        }
    }

}