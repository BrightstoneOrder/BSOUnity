using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brightstone
{
	public class BaseEditor : Editor 
	{
        public class SerializedListProperty
        {
            public SerializedProperty prop;
            public ReorderableList list;
        }

        private List<SerializedProperty> mAutoProperties = null;

        public delegate void ListDrawCallback(ReorderableList list);
        protected virtual ListDrawCallback GetListDrawCallback(string typename)
        {
            Type type = typeof(BaseEditor);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            for (int i = 0; i < methods.Length; ++i)
            {
                string name = methods[i].Name;
                if (name.Contains(typename) && name.Contains("ListDraw"))
                {
                    Delegate del = Delegate.CreateDelegate(typeof(ListDrawCallback), methods[i]);
                    return del as ListDrawCallback;
                }
            }
            return null;
        }

        protected SerializedListProperty CreateList(string propName)
        {
            SerializedListProperty listProp = new SerializedListProperty();
            listProp.prop = serializedObject.FindProperty(propName);
            listProp.list = new ReorderableList(serializedObject, listProp.prop, true, true, true, true);

            ListDrawCallback listDraw = GetListDrawCallback(listProp.prop.type);
            if (listDraw != null)
            {
                listDraw(listProp.list);
            }
            //CreateListDraw_GameOption(listProp.list);
            return listProp;
        }

        private static List<string> GetSerializedFields(Type type)
        {
            List<string> fieldStrings = new List<string>();
            MemberInfo[] fields = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance);
            if(fields == null || fields.Length == 0)
            {
                return fieldStrings;
            }
            Type attribType = typeof(SerializeField);
            for (int i = 0; i < fields.Length; ++i)
            {
                if(fields[i].GetCustomAttributes(attribType,true).Length > 0)
                {
                    fieldStrings.Add(fields[i].Name);
                }
            }
            return fieldStrings;
        }

        private string FormatLabelString(string label)
        {
            int offset = 0;
            if (label.Length > 1 && label[0] == 'm')
            {
                if (label[1] == '_') // m_Health
                {
                    offset = 2;
                }
                else if (char.IsUpper(label[1])) // mHealth
                {
                    offset = 1;
                }
            }
            label = label.Substring(offset);
            bool readingLower = char.IsLower(label[0]);

            for (int i = 1; i < label.Length; ++i)
            {
                bool charIsUpper = char.IsUpper(label[i]);
                if (readingLower && charIsUpper)
                {
                    //label.Insert(i-1, " "); // insert not working?
                    label = label.Substring(0, i) + " " + label.Substring(i);
                    ++i;
                }
                readingLower = !charIsUpper;
            }
            return label;
        }

        protected void AutoSerializeProperties(Type editType)
        {
            if(mAutoProperties == null)
            {
                mAutoProperties = new List<SerializedProperty>();
                List<string> propNames = GetSerializedFields(editType);
                for(int i = 0; i < propNames.Count; ++i)
                {
                    SerializedProperty prop = serializedObject.FindProperty(propNames[i]);
                    if(prop != null)
                    {
                        mAutoProperties.Add(prop);
                    }
                }
            }
            for(int i = 0; i < mAutoProperties.Count; ++i)
            {
                AddProperty(mAutoProperties[i]);
            }
        }

        // private static void ListDraw_GameOption(ReorderableList list)
        // {
        //     const float VARIABLE_NAME_WIDTH = 100.0f;
        //     float singleLineHeight = EditorGUIUtility.singleLineHeight;
        //     list.drawElementCallback =
        //         (Rect rect, int index, bool isActive, bool isFocused) =>
        //         {
        //             // type - GameOptionType
        //             // value - String
        //             var element = list.serializedProperty.GetArrayElementAtIndex(index);
        //             EditorGUI.LabelField(new Rect(rect.x, rect.y, VARIABLE_NAME_WIDTH, singleLineHeight), "Name:");
        //             EditorGUI.PropertyField(new Rect(rect.x + VARIABLE_NAME_WIDTH, rect.y, rect.width - VARIABLE_NAME_WIDTH, singleLineHeight),
        //                 element.FindPropertyRelative("mName"), GUIContent.none);
        //             EditorGUI.LabelField(new Rect(rect.x, rect.y + singleLineHeight, VARIABLE_NAME_WIDTH, singleLineHeight), "Option Type:");
        //             EditorGUI.PropertyField(new Rect(rect.x + VARIABLE_NAME_WIDTH, rect.y + singleLineHeight, rect.width - VARIABLE_NAME_WIDTH, singleLineHeight),
        //                 element.FindPropertyRelative("mType"), GUIContent.none);
        //             EditorGUI.LabelField(new Rect(rect.x, rect.y + singleLineHeight * 2.0f, VARIABLE_NAME_WIDTH, singleLineHeight), "Value:");
        //             var optionTypeProp = element.FindPropertyRelative("mType");
        //             if (optionTypeProp.propertyType != SerializedPropertyType.Enum)
        //             {
        //                 EditorGUI.PropertyField(new Rect(rect.x + VARIABLE_NAME_WIDTH, rect.y + singleLineHeight * 2.0f, rect.width - VARIABLE_NAME_WIDTH, singleLineHeight),
        //                 element.FindPropertyRelative("mValue"), GUIContent.none);
        //             }
        //             else
        //             {
        //                 GameOptionType optionType = (GameOptionType)(Enum.GetValues(typeof(GameOptionType)).GetValue(optionTypeProp.enumValueIndex));
        //                 SerializedProperty valueProp = element.FindPropertyRelative("mValue");
        //                 Rect valueRect = new Rect(rect.x + VARIABLE_NAME_WIDTH, rect.y + singleLineHeight * 2.0f, rect.width - VARIABLE_NAME_WIDTH, singleLineHeight);
        // 
        //                 switch (optionType)
        //                 {
        //                     case GameOptionType.Bool:
        //                         {
        //                             bool value = false;
        //                             bool.TryParse(valueProp.stringValue, out value);
        //                             valueProp.stringValue = EditorGUI.Toggle(valueRect, value).ToString();
        //                         }
        //                         break;
        //                     case GameOptionType.Float:
        //                         {
        //                             float value = 0.0f;
        //                             float.TryParse(valueProp.stringValue, out value);
        //                             valueProp.stringValue = EditorGUI.FloatField(valueRect, value).ToString();
        //                         }
        //                         break;
        //                     case GameOptionType.Int:
        //                         {
        //                             int value = 0;
        //                             int.TryParse(valueProp.stringValue, out value);
        //                             valueProp.stringValue = EditorGUI.IntField(valueRect, value).ToString();
        //                         }
        //                         break;
        //                     case GameOptionType.String:
        //                         valueProp.stringValue = EditorGUI.TextField(valueRect, valueProp.stringValue);
        //                         break;
        //                 }
        // 
        //             }
        //         };
        // 
        //     list.elementHeight = EditorGUIUtility.singleLineHeight * 3.0f;
        // }

        private int mIndentLevel = 0; // cannot go below 0


        private void BeginEdit()
        {
            serializedObject.Update();
        }

        private void EndEdit()
        {
            serializedObject.ApplyModifiedProperties();
            mIndentLevel = 0;
        }

        public override void OnInspectorGUI()
        {
            BeginEdit();
            BaseComponent targetComponent = target as BaseComponent;
            OnEditorUpdate(targetComponent);
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                OnGameUpdate(targetComponent);
                GUI.enabled = true;
                Repaint();
            }
            EndEdit();
        }

        protected virtual void InitProperties()
        {

        }

        /** Called when game is running to update inspector. */
        protected virtual void OnGameUpdate(BaseComponent targetComponent)
        {

        }

        /** Called when in game or editor to update inspector. */
        protected virtual void OnEditorUpdate(BaseComponent targetComponent)
        {

        }

        protected bool ShowDebug(BaseComponent targetComponent)
        {
            return AddFoldout("Show Debug", "showDebug", targetComponent);
        }

        protected void AddProperty(SerializedProperty prop)
        {
            string name = FormatLabelString(prop.name);
            GUIContent label = new GUIContent(name);
            EditorGUILayout.PropertyField(prop, label, true);
        }

        protected void AddProperty(SerializedListProperty prop)
        {
            if (prop != null && prop.prop != null && prop.prop.isArray)
            {
                prop.list.DoLayoutList();
            }
        }

        protected void AddToggle(string name, ref int value)
        {
            value = EditorGUILayout.Toggle(name, value != 0) ? 1 : 0;
        }
        protected void AddToggle(string name, int value)
        {
            EditorGUILayout.Toggle(name, value != 0);
        }

        protected void AddFoldout(string name, ref int value)
        {
            value = EditorGUILayout.Foldout(value != 0, name) ? 1 : 0;
        }
        protected bool AddFoldout(string name, string varName, BaseComponent target)
        {
            int value = target.GetEditorInt(varName);
            AddFoldout(name, ref value);
            target.SetEditorInt(varName, value);
            return value != 0;
        }

        protected void AddLabel(string label)
        {
            EditorGUILayout.LabelField(label);
        }

        protected void AddFloat(string name, ref float value)
        {
            value = EditorGUILayout.FloatField(name, value);
        }

        protected void AddFloat(string name, float value)
        {
            EditorGUILayout.FloatField(name, value);
        }

        protected void AddInt(string name, ref int value)
        {
            value = EditorGUILayout.IntField(name, value);
        }

        protected void AddInt(string name, int value)
        {
            EditorGUILayout.IntField(name, value);
        }

        protected void AddString(string name, ref string value)
        {
            value = EditorGUILayout.TextField(name, value);
        }

        protected void AddString(string name, string value)
        {
            EditorGUILayout.TextField(name, value);
        }

        protected void AddVector(string name, ref Vector3 vector)
        {
            vector = EditorGUILayout.Vector3Field(name, vector);
        }

        protected void AddVector(string name, Vector3 vector)
        {
            EditorGUILayout.Vector3Field(name, vector);
        }

        protected void AddObject<T>(string name, ref T obj) where T : UnityEngine.Object
        {
            if (obj == null)
            {
                EditorGUILayout.ObjectField(name, null, typeof(T), true);
                return;
            }
            obj = EditorGUILayout.ObjectField(name, obj, obj.GetType(), true) as T;
        }

        /** 
        * @param showAll - Show all members of enum T even if not enabled.
        */
        protected void AddBitfield<T>(string name, Bitfield<T> bitfield, bool showAll) where T : struct, IConvertible
        {
            string[] names = Enum.GetNames(typeof(T));
            EditorGUILayout.LabelField(name);
            PushIndent();
            for (int i = 0; i < names.Length; ++i)
            {
                int mask = 1 << i;
                bool isEnabled = bitfield.Is(mask);
                string displayString = names[i] + " : " + (isEnabled ? "ON" : "OFF");
                if (isEnabled)
                {
                    EditorGUILayout.LabelField(displayString);
                }
                else if (showAll)
                {
                    EditorGUILayout.LabelField(displayString);
                }
            }
            PopIndent();
        }

        protected void PushIndent()
        {
            ++mIndentLevel;
        }

        protected void PopIndent()
        {
            if (mIndentLevel > 0)
            {
                --mIndentLevel;
            }
        }

        protected void PushGroup(string name)
        {
            EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            PushIndent();
        }

        protected void PopGroup()
        {
            PopIndent();
        }
    }
}