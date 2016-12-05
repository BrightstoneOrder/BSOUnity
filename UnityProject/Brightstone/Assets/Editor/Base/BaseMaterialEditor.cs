using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Brightstone
{
    public class BaseMaterialEditor : MaterialEditor
    {
        private class ShaderOption
        {
            private string mOptionName = string.Empty;
            private string mOptionPrettyName = string.Empty;
            private string mOptionDescription = string.Empty;
            private bool mEnabled = false;

            public void SetName(string name)
            {
                mOptionName = name;
                bool setCap = true;
                for (int i = 0; i < mOptionName.Length; ++i)
                {
                    if(mOptionName[i] == '_')
                    {
                        mOptionPrettyName += ' ';
                        setCap = true;
                        continue;
                    }
                    if(setCap)
                    {
                        mOptionPrettyName += char.ToUpper(mOptionName[i]);
                        setCap = false;
                    }
                    else
                    {
                        mOptionPrettyName += char.ToLower(mOptionName[i]);
                    }
                }
            }

            public void SetDescription(string description)
            {
                mOptionDescription = description;
            }

            public string name { get { return mOptionName; } }
            public string prettyName { get { return mOptionPrettyName; } }
            public string description { get { return mOptionDescription; } }
            public bool enabled { get { return mEnabled; } set { mEnabled = value; } }
        }

        private List<ShaderOption> mOptions = new List<ShaderOption>();


        /** Called to register shader options. */
        protected virtual void OnRegisterOptions()
        {

        }

        protected void AddOption(string name)
        {
            AddOption(name, string.Empty);
        }

        protected void AddOption(string name, string desc)
        {
            ShaderOption opt = new ShaderOption();
            opt.SetName(name);
            opt.SetDescription(desc);
            mOptions.Add(opt);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(!isVisible)
            {
                return;
            }
            // Register Options
            if(mOptions.Count == 0)
            {
                OnRegisterOptions();
            }
            // Get Active Options
            Material self = target as Material;
            for(int i = 0; i < mOptions.Count; ++i)
            {
                mOptions[i].enabled = self.shaderKeywords.Contains(mOptions[i].name + "_ON");
            }
            // Get Input
            EditorGUI.BeginChangeCheck();
            for(int i = 0; i < mOptions.Count; ++i)
            {
                mOptions[i].enabled = EditorGUILayout.Toggle(new GUIContent(mOptions[i].prettyName, mOptions[i].description), mOptions[i].enabled);
            }

            // Update Options
            if(EditorGUI.EndChangeCheck())
            {
                List<string> keywords = new List<string>();
                for(int i = 0; i < mOptions.Count; ++i)
                {
                    if(mOptions[i].enabled)
                    {
                        keywords.Add(mOptions[i].name + "_ON");
                    }
                    else
                    {
                        keywords.Add(mOptions[i].name + "_OFF");
                    }
                }
                self.shaderKeywords = keywords.ToArray();
                // string msg = string.Empty;
                // for(int i = 0; i < keywords.Count; ++i)
                // {
                //     msg += keywords[i] + " ";
                // }
                // Debug.Log("Keywords " + msg);

                EditorUtility.SetDirty(self);
            }
        }

         
    }
}



