using System.Text;
using System.Collections.Generic;
using System;

namespace Brightstone
{
    public class NumericalStream : TextStream
    {
        //
        protected int mCurrentNameID = 0;

        public override void SerializeInt(string name, ref int n)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamProperty property = top.FindProperty(mCurrentNameID);
                ++mCurrentNameID;
                if (property != null)
                {
                    if (property.GetPropertyType() != StreamPropertyType.SPT_INT32)
                    {
                        ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, name);
                        return;
                    }
                    n = property.GetIntValue();
                }
            }
            else
            {
                // Maybe use anon name..? Name doesn't really even matter though.. Just need unique ID
                StreamProperty property = top.FindProperty(name, true);
                property.SetUseHex(ENCODE_HEX);
                property.SetValue(n);
            }

        }
        public override void SerializeInt64(string name, ref long n)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamProperty property = top.FindProperty(mCurrentNameID);
                ++mCurrentNameID;
                if (property != null)
                {
                    if (property.GetPropertyType() != StreamPropertyType.SPT_INT64)
                    {
                        ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, name);
                        return;
                    }
                    n = property.GetLongValue();
                }
            }
            else
            {
                StreamProperty property = top.FindProperty(name, true);
                property.SetUseHex(ENCODE_HEX);
                property.SetValue(n);
            }
        }
        public override void SerializeFloat(string name, ref float n)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamProperty property = top.FindProperty(mCurrentNameID);
                ++mCurrentNameID;
                if (property != null)
                {
                    if (property.GetPropertyType() != StreamPropertyType.SPT_FLOAT)
                    {
                        ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, name);
                        return;
                    }
                    n = property.GetFloatValue();
                }
            }
            else
            {
                StreamProperty property = top.FindProperty(name, true);
                property.SetUseHex(ENCODE_HEX);
                property.SetValue(n);
            }
        }
        public override void SerializeString(string name, ref string s)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamProperty property = top.FindProperty(mCurrentNameID);
                ++mCurrentNameID;
                if (property != null)
                {
                    if (property.GetPropertyType() != StreamPropertyType.SPT_STRING)
                    {
                        ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, name);
                        return;
                    }
                    s = property.GetStringValue();
                }
            }
            else
            {
                StreamProperty property = top.FindProperty(name, true);
                property.SetUseHex(ENCODE_HEX);
                property.SetValue(s);
            }
        }
        public override void SerializeObject(string name, BaseObject o)
        {
            // Object is a property... but its a child..
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamObject child = top.FindChild(mCurrentNameID);
                ++mCurrentNameID;
                if (child != null)
                {
                    context.Push(child);
                    o.Serialize(this);
                    context.Pop();
                }
            }
            else
            {
                StreamObject child = top.FindChild(name, true);
                context.Push(child);
                o.Serialize(this);
                context.Pop();
            }
        }
        public override void SerializeObject(string name, BaseComponent o)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamObject child = top.FindChild(mCurrentNameID);
                ++mCurrentNameID;
                if (child != null)
                {
                    context.Push(child);
                    o.Serialize(this);
                    context.Pop();
                }
            }
            else
            {
                StreamObject child = top.FindChild(name, true);
                context.Push(child);
                o.Serialize(this);
                context.Pop();
            }
        }

        public override void Serialize(string name, ref List<int> collection)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamObject child = top.FindChild(mCurrentNameID);
                ++mCurrentNameID;
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {

                        object obj = child.GetPropertyAtIndex(i);
                        ++mCurrentNameID;
                        StreamProperty prop = obj as StreamProperty;
                        if (prop != null && prop.GetPropertyType() == StreamPropertyType.SPT_INT32)
                        {
                            collection.Add(prop.GetIntValue());
                        }
                        else if (prop != null && prop.GetPropertyType() != StreamPropertyType.SPT_INT32)
                        {
                            ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, "property = " + name);
                        }
                    }
                    context.Pop();
                }
            }
            else
            {
                StreamObject child = top.FindChild(name, true);
                child.SetType(StreamPropertyType.SPT_ARRAY);
                context.Push(child);
                for (int i = 0; i < collection.Count; ++i)
                {
                    StreamProperty property = child.AddProperty();
                    property.SetUseHex(ENCODE_HEX);
                    property.SetValue(collection[i]);
                    property.SetArray(true);
                }
                context.Pop();
            }
        }
        public override void Serialize(string name, ref List<long> collection)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamObject child = top.FindChild(mCurrentNameID);
                ++mCurrentNameID;
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {

                        object obj = child.GetPropertyAtIndex(i);
                        ++mCurrentNameID;
                        StreamProperty prop = obj as StreamProperty;
                        if (prop != null && prop.GetPropertyType() == StreamPropertyType.SPT_INT64)
                        {
                            collection.Add(prop.GetLongValue());
                        }
                        else if (prop != null && prop.GetPropertyType() != StreamPropertyType.SPT_INT64)
                        {
                            ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, "property = " + name);
                        }
                    }
                    context.Pop();
                }
            }
            else
            {
                StreamObject child = top.FindChild(name, true);
                child.SetType(StreamPropertyType.SPT_ARRAY);
                context.Push(child);
                for (int i = 0; i < collection.Count; ++i)
                {
                    StreamProperty property = child.AddProperty();
                    property.SetUseHex(ENCODE_HEX);
                    property.SetValue(collection[i]);
                    property.SetArray(true);
                }
                context.Pop();
            }
        }
        public override void Serialize(string name, ref List<float> collection)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamObject child = top.FindChild(mCurrentNameID);
                ++mCurrentNameID;
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {

                        object obj = child.GetPropertyAtIndex(i);
                        ++mCurrentNameID;
                        StreamProperty prop = obj as StreamProperty;
                        if (prop != null && prop.GetPropertyType() == StreamPropertyType.SPT_FLOAT)
                        {
                            collection.Add(prop.GetFloatValue());
                        }
                        else if (prop != null && prop.GetPropertyType() != StreamPropertyType.SPT_FLOAT)
                        {
                            ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, "property = " + name);
                        }
                    }
                    context.Pop();
                }
            }
            else
            {
                StreamObject child = top.FindChild(name, true);
                child.SetType(StreamPropertyType.SPT_ARRAY);
                context.Push(child);
                for (int i = 0; i < collection.Count; ++i)
                {
                    StreamProperty property = child.AddProperty();
                    property.SetUseHex(ENCODE_HEX);
                    property.SetValue(collection[i]);
                    property.SetArray(true);
                }
                context.Pop();
            }
        }
        public override void Serialize(string name, ref List<string> collection)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamObject child = top.FindChild(mCurrentNameID);
                ++mCurrentNameID;
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {

                        object obj = child.GetPropertyAtIndex(i);
                        ++mCurrentNameID;
                        StreamProperty prop = obj as StreamProperty;
                        if (prop != null && prop.GetPropertyType() == StreamPropertyType.SPT_STRING)
                        {
                            collection.Add(prop.GetStringValue());
                        }
                        else if (prop != null && prop.GetPropertyType() != StreamPropertyType.SPT_STRING)
                        {
                            ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, "property = " + name);
                        }
                    }
                    context.Pop();
                }
            }
            else
            {
                StreamObject child = top.FindChild(name, true);
                child.SetType(StreamPropertyType.SPT_ARRAY);
                context.Push(child);
                for (int i = 0; i < collection.Count; ++i)
                {
                    StreamProperty property = child.AddProperty();
                    property.SetUseHex(ENCODE_HEX);
                    property.SetValue(collection[i]);
                    property.SetArray(true);
                }
                context.Pop();
            }
        }

        public override void Serialize<T>(string name, ref List<T> collection)
        {
            StreamContext context = GetActiveContext();
            if (context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if (top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if (IsReading())
            {
                StreamObject child = top.FindChild(mCurrentNameID);
                ++mCurrentNameID;
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {
                        object obj = child.GetPropertyAtIndex(i);
                        ++mCurrentNameID;
                        StreamObject prop = obj as StreamObject;
                        if (prop != null)
                        {
                            context.Push(prop);
                            Type type = typeof(T);
                            BaseObject instance = Activator.CreateInstance(type) as BaseObject;
                            if (instance != null)
                            {
                                instance.Serialize(this);
                                collection.Add(instance as T);
                            }
                            context.Pop();
                        }
                    }
                    context.Pop();
                }
            }
            else
            {
                StreamObject child = top.FindChild(name, true);
                child.SetType(StreamPropertyType.SPT_ARRAY);
                context.Push(child);
                for (int i = 0; i < collection.Count; ++i)
                {
                    StreamObject prop = child.AddChild();
                    prop.SetType(StreamPropertyType.SPT_OBJECT_ARRAY);
                    context.Push(prop);
                    BaseObject baseObject = collection[i] as BaseObject;
                    if (baseObject != null)
                    {
                        baseObject.Serialize(this);
                    }
                    else
                    {
                        BaseComponent baseComponent = collection[i] as BaseComponent;
                        if (baseComponent != null)
                        {
                            baseComponent.Serialize(this);
                        }
                    }
                    context.Pop();
                }
                context.Pop();
            }
        }
        //

        public override void ParseText(string text)
        {
            mParseMode = ParseMode.PM_WHITE_SPACE;
            StringBuilder buffer = new StringBuilder();
            int objectDepth = 0;
            int nameID = 0;
            for(int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                switch (mParseMode)
                {
                    case ParseMode.PM_WHITE_SPACE: // skip context.. should all be one line and simple..
                        {
                            buffer.Append(c);
                            if (c == '\n')
                            {
                                // string

                                string line = buffer.ToString();
                                int start1 = -1; // after $
                                int start2 = -1; // after ,
                                for (int j = 0; j < line.Length; ++j)
                                {
                                    if (start1 == -1 && line[j] == '$')
                                    {
                                        start1 = j + 1;
                                    }
                                    else if (start2 == -1 && line[j] == ',')
                                    {
                                        start2 = j + 1;
                                        break;
                                    }
                                }

                                string typeA = line.Substring(start1, start2 - 2); // offset by 2 & $ sign
                                string typeB = line.Substring(start2, line.Length - start2 - 1); // offset by 1 & $ sign
                                StartContext(typeA, typeB);
                                mParseMode = ParseMode.PM_CONTENT;
                                buffer.Clear();
                                ++objectDepth;
                                nameID = 0;
                                if (line[0] != '$')
                                {
                                    ReportError(StreamError.SE_UNEXPECTED_TOKEN, "EXPECTED $ token at start.");
                                }
                            }
                        }
                        break;
                    case ParseMode.PM_CONTENT:
                        {
                            buffer.Append(c);
                            if(c == '\n')
                            {
                                string line = buffer.ToString();
                                if(line[0] != '$')
                                {
                                    ReportError(StreamError.SE_UNEXPECTED_TOKEN, "EXPECTED $ token at start.");
                                }
                                int valueStart = -1;
                                // determine if there is a value...
                                for(int j =1; j < line.Length; ++j)
                                {
                                    if(line[j] == '$')
                                    {
                                        valueStart = j + 1;
                                        break;
                                    }
                                }

                                // figure out type..
                                string typeString = string.Empty;
                                string valueString = string.Empty;
                                StreamPropertyType propType = StreamPropertyType.SPT_UNKNOWN;

                                // has value
                                if(valueStart != -1)
                                {
                                    typeString = line.Substring(1, valueStart - 2); // ignore '$'x2 and offset by 1
                                    valueString = line.Substring(valueStart, line.Length - valueStart - 1);
                                }
                                else
                                {
                                    typeString = line.Substring(1, line.Length - 2); // ignore '$' and '\n'
                                }

                                // Parse prop type
                                {
                                    int typeValue = 0;
                                    if (int.TryParse(typeString, out typeValue))
                                    {
                                        propType = (StreamPropertyType)typeValue;
                                    }
                                }

                                //Log.Lib.Info("$" + typeString + " TYPE=" + propType.ToString() + " VALUE=" + valueString + " DEPTH=" + objectDepth);

                                switch (propType)
                                {
                                    case StreamPropertyType.SPT_ARRAY:
                                    case StreamPropertyType.SPT_OBJECT:
                                    case StreamPropertyType.SPT_OBJECT_ARRAY:
                                        {
                                            StreamObject top = GetActiveContext().GetTop();
                                            StreamObject child = top.AddChild();
                                            child.SetNameID(nameID);
                                            ++nameID;
                                            child.SetType(propType);
                                            GetActiveContext().Push(child);
                                            ++objectDepth;
                                        }
                                        break;
                                    case StreamPropertyType.SPT_FLOAT:
                                    case StreamPropertyType.SPT_INT32:
                                    case StreamPropertyType.SPT_INT64:
                                    case StreamPropertyType.SPT_STRING:
                                        {
                                            StreamObject top = GetActiveContext().GetTop();
                                            StreamProperty property = top.AddProperty();
                                            property.SetUseHex(ENCODE_HEX);
                                            property.SetNameID(nameID);
                                            ++nameID;
                                            property.SetArray(top.GetPropertyType() == StreamPropertyType.SPT_ARRAY);
                                            property.ParseValue(valueString, propType);
                                        }
                                        break;
                                    case StreamPropertyType.SPT_STREAM_POP:
                                        {
                                            GetActiveContext().Pop();
                                            --objectDepth;
                                            if(objectDepth == 0)
                                            {
                                                // Log.Lib.Info("$" + typeString + " DONE TYPE!");
                                                mParseMode = ParseMode.PM_WHITE_SPACE;
                                                StopContext();
                                            }
                                        }
                                        break;
                                    case StreamPropertyType.SPT_UNKNOWN:
                                    default:
                                        {
                                            ReportError(StreamError.SE_UNEXPECTED_TOKEN, line);
                                        }
                                        break;

                                        
                                }
                                buffer.Clear();
                            }
                        }
                        break;
                }

            }
        }

        public override string WriteText()
        {
            StringBuilder sb = new StringBuilder();
            mActiveContext = 0;
            for(int i = 0; i < mStreamContexts.Count; ++i)
            {
                StreamContext context = GetActiveContext();
                // Write $[A,B]
                sb.Append('$').Append(context.GetTypeName()).Append(',').Append(context.GetParentTypeName()).Append('\n');

                StreamObject top = context.GetTop();
                RecursiveWriteNumericalText(sb, top.GetProperties());
                ++mActiveContext;
            }

            return sb.ToString();
        }

        private void RecursiveWriteNumericalText(StringBuilder sb, List<object> properties)
        {
            string POP_STRING = ((int)StreamPropertyType.SPT_STREAM_POP).ToString(); // Todo.. Can optimize and make constant..
            for (int i = 0; i < properties.Count; ++i)
            {
                StreamProperty property = properties[i] as StreamProperty;
                if (property != null)
                {
                    int propTypeID = (int)property.GetPropertyType();
                    sb.Append('$').Append(propTypeID.ToString()).Append('$').Append(property.GetStringValue()).Append('\n');
                }
                else
                {
                    StreamObject streamObj = properties[i] as StreamObject;
                    if (streamObj != null)
                    {
                        int propTypeID = (int)streamObj.GetPropertyType();
                        sb.Append('$').Append(propTypeID.ToString()).Append('\n');
                        RecursiveWriteNumericalText(sb, streamObj.GetProperties());
                    }
                }
            }
            sb.Append('$').Append(POP_STRING).Append('\n');
        }

    }
}
