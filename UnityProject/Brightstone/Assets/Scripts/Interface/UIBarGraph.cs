using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Brightstone
{
	public class UIBarGraph : BaseComponent 
	{
        public class UIBar
        {
            // private float mWidth = 0.0f;
            private float mHeight = 0.0f;
            private RectTransform mTransform = null;
            private Text mText = null;
            private RectTransform mTextTransform = null;
            private float mValue = 0.0f;
            private string mName = string.Empty;

            public void Apply(float width, float height)
            {
                // mWidth = width;
                mHeight = height;
                if (mTransform != null)
                {
                    mTransform.sizeDelta = new Vector2(width, height);
                }
            }

            public void SetComponent(RectTransform transform, Text text)
            {
                mTransform = transform;
                mText = text;
                mTextTransform = mText.GetComponent<RectTransform>();
            }

            public void SetPosition(float x, float y)
            {
                mTransform.anchoredPosition = new Vector2(x, y);
                mTextTransform.anchoredPosition = new Vector2(x - 150.0f, y + mHeight * 0.25f);
            }

            public void SetText(string text)
            {
                mText.text = mName + text;
            }
            public void SetName(string name)
            {
                mName = name;
            }

            public float GetValue()
            {
                return mValue;
            }
            public void SetValue(float value)
            {
                mValue = value;
            }
        }

        [SerializeField]
        private float mGraphWidth = 400;
        [SerializeField]
        private float mGraphHeight = 400;

        private const float TEXT_WIDTH = 300.0f;
        private const float TEXT_HEIGHT = 40.0f;
        [SerializeField]
        private GameObject mBarPrefab = null;
        [SerializeField]
        private GameObject mTextPrefab = null;
        [SerializeField]
        private bool mShowPercent = false;

        private List<UIBar> mBars = new List<UIBar>();
        private LinearColorPallet mColorPallet = new LinearColorPallet();

        // Random Color...
        public struct ByteColor
        {
            public float r;
            public float g;
            public float b;

            public ByteColor(float red, float green, float blue)
            {
                r = red;
                g = green;
                b = blue;
            }

            public void Mul(float amount)
            {
                r *= amount;
                g *= amount;
                b *= amount;
            }
        }

        public class LinearColorPallet
        {
            private ByteColor[] mCombinations = new ByteColor[7];
            private int mLevel = 0;
            private int mStepLevel = 0;
            private int mResetLevel = 7;
            private float mStart = 0.0f;

            public void Init(float start, int reset)
            {
                mStart = start;
                mLevel = 0;
                mStepLevel = 0;
                mResetLevel = reset;
                //x - -
                //- x -
                //- - x
                //x x - 
                //- x x
                //x - x
                //x x x
                mCombinations[0] = new ByteColor(255.0f * start, 0.0f, 0.0f);
                mCombinations[1] = new ByteColor(0.0f, 255.0f * start, 0.0f);
                mCombinations[2] = new ByteColor(0.0f, 0.0f, 255.0f * start);
                mCombinations[3] = new ByteColor(255.0f * start, 255.0f * start, 0.0f);
                mCombinations[4] = new ByteColor(0.0f, 255.0f * start, 255.0f * start);
                mCombinations[5] = new ByteColor(255.0f * start, 0.0f, 255.0f * start);
                mCombinations[6] = new ByteColor(255.0f * start, 255.0f * start, 255.0f * start);
            }

            public void Next()
            {
                ++mLevel;
                if(mLevel == mCombinations.Length)
                {
                    mLevel = 0;
                    ++mStepLevel;
                    if(mStepLevel == mResetLevel)
                    {
                        Init(mStart, mResetLevel);
                    }
                    else
                    {
                        for (int i = 0; i < mCombinations.Length; ++i)
                        {
                            mCombinations[i].Mul(0.5f);
                        }
                    }
                }
            }

            public Color GetColor()
            {
                ByteColor red = mCombinations[mLevel];
                ByteColor green = mCombinations[mLevel];
                ByteColor blue = mCombinations[mLevel];
                red.r = red.r * Random.Range(0.3465f, 0.6533f);
                green.g = green.g * Random.Range(0.2265f, 0.6533f);
                blue.b = blue.b * Random.Range(0.4265f, 0.8833f);
                Next();
                return new Color(red.r / 255.0f, green.g / 255.0f, blue.b / 255.0f, 255.0f);
            }
        }
        
        
        private UIBar CreateBar(float value, string name)
        {
            GameObject instance = Instantiate<GameObject>(mBarPrefab);
            UIBar bar = new UIBar();
            RectTransform xform = instance.GetComponent<RectTransform>();
            GameObject textInstance = Instantiate<GameObject>(mTextPrefab);
            bar.SetComponent(xform,textInstance.GetComponent<Text>());
            bar.SetValue(value);
            bar.SetName(name);
            bar.SetText(" (" + value.ToString("n3") + ") ");
            xform.SetParent(transform, false);
            xform.localScale = Vector3.one;
            textInstance.GetComponent<RectTransform>().SetParent(transform, false);
            textInstance.GetComponent<RectTransform>().localScale = Vector3.one * 0.5f;
            Image image = instance.GetComponent<Image>();
            image.color = mColorPallet.GetColor();

            return bar;
        }

		private void Start()
        {
            InternalInit();
        }
        private void OnDestroy()
        {
            InternalDestroy();
        }
        private int SortBar(UIBar a, UIBar b)
        {
            if(a.GetValue() < b.GetValue())
            {
                return 1;
            }
            else if(a.GetValue() == b.GetValue())
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        protected override void OnInit()
        {
            mColorPallet.Init(0.75f,7);
            for(int i = 0; i < 15; ++i)
            {
                mBars.Add(CreateBar(Random.Range(10.0f,600.0f), "Bar_" + i.ToString()));
            }
            
        }

        float mElapsed = 0.0f;

        private void Update()
        {
            if(mBars.Count == 0)
            {
                return;
            }
            mElapsed += Time.deltaTime;
            if(mElapsed > 0.05f)
            {
                for (int i = 0; i < mBars.Count; ++i)
                {
                    mBars[i].SetValue(mBars[i].GetValue() + Random.Range(10.0f,35.0f));
                }
                mElapsed = 0.0f;
            }
            

            mBars.Sort(SortBar);
            float maxValue = mBars[0].GetValue();
            float minValue = mBars[mBars.Count - 1].GetValue();
            float height = mGraphHeight / mBars.Count;
            float yPos = 0.0f;
            for (int i = 0; i < mBars.Count; ++i)
            {
                float percent = Mathf.InverseLerp(minValue, maxValue, mBars[i].GetValue());
                float size = mGraphWidth * percent;
                mBars[i].Apply(size, height);
                mBars[i].SetPosition(0.0f, yPos);
                if(mShowPercent)
                {
                    float roundedPercent = Mathf.Floor(percent * 100.0f);
                    mBars[i].SetText(" (" + roundedPercent.ToString("n0") + "%)");
                }
                else
                {
                    mBars[i].SetText(" (" + mBars[i].GetValue() + ")");
                }

                yPos += height;
            }
        }

        
    }
}