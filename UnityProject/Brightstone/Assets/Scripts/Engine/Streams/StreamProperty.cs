namespace Brightstone
{
    public class StreamProperty
    {
        private string mName = string.Empty;
        private int mNameID = 0;
        private string mValueString = string.Empty;
        private StreamPropertyType mType = StreamPropertyType.SPT_INT32;
        private bool mArray = false;
        private bool mUseHex = false;

        // values
        private int mIntValue = 0;
        private long mLongValue = 0;
        private float mFloatValue = 0.0f;
        

        public void ClearValue()
        {
            mIntValue = 0;
            mLongValue = 0;
            mFloatValue = 0.0f;
            mValueString = string.Empty;
        }

        public string GetName() { return mName; }
        public void SetName(string name) { mName = name; }
        public int GetNameID() { return mNameID; }
        public void SetNameID(int name) { mNameID = name; }
        public StreamPropertyType GetPropertyType() { return mType; }
        public bool IsArray() { return mArray; }
        public void SetArray(bool value) { mArray = value; }
        public bool GetUseHex() { return mUseHex; }
        public void SetUseHex(bool value) { mUseHex = value; }

        public int GetIntValue() { return mIntValue; }
        public long GetLongValue() { return mLongValue; }
        public float GetFloatValue() { return mFloatValue; }
        public string GetStringValue() { return mValueString; }

        public void SetValue(int value)
        {
            ClearValue();
            mType = StreamPropertyType.SPT_INT32;
            if(mUseHex)
            {
                mValueString = Util.GetHexString(value);
            }
            else
            {
                mValueString = value.ToString();
            }
            mIntValue = value;
        }
        public void SetValue(long value)
        {
            ClearValue();
            mType = StreamPropertyType.SPT_INT64;
            if (mUseHex)
            {
                mValueString = Util.GetHexString(value);
            }
            else
            {
                mValueString = value.ToString();
            }
            mLongValue = value;
        }
        public void SetValue(float value)
        {
            ClearValue();
            mType = StreamPropertyType.SPT_FLOAT;
            if (mUseHex)
            {
                mValueString = Util.GetHexString(value);
            }
            else
            {
                mValueString = value.ToString();
            }
            mFloatValue = value;
        }
        public void SetValue(string value)
        {
            ClearValue();
            mType = StreamPropertyType.SPT_STRING;
            mValueString = value;
        }

        public void ParseValue(string value, StreamPropertyType type)
        {
            ClearValue();
            mType = type;
            mValueString = value;
            switch (type)
            {
                case StreamPropertyType.SPT_INT32:
                    if(mUseHex)
                    {
                        mIntValue = Util.GetHex32(value);
                    }
                    else
                    {
                        int.TryParse(value, out mIntValue);
                    }
                    break;
                case StreamPropertyType.SPT_INT64:
                    if(mUseHex)
                    {
                        mLongValue = Util.GetHex64(value);
                    }
                    else
                    {
                        long.TryParse(value, out mLongValue);
                    }
                    break;
                case StreamPropertyType.SPT_FLOAT:
                    if(mUseHex)
                    {
                        mFloatValue = Util.GetHexFloat(value);
                    }
                    else
                    {
                        float.TryParse(value, out mFloatValue);
                    }
                    break;
            }
        }

        
    }
}
