using UnityEngine;

namespace Brightstone
{
    /**
    * Custom axis used to check on Input Events and translate them into Input Codes.
    */
	public class InputHandler 
	{
        private float mCurrentValue = 0.0f;
        private float mLastValue = 0.0f;
        private KeyCode mPrimaryKey = KeyCode.None;
        private KeyCode mSecondaryKey = KeyCode.None;
        private InputCode mInputCode = InputCode.IC_NONE;

        /** If true, ONLY the Primary Key is used and values are clamped from 0 to 1. */
        private InputHandlerType mHandlerType = InputHandlerType.IHT_BUTTON;
        /** Used for Non-Buttons, modifies the rate at which value is modified.*/
        private float mDeltaModifier = 1.0f;
        /** Used for Non-Buttons, checks */
        private bool mSnapToZero = false;

        public float GetCurrentValue() { return mCurrentValue; }
        public float GetLastValue() { return mLastValue; }
        public KeyCode GetPrimaryKey() { return mPrimaryKey; }
        public KeyCode GetSecondaryKey() { return mSecondaryKey; }
        public InputCode GetInputCode() { return mInputCode; }
        public InputHandlerType GetInputHandlerType() { return mHandlerType; }
        public float GetDeltaModifier() { return mDeltaModifier; }
        public bool GetSnapToZero() { return mSnapToZero; }

        public void SetPrimaryKey(KeyCode keyCode) { mPrimaryKey = keyCode; }
        public void SetSecondaryKey(KeyCode keyCode) { mSecondaryKey = keyCode; }
        public void SetInputCode(InputCode code) { mInputCode = code; }
        public void SetInputHandlerType(InputHandlerType type) { mHandlerType = type; }
        public void SetDeltaModifier(float value) { mDeltaModifier = value; }
        public void SetSnapToZero(bool value) { mSnapToZero = value; }

        
        public void Update(InputMgr inputMgr, float delta)
        {
            switch(mHandlerType)
            {
                case InputHandlerType.IHT_BUTTON: UpdateButton(inputMgr, delta); break;
                case InputHandlerType.IHT_AXIS:
                case InputHandlerType.IHT_DUAL_AXIS: UpdateAxis(inputMgr, delta); break;
            }
        }

        private void UpdateButton(InputMgr inputMgr, float delta)
        {
            if(mPrimaryKey == KeyCode.None)
            {
                return;
            }
            mLastValue = mCurrentValue;
            if(inputMgr.NativeGetKeyState(mPrimaryKey))
            {
                mCurrentValue = 1.0f;
            }
            else
            {
                mCurrentValue = 0.0f;
            }
        }

        private void UpdateAxis(InputMgr inputMgr, float delta)
        {
            mLastValue = mCurrentValue;

            // Update as single axis
            if (mSecondaryKey == KeyCode.None) 
            {
                bool isDown = inputMgr.NativeGetKeyState(mPrimaryKey);
                if(isDown)
                {
                    mCurrentValue = Mathf.Min(1.0f, mCurrentValue + delta * mDeltaModifier);
                }
                else
                {
                    if(mSnapToZero)
                    {
                        mCurrentValue = 0.0f;
                    }
                    else
                    {
                        mCurrentValue = Mathf.Max(0.0f, mCurrentValue - delta * mDeltaModifier);
                    }
                }
            }
            else // Update as dual axis
            {
                bool pDown = inputMgr.NativeGetKeyState(mPrimaryKey);
                bool sDown = inputMgr.NativeGetKeyState(mSecondaryKey);
                if(pDown && sDown)
                {
                    return;
                }
                if(pDown)
                {
                    mCurrentValue = Mathf.Min(1.0f, mCurrentValue + delta * mDeltaModifier);
                }
                else if(sDown)
                {
                    mCurrentValue = Mathf.Max(-1.0f, mCurrentValue - delta * mDeltaModifier);
                }
                else
                {
                    if(mSnapToZero)
                    {
                        mCurrentValue = 0.0f;
                    }
                    else if(mCurrentValue > 0.0f)
                    {
                        mCurrentValue = Mathf.Max(0.0f, mCurrentValue - delta * mDeltaModifier);
                    }
                    else if(mCurrentValue < 0.0f)
                    {
                        mCurrentValue = Mathf.Min(0.0f, mCurrentValue + delta * mDeltaModifier);
                    }
                }
            }
        }
	}
}