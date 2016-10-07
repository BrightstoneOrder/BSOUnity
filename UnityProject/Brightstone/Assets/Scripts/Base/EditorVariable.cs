namespace Brightstone
{
    /** Editor vars are variables of simple data types used to store meta data for the editor in ComponentBase*/
    public class EditorVariable
    {
        /** Name of the var.*/
        private string mName = string.Empty;
        private string mValueString = string.Empty;

        public EditorVariable(string name, int value)
        {
            mName = name;
            mValueString = value.ToString();
        }

        public EditorVariable(string name, float value)
        {
            mName = name;
            mValueString = value.ToString();
        }

        public EditorVariable(string name, string value)
        {
            mName = name;
            mValueString = value;
        }

        public void SetName(string name)
        {
            mName = name;
        }
        public string GetName()
        {
            return mName;
        }

        public void SetValue(int value)
        {
            mValueString = value.ToString();
        }

        public void SetValue(float value)
        {
            mValueString = value.ToString();
        }

        public void SetValue(string value)
        {
            mValueString = value;
        }

        public int GetInt()
        {
            int value = 0;
            int.TryParse(mValueString, out value);
            return value;
        }

        public float GetFloat()
        {
            float value;
            float.TryParse(mValueString, out value);
            return value;
        }

        public string GetString()
        {
            return mValueString;
        }
    }

}