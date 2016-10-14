using System.Text;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Brightstone
{
    /**
    * This class can serialize int, long, string, and BaseObject types.
    */
    class TextStream : BaseStream
    {
        /**Set true if you want to have the text stream encode numbers in hex.*/
        protected static bool ENCODE_HEX = false;
        /**Set true if you want to use 4 spaces instead of a tab. */
        protected static bool ENCODE_TABS_AS_SPACE = false;
        /**Parse mode specific to text stream parsing. */
        protected enum ParseMode
        {
            PM_WHITE_SPACE,
            PM_CONTEXT,
            PM_CONTENT
        }
        /**Indent level for writing text.*/
        private int mIndentLevel = 0;
        /**Active parse mode for the text stream.*/
        protected ParseMode mParseMode = ParseMode.PM_WHITE_SPACE;
        

        public override void SerializeInt(ref int n)
        {
            SerializeInt(GetAnonymousName(), ref n);
        }
        public override void SerializeInt64(ref long n)
        {
            SerializeInt64(GetAnonymousName(), ref n);
        }
        public override void SerializeFloat(ref float n)
        {
            SerializeFloat(GetAnonymousName(), ref n);
        }
        public override void SerializeString(ref string s)
        {
            SerializeString(GetAnonymousName(), ref s);
        }
        public override void SerializeObject(BaseObject o)
        {
            SerializeObject(GetAnonymousName(), o);
        }
        public override void SerializeObject(BaseComponent o)
        {
            SerializeObject(GetAnonymousName(), o);
        }
        public override void SerializeInt(string name, ref int n)
        {
            StreamContext context = GetActiveContext();
            if(context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if(top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            ProcessName(ref name);
            if(IsReading())
            {
                StreamProperty property = top.FindProperty(name);
                if(property != null)
                {
                    if(property.GetPropertyType() != StreamPropertyType.SPT_INT32)
                    {
                        ReportError(StreamError.SE_INVALID_PROPERTY_TYPE, name);
                        return;
                    }
                    n = property.GetIntValue();
                }
            }
            else
            {
                StreamProperty property = top.FindProperty(name, true);
                property.SetUseHex(ENCODE_HEX);
                property.SetValue(n);
            }
            
        }
        public override void SerializeInt64(string name, ref long n)
        {
            StreamContext context = GetActiveContext();
            if(context == null)
            {
                ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
                return;
            }
            StreamObject top = context.GetTop();
            if(top == null)
            {
                ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
                return;
            }
            if(IsReading())
            {
                StreamProperty property = top.FindProperty(name);
                if(property != null)
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
                StreamProperty property = top.FindProperty(name);
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
                StreamProperty property = top.FindProperty(name);
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
                StreamObject child = top.FindChild(name);
                if(child != null)
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
                StreamObject child = top.FindChild(name);
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
                StreamObject child = top.FindChild(name);
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for(int i = 0; i < child.GetArraySize(); ++i)
                    {
                        
                        object obj = child.GetPropertyAtIndex(i);
                        StreamProperty prop = obj as StreamProperty;
                        if(prop != null && prop.GetPropertyType() == StreamPropertyType.SPT_INT32)
                        {
                            collection.Add(prop.GetIntValue());
                        }
                        else if(prop != null && prop.GetPropertyType() != StreamPropertyType.SPT_INT32)
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
                for(int i = 0; i < collection.Count; ++i)
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
                StreamObject child = top.FindChild(name);
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {

                        object obj = child.GetPropertyAtIndex(i);
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

        // public void StartArray(string name)
        // {
        //     StreamContext context = GetActiveContext();
        //     if (context == null)
        //     {
        //         ReportError(StreamError.SE_INVALID_CONTEXT, "property = " + name);
        //         return;
        //     }
        //     StreamObject top = context.GetTop();
        //     if (top == null)
        //     {
        //         ReportError(StreamError.SE_INVALID_OBJECT, "property = " + name);
        //         return;
        //     }
        //     if(IsReading())
        //     {
        //         StreamObject child = top.FindChild(name);
        //         if(child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
        //         {
        //             context.Push(child);
        //         }
        //     }
        // }
        // 
        // public void StopArray()
        // {
        // 
        // }

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
                StreamObject child = top.FindChild(name);
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {

                        object obj = child.GetPropertyAtIndex(i);
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
                StreamObject child = top.FindChild(name);
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {

                        object obj = child.GetPropertyAtIndex(i);
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
                StreamObject child = top.FindChild(name);
                if (child != null && child.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                {
                    context.Push(child);
                    collection.Clear();
                    for (int i = 0; i < child.GetArraySize(); ++i)
                    {
                        object obj = child.GetPropertyAtIndex(i);
                        StreamObject prop = obj as StreamObject;
                        if (prop != null)
                        {
                            context.Push(prop);
                            Type type = typeof(T);
                            BaseObject instance = Activator.CreateInstance(type) as BaseObject;
                            if(instance != null)
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
                    if(baseObject != null)
                    {
                        baseObject.Serialize(this);
                    }
                    else
                    {
                        BaseComponent baseComponent = collection[i] as BaseComponent;
                        if(baseComponent != null)
                        {
                            baseComponent.Serialize(this);
                        }
                    }
                    context.Pop();
                }
                context.Pop();
            }
        }

        private void RecursiveWriteText(StringBuilder sb, List<object> properties)
        {
            for(int i = 0; i < properties.Count;++i)
            {
                StreamProperty property = properties[i] as StreamProperty;
                if(property != null)
                {
                    string propName = property.GetName();
                    string propValueString = string.Empty;
                    switch(property.GetPropertyType())
                    {
                        case StreamPropertyType.SPT_FLOAT:
                            propValueString = "Float(" + property.GetStringValue() + ")";
                            break;
                        case StreamPropertyType.SPT_INT32:
                            propValueString = "Number32(" + property.GetStringValue() + ")";
                            break;
                        case StreamPropertyType.SPT_INT64:
                            propValueString = "Number64(" + property.GetStringValue() + ")";
                            break;
                        case StreamPropertyType.SPT_STRING:
                            propValueString = "String(\"" + property.GetStringValue() + "\")";
                            break;
                        default:

                            break;
                    }
                    string indent = GetIndentString();
                    if(property.IsArray())
                    {
                        sb.Append(indent).Append(propName).Append(propValueString).Append('\n');
                    }
                    else
                    {
                        sb.Append(indent).Append(propName).Append(':').Append(propValueString).Append('\n');
                    }
                }
                else
                {
                    StreamObject streamObj = properties[i] as StreamObject;
                    if(streamObj != null)
                    {
                        if(streamObj.GetPropertyType() == StreamPropertyType.SPT_OBJECT_ARRAY)
                        {
                            sb.Append(GetIndentString()).Append(streamObj.GetName()).Append('{').Append('\n');
                        }
                        else
                        {
                            sb.Append(GetIndentString()).Append(streamObj.GetName()).Append(':').Append('{').Append('\n');
                        }
                        ++mIndentLevel;
                        RecursiveWriteText(sb, streamObj.GetProperties());
                        --mIndentLevel;
                        sb.Append(GetIndentString()).Append('}').Append('\n'); // TODO: IsArray
                    }
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
                sb.Append('$').Append('[').Append(context.GetTypeName()).Append(',').Append(context.GetParentTypeName()).Append(']').Append('\n');
                sb.Append('{').Append('\n');
                mIndentLevel = 1;
                // 
                StreamObject top = context.GetTop();
                RecursiveWriteText(sb, top.GetProperties());
                mIndentLevel = 0;
                sb.Append('}').Append('\n');
                ++mActiveContext;
            }
            return sb.ToString();
        }

        public override void ParseText(string text)
        {
            mParseMode = ParseMode.PM_WHITE_SPACE;
            bool criticalError = false;
            StringBuilder buffer = new StringBuilder();
            int objectDepth = 0;
            bool justReadOpenBrace = false;
            bool readArray = false;
            int lineNumber = 0;
            for(int i = 0; i < text.Length && !criticalError; ++i)
            {
                ParseMode activeParseMode = mParseMode;
                char c = text[i];
                // Keep track of line numbers...
                if(c == '\n')
                {
                    ++lineNumber;
                    // Debug stuff...
                    // string stripped = Util.StrStripWhitespace(buffer.ToString(), true);
                    // Log.Lib.Info(stripped + " objectDepth=" + objectDepth.ToString());
                }
                switch(activeParseMode)
                {
                    // Read white space.. If we come in contact with $ then read the context
                    case ParseMode.PM_WHITE_SPACE:
                        {
                            justReadOpenBrace = false;
                            if(c == ' ' || c == '\t' || c == '\n')
                            {
                                continue;
                            }
                            if(c == '$')
                            {
                                mParseMode = ParseMode.PM_CONTEXT;
                                buffer.Append(c);
                            }
                            else if(c == '}' && objectDepth == 0)
                            {
                                if(GetActiveContext() != null) // maybe serialized nothing?
                                {
                                    GetActiveContext().Pop();
                                }
                                StopContext();
                            }
                            else
                            {
                                ReportError(StreamError.SE_UNEXPECTED_TOKEN, "Mode == PM_WHITE_SPACE");
                            }
                        }
                        break;
                    // Read context $[A,B]
                    case ParseMode.PM_CONTEXT:
                        {
                            justReadOpenBrace = false;
                            buffer.Append(c);
                            if (c == '\n')
                            {
                                if (GetActiveContext() != null)
                                {
                                    mParseMode = ParseMode.PM_CONTENT;
                                    buffer.Clear();
                                }
                                else
                                {
                                    string s = buffer.ToString();
                                    int start = -1; // s.IndexOf('[');
                                    int end = -1; // s.IndexOf(']');
                                    int middle = -1; // s.IndexOf(',');

                                    for (int j = 0; j < s.Length; ++j)
                                    {
                                        char bufferChar = s[j];
                                        if (bufferChar == '[')
                                        {
                                            start = j;
                                        }
                                        else if (bufferChar == ']')
                                        {
                                            end = j;
                                            break;
                                        }
                                        else if (bufferChar == ',')
                                        {
                                            middle = j;
                                        }
                                    }
                                    if (start == -1 || end == -1 || middle == -1)
                                    {
                                        ReportError(StreamError.SE_UNEXPECTED_TOKEN, "Missing context token! [ or ] or , at line " + lineNumber.ToString());
                                        criticalError = true;
                                        break;
                                    }

                                    string name = s.Substring(start + 1, middle - start - 1);
                                    string parentName = s.Substring(middle + 1, end - middle - 1);
                                    StartContext(name, parentName);
                                }
                            }
                        }
                        break;
                    // Read all content of the Context.
                    case ParseMode.PM_CONTENT:
                        {
                            buffer.Append(c);
                            // { immedieatly followed by { is an array of objects. 
                            if(c == '{' && justReadOpenBrace)
                            {
                                StreamObject top = GetActiveContext().GetTop();
                                if(top.GetPropertyType() == StreamPropertyType.SPT_UNKNOWN)
                                {
                                    top.SetType(StreamPropertyType.SPT_ARRAY);
                                }
                                StreamObject child = top.AddChild();
                                child.SetType(StreamPropertyType.SPT_OBJECT_ARRAY);
                                GetActiveContext().Push(child);

                                justReadOpenBrace = false;
                                readArray = true;
                                ++objectDepth;
                                buffer.Clear();
                            }
                            // Read to the end of the line. Skip '\n' only strings (empty lines)
                            else if(c == '\n')
                            {
                                string line = buffer.ToString();
                                string stripped = Util.StrStripWhitespace(line, true);
                                if(stripped == "\n")
                                {
                                    buffer.Clear();
                                    continue;
                                }
                                // Tokens for generating substring.
                                int typeStart = 0;
                                int dataStart = -1;
                                int dataEnd = -1;
                                bool readStruct = false;
                                // Error detection.
                                bool readBraceBefore = justReadOpenBrace;
                                
                                // Find first occurance of tokens.
                                for(int j = 0; j < stripped.Length; ++j)
                                {
                                    char lineChar = stripped[j];
                                    if(lineChar == ':' && typeStart <= 0)
                                    {
                                        typeStart = j;
                                    }
                                    else if(lineChar == '{')
                                    {
                                        ++objectDepth;
                                        readStruct = true;
                                        justReadOpenBrace = true;
                                        break;
                                    }
                                    else if(lineChar == '(' && dataStart == -1)
                                    {
                                        dataStart = j;
                                        break;
                                    }
                                }

                                // Report any error detection.
                                if(typeStart > 0 && ((readBraceBefore && dataStart == -1) || (!readStruct && dataStart == -1)))
                                {
                                    ReportError(StreamError.SE_UNEXPECTED_TOKEN, "Missing { at line " + lineNumber.ToString());
                                    criticalError = true;
                                    continue;
                                }
                                                              
                                string propName = typeStart == 0 ? string.Empty : stripped.Substring(0, typeStart);
                                StreamObject top = GetActiveContext().GetTop();
                                // Start array of properties.
                                if (readStruct && !readArray)
                                {
                                    StreamObject child = top.FindChild(propName, true);
                                    child.SetType(StreamPropertyType.SPT_UNKNOWN);
                                    GetActiveContext().Push(child);
                                    buffer.Clear();
                                    continue;
                                }
                                // Start reading object of an array
                                else if (readStruct && top.GetPropertyType() == StreamPropertyType.SPT_OBJECT_ARRAY)
                                {
                                    StreamObject child = top.FindChild(propName, true);
                                    child.SetType(StreamPropertyType.SPT_UNKNOWN);
                                    GetActiveContext().Push(child);

                                    buffer.Clear();
                                    continue;
                                }
                                else if (readStruct && top.GetPropertyType() == StreamPropertyType.SPT_OBJECT)
                                {
                                    StreamObject child = top.FindChild(propName, true);
                                    child.SetType(StreamPropertyType.SPT_UNKNOWN);
                                    GetActiveContext().Push(child);
                                    buffer.Clear();
                                    continue;
                                }
                                
                                if(justReadOpenBrace && top.GetPropertyType() == StreamPropertyType.SPT_UNKNOWN)
                                {
                                    top.SetType(StreamPropertyType.SPT_ARRAY);
                                }
                                justReadOpenBrace = false;

                                // Read tokens in reverse order.
                                for (int j = stripped.Length-1; j >= 0; --j)
                                {
                                    char lineChar = stripped[j];
                                    if(lineChar == ')')
                                    {
                                        dataEnd = j;
                                        break;
                                    }
                                }
                                // More error detection.
                                if(dataEnd == -1)
                                {
                                    criticalError = true;
                                    ReportError(StreamError.SE_UNEXPECTED_TOKEN, "Missing ) at line " + lineNumber.ToString());
                                    continue;
                                }

                                // Just a single { and this is actually a property.. So top is actually object not array
                                if(readBraceBefore && dataEnd != -1 && typeStart != 0 && top.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                                {
                                    top.SetType(StreamPropertyType.SPT_OBJECT);
                                }

                                // Construct property
                                string typeName = string.Empty;
                                if(typeStart == 0)
                                {
                                    typeName = stripped.Substring(typeStart, dataStart);
                                }
                                else
                                {
                                    typeName = stripped.Substring(typeStart + 1, dataStart - typeStart - 1);
                                }
                                string valueStr = stripped.Substring(dataStart + 1, dataEnd - dataStart - 1);

                                StreamProperty property = top.AddProperty();
                                property.SetUseHex(ENCODE_HEX);
                                property.SetName(propName);
                                if(top.GetPropertyType() == StreamPropertyType.SPT_ARRAY)
                                {
                                    property.SetArray(true);
                                }
                                if(typeName == "String")
                                {
                                    property.ParseValue(valueStr, StreamPropertyType.SPT_STRING);
                                }
                                else if(typeName == "Number32")
                                {
                                    property.ParseValue(valueStr, StreamPropertyType.SPT_INT32);
                                }
                                else if(typeName == "Number64")
                                {
                                    property.ParseValue(valueStr, StreamPropertyType.SPT_INT64);
                                }
                                else if(typeName == "Float")
                                {
                                    property.ParseValue(valueStr, StreamPropertyType.SPT_FLOAT);
                                }
                                else
                                {
                                    if (typeStart == 0 && dataStart != -1)
                                    {
                                        ReportError(StreamError.SE_UNEXPECTED_TOKEN, "Missing : at line " + lineNumber.ToString());
                                        criticalError = true;
                                        continue;
                                    }
                                }
                                buffer.Clear();
                            }
                            else if(c == '}')
                            {
                                --objectDepth;
                                if(objectDepth > 0)
                                {
                                    if(GetActiveContext().GetTop().GetPropertyType() == StreamPropertyType.SPT_OBJECT_ARRAY)
                                    {
                                        justReadOpenBrace = true;
                                    }
                                    GetActiveContext().Pop();
                                }
                                else
                                {
                                    mParseMode = ParseMode.PM_WHITE_SPACE;
                                }

                                if(readArray)
                                {
                                    readArray = false;
                                }
                                buffer.Clear();
                            }
                        }
                        break;
                }
            }
        }

        /** Helper function to "tab" string */
        private string GetIndentString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Capacity = ENCODE_TABS_AS_SPACE ? mIndentLevel * 4 : mIndentLevel;
            for(int i = 0; i < mIndentLevel; ++i)
            {
                if(ENCODE_TABS_AS_SPACE)
                {
                    sb.Append(' ');
                }
                else
                {
                    sb.Append('\t');
                }
            }
            return sb.ToString();
        }

        public override bool IsText()
        {
            return true;
        }
    }
}
