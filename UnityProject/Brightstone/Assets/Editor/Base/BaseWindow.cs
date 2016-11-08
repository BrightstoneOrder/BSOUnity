using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Brightstone
{
    public class BaseWindow : EditorWindow
    {
        // Functions to call

        // Awake : Called as the new window is opened => OnInit
        // OnDestroy : called as the window is closed => OnClose
        // OnGUI : Called to draw GUI ( slow / repaint ) => InternalDraw
        // Update : Called to update ( fast )

        /** -- Copy Paste --
        void Awake()
        {
            OnInit();
        }
        void OnDestroy()
        {
            OnClose();
        }
        void OnGUI()
        {
            InternalDraw();
        }
        */

        private int mIndentLevel = 0; // cannot go below 0

        public class SerializedListProperty
        {
            public SerializedProperty prop;
            public ReorderableList list;
        }

        public delegate void ListDrawCallback(ReorderableList list);
        public delegate void ButtonCallback(bool pressed);
        protected virtual ListDrawCallback GetListDrawCallback(string typename)
        {
            Type type = GetType();
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

        protected virtual void OnInit()
        {

        }

        protected virtual void OnClose()
        {

        }

        protected void InternalDraw()
        {
            BeginEdit();
            OnDraw();
            GUI.enabled = false;
            OnDrawFrozen();
            GUI.enabled = true;
            EndEdit();
        }

        protected virtual void OnDraw()
        {

        }

        protected virtual void OnDrawFrozen()
        {

        }

        protected SerializedListProperty CreateList<T>(string propName, List<T> list)
        {
            Type type = typeof(T);
            SerializedListProperty listProp = new SerializedListProperty();
            listProp.list = new ReorderableList(list, type, true, true, true, true);
            ListDrawCallback listDraw = GetListDrawCallback(type.Name);
            if (listDraw != null)
            {
                listDraw(listProp.list);
            }
            //CreateListDraw_GameOption(listProp.list);
            return listProp;
        }


        private static string FormatLabelString(string label)
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



        private void BeginEdit()
        {

        }
        private void EndEdit()
        {
            mIndentLevel = 0;
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

        protected void AddButton(string name, ButtonCallback cb)
        {
            if(GUILayout.Button(name))
            {
                if(cb != null)
                {
                    cb.Invoke(true);
                }
            }
            else
            {
                if(cb != null)
                {
                    cb.Invoke(false);
                }
            }
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

