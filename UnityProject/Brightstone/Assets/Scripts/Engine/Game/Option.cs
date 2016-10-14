using UnityEngine;

namespace Brightstone
{
    public class Option : BaseObject
    {
        private OptionType mOptionType = OptionType.OT_BOOL;
        private OptionName mOptionName = OptionName.ON_NONE;
        private float mValue = 0.0f;
        private float mMin = 0.0f;
        private float mMax = 0.0f;
        private EnumDescriptor[] mEnumDescriptors = null;

        public override void Serialize(BaseStream stream)
        {
            base.Serialize(stream);

            
            if(stream.IsReading())
            {
                string otString = string.Empty;
                stream.SerializeString("OptionType", ref otString);
                mOptionType = Util.GetEnumValueFromString<OptionType>(otString);
            }
            else
            {
                string otString = mOptionType.ToString();
                stream.SerializeString("OptionType", ref otString);
            }
            stream.SerializeString("Name", ref mName);

            switch(mOptionType)
            {
                case OptionType.OT_BOOL:
                    {
                        int nameID = (int)mOptionName;
                        int value = mValue == 0.0f ? 0 : 1;
                        stream.SerializeInt("OptionName", ref nameID);
                        stream.SerializeInt("Value", ref value);
                        mMin = mMax = 0.0f;
                        mEnumDescriptors = null;
                        mOptionName = (OptionName)nameID;
                        mValue = value;
                    }
                    break;
                case OptionType.OT_FLOAT:
                    {
                        int nameID = (int)mOptionName;
                        stream.SerializeInt("OptionName", ref nameID);
                        stream.SerializeFloat("Value", ref mValue);
                        stream.SerializeFloat("Min", ref mMin);
                        stream.SerializeFloat("Max", ref mMax);
                        mEnumDescriptors = null;
                        mOptionName = (OptionName)nameID;
                    }
                    break;
                case OptionType.OT_INT:
                    {
                        int nameID = (int)mOptionName;
                        int value = Mathf.RoundToInt(mValue);
                        int min = Mathf.RoundToInt(mMin);
                        int max = Mathf.RoundToInt(mMax);
                        stream.SerializeInt("OptionName", ref nameID);
                        stream.SerializeInt("Value", ref value);
                        stream.SerializeInt("Min", ref min);
                        stream.SerializeInt("Max", ref max);
                        mOptionName = (OptionName)nameID;
                        mValue = value;
                        mMin = min;
                        mMax = max;
                        mEnumDescriptors = null;
                    }
                    break;
                case OptionType.OT_ENUM:
                    {
                        int nameID = (int)mOptionName;
                        int value = Mathf.RoundToInt(mValue);
                        int numEnums = mEnumDescriptors != null ? mEnumDescriptors.Length : 0;
                        stream.SerializeInt("OptionName", ref nameID);
                        stream.SerializeInt("Value", ref value);
                        stream.SerializeInt("NumEnums", ref numEnums);
                        if(stream.IsReading())
                        {
                            mEnumDescriptors = new EnumDescriptor[numEnums];
                        }
                        for (int i = 0; i < numEnums; ++i)
                        {
                            string tagName = "EN" + i.ToString();
                            string tagId = "EI" + i.ToString();

                            EnumDescriptor descriptor = stream.IsReading() ? new EnumDescriptor() : mEnumDescriptors[i];
                            stream.SerializeString(tagName, ref descriptor.name);
                            stream.SerializeInt(tagId, ref descriptor.id);
                            mEnumDescriptors[i] = descriptor;
                        }
                        mOptionName = (OptionName)nameID;
                        mValue = value;
                        mMin = mMax = 0.0f;
                    }
                    break;
            }
        }

        public OptionName GetOptionName() { return mOptionName; }
        public OptionType GetOptionType() { return mOptionType; }
        public float GetRawValue() { return mValue; }
        public float GetMin() { return mMin; }
        public float GetMax() { return mMax; }
        public EnumDescriptor[] GetEnumDescriptors() { return mEnumDescriptors; }

        public void LoadFrom(Option other)
        {
            mName = other.mName;
            mOptionType = other.mOptionType;
            mOptionName = other.mOptionName;
            mValue = other.mValue;
            mMin = other.mMin;
            mMax = other.mMax;
            mEnumDescriptors = other.mEnumDescriptors;
        }

        public void InitInt(OptionName name, string prettyName, int defaultValue, int min, int max)
        {
            mOptionType = OptionType.OT_INT;
            mOptionName = name;
            mName = prettyName;
            if (min > max)
            {
                int t = min;
                min = max;
                max = t;
            }
            mMin = min;
            mMax = max;
            mValue = defaultValue;
            if (mMin != 0.0f || mMax != 0.0f)
            {
                mValue = Mathf.Clamp(mValue, mMin, mMax);
            }
        }

        public void SetIntValue(int value)
        {
            if (mOptionType == OptionType.OT_INT)
            {
                if (mMin != 0.0f || mMax != 0.0f)
                {
                    mValue = Mathf.Clamp(value, mMin, mMax);
                }
                else
                {
                    mValue = value;
                }
            }
            else
            {
                Log.Game.Error("Option.SetIntValue failed because this option is not a Int! Name=" + mName);
            }
        }

        public int GetIntValue()
        {
            if(mOptionType == OptionType.OT_INT)
            {
                return (int)mValue;
            }
            return 0;
        }
        

        public void InitFloat(OptionName name, string prettyName, float defaultValue, float min, float max)
        {
            mOptionType = OptionType.OT_FLOAT;
            mOptionName = name;
            mName = prettyName;
            if (min > max)
            {
                float t = min;
                min = max;
                max = t;
            }
            mMin = min;
            mMax = max;
            mValue = defaultValue;
            if (mMin != 0.0f || mMax != 0.0f)
            {
                mValue = Mathf.Clamp(mValue, mMin, mMax);
            }
        }

        public void SetFloatValue(float value)
        {
            if (mOptionType == OptionType.OT_FLOAT)
            {
                if (mMin != 0.0f || mMax != 0.0f)
                {
                    mValue = Mathf.Clamp(value, mMin, mMax);
                }
                else
                {
                    mValue = value;
                }
            }
            else
            {
                Log.Game.Error("Option.SetFloatValue failed because this option is not a Float! Name=" + mName);
            }
        }

        public float GetFloatValue()
        {
            if (mOptionType == OptionType.OT_FLOAT)
            {
                return mValue;
            }
            return 0.0f;
        }

        public void InitBool(OptionName name, string prettyName, bool defaultValue)
        {
            mOptionType = OptionType.OT_BOOL;
            mOptionName = name;
            mName = prettyName;
            mValue = defaultValue ? 1.0f : 0.0f;
            mMin = 0.0f;
            mMax = 0.0f;
        }

        public void SetBoolValue(bool value)
        {
            if(mOptionType == OptionType.OT_BOOL)
            {
                mValue = value ? 1.0f : 0.0f;
            }
            else
            {
                Log.Game.Error("Option.SetBoolValue failed because this option is not a Bool! Name=" + mName);
            }
        }

        public bool GetBoolValue()
        {
            if(mOptionType == OptionType.OT_BOOL)
            {
                return mValue == 0.0f ? false : true;
            }
            return false;
        }

        public void InitEnum(OptionName name, string prettyName, EnumDescriptor[] descriptors)
        {
            mOptionType = OptionType.OT_ENUM;
            mOptionName = name;
            mName = prettyName;
            mValue = descriptors[0].id;
            mMin = 0.0f;
            mMax = 0.0f;
            mEnumDescriptors = descriptors;
        }

        public void SetEnumValue(int id)
        {
            if(mOptionType != OptionType.OT_ENUM )
            {
                Log.Game.Error("Option.SetEnumValue failed because this option is not a Enum! Name=" + mName);
                return;
            }
            if(mEnumDescriptors == null)
            {
                Log.Game.Error("Option.SetEnumValue failed because this option is enum is not initialized! Name=" + mName);
                return;
            }

            for(int i = 0; i < mEnumDescriptors.Length; ++i)
            {
                if(mEnumDescriptors[i].id == id)
                {
                    mValue = id;
                    return;
                }
            }
            Log.Game.Error("Option.SetEnumValue failed because enum ID does not exist! Name=" + mName + ", Arg=" + id.ToString());
        }

        public void SetEnumValue(string name)
        {
            if (mOptionType != OptionType.OT_ENUM)
            {
                Log.Game.Error("Option.SetEnumValue failed because this option is not a Enum! Name=" + mName);
                return;
            }
            if (mEnumDescriptors == null)
            {
                Log.Game.Error("Option.SetEnumValue failed because this option is enum is not initialized! Name=" + mName);
                return;
            }

            for (int i = 0; i < mEnumDescriptors.Length; ++i)
            {
                if (mEnumDescriptors[i].name == name)
                {
                    mValue = mEnumDescriptors[i].id;
                    return;
                }
            }
            Log.Game.Error("Option.SetEnumValue failed because enum Name does not exist! Name=" + mName + ", Arg=" + name);
        }

        public int GetEnumValue()
        {
            if (mOptionType != OptionType.OT_ENUM || mEnumDescriptors == null)
            {
                return 0;
            }
            return Mathf.RoundToInt(mValue);
        }

        public string GetEnumString(int id)
        {
            if (mOptionType != OptionType.OT_ENUM || mEnumDescriptors == null)
            {
                return string.Empty;
            }
            for(int i = 0; i < mEnumDescriptors.Length; ++i)
            {
                if(mEnumDescriptors[i].id == id)
                {
                    return mEnumDescriptors[i].name;
                }
            }
            return string.Empty;
        }
    }
}

