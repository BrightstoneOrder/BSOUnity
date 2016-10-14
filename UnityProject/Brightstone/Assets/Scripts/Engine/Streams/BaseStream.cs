using System.Collections.Generic;
using System;

namespace Brightstone
{
    /**
    * -- Stream Design -- 
    * 
    * -- TextStream
    * TODO: Support inheritence? Maybe? 
    * -- BinaryStream
    * -- 
    */
    public class BaseStream
    {

        private bool mReading = false;
        protected List<StreamContext> mStreamContexts = new List<StreamContext>();
        protected int mActiveContext = -1;
        private int mAnonIndex = 0;
        private StreamErrorBitfield mErrors = new StreamErrorBitfield();

        public void StartContext(string type, string parentType)
        {
            StopContext();
            StreamContext context = new StreamContext();
            context.Init(type, parentType);
            mStreamContexts.Add(context);
            mActiveContext = mStreamContexts.Count - 1;
        }
        public void NextContext()
        {
            int before = mActiveContext;
            StopContext();
            mActiveContext = before + 1;
        }
        public void StopContext()
        {
            mActiveContext = -1;
        }

        public virtual void SerializeInt(ref int n)
        {
            
        }
        public virtual void SerializeInt64(ref long n)
        {

        }
        public virtual void SerializeFloat(ref float n)
        {

        }
        public virtual void SerializeString(ref string s)
        {

        }
        public virtual void SerializeObject(BaseObject o)
        {

        }
        public virtual void SerializeObject(BaseComponent o)
        {

        }
        public virtual void SerializeInt(string name, ref int n)
        {

        }
        public virtual void SerializeInt64(string name, ref long n)
        {

        }
        public virtual void SerializeFloat(string name, ref float n)
        {

        }
        public virtual void SerializeString(string name, ref string s)
        {

        }
        public virtual void SerializeObject(string name, BaseObject o)
        {

        }
        public virtual void SerializeObject(string name, BaseComponent o)
        {

        }
        
        // Array

        public virtual void Serialize(string name, ref List<int> collection)
        {

        }
        public virtual void Serialize(string name, ref List<long> collection)
        {

        }
        public virtual void Serialize(string name, ref List<float> collection)
        {

        }
        public virtual void Serialize(string name, ref List<string> collection)
        {

        }
        public virtual void Serialize<T>(string name, ref List<T> collection) where T : BaseObject
        {

        }

        public virtual string WriteText()
        {
            return string.Empty;
        }

        public virtual void ParseText(string text)
        {
            
        }

        protected void ProcessName(ref string name)
        {
            
        }

        protected void ReportError(StreamError error, string message)
        {
            if((!mErrors.Has(error) && error == StreamError.SE_INVALID_CONTEXT)
                || (!mErrors.Has(error) && error != StreamError.SE_INVALID_CONTEXT))
            {
                Log.Lib.Error(error.ToString() + " occured in stream. " + message);
            }
            mErrors.Set(error);
        }

        protected string GetAnonymousName()
        {
            string name = "AN" + mAnonIndex.ToString();
            ++mAnonIndex;
            return name;
        }

        public StreamContext GetActiveContext()
        {
            return (mActiveContext < mStreamContexts.Count && mActiveContext >= 0) ? mStreamContexts[mActiveContext] : null;
        }

        public bool IsReading() { return mReading; }
        public void SetReadingMode(bool reading) { mReading = reading; }

 
    }

    /**

    Types
    StreamTag
    - string Name
    - string Value
    - int valueType

    Internal Types
    StreamContext -- Current Serialized Type
    StreamObject -- An object containing a bunch of properties.
    StreamProperty -- A key/value pair, properties can contain other properties

    StreamContext
    {
        StreamObject
        {
            StreamObject
            Property
        }
    }

    Functions
    - SerializeInt(int n)
    - SerializeInt64(long n)
    - SerializeString(string s)
    - SerializeObject(BaseObject o)
    - SerializeObject(BaseComponent o)
    - SerializeTag(StreamTag t)
    - PushScope
    - PopScope
    - StartType
    - EndType

    -- Text Stream
    $Object[A,B]
    {
        "Name":"Kris"
        "Level":Number32(17)
        "Money":Number64(320020)
        "MainHand":{
            "Name":"ThunderWorry"
            "LevelReq":Number32(32)
        }
        "Items":{
            "Name":"Shield"
            "LevelReq":Number32(42)
        },
        {
            "Name":"Helm"
            "LevelReq":Number32(82)
        }
    }

    // Transport
    s.SerializeString("Name", ref mName); // 
    ProcessNameText(ref name);
    if(reading)
    {
        var property = GetTop().FindProperty(name);
        if(property != null)
        {
            if(!property.IsString())
            {
                AppendStreamingError(SE_INVALID_PROPERTY_TYPE, name);
                return;
            }
            value = property.GetValueString();
        }
    }
    else
    {
        var property = GetTop().CreateProperty(name);
        if(property != null)
        {
            property.SetValue(value);
        }
    }
    if(reading)
    {
        var property = GetTop().FindPropertyByIndex(GetTop().GetPropertyIndex());
        GetTop().IncrementPropertyIndex();
        if(property != null)
        {
            if(!property.IsString())
            {
                ProcessNameText(ref name);
                AppendStreamingError(SE_INVALID_PROPERTY_TYPE, name);
                return;
            }
            value = property.GetValueString();
        }        
    }
    else
    {
        var property = GetTop().CreateProperty(name);
        GetTop().IncrementPropertyIndex();
        if(property != null)
        {
            property.SetValue(value);
        }
    }
    
    -- Binary Stream
    where 0 = Object, 1 = String, 2 = Number32, 3 = Number64

    $Object[A,B]
    {
        0=Kris
        1=32
        2=320020
        
    }


    */
}
