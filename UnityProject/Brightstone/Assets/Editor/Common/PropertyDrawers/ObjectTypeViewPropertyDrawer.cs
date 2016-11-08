using UnityEngine;
using UnityEditor;

namespace Brightstone
{
    [CustomPropertyDrawer(typeof(ObjectTypeViewAttribute))]
    public class ObjectTypeViewPropertyDrawer : PropertyDrawer
    {
        const float HEIGHT = 3.0f;
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

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // string labelText = label.text;
            // position.height = GetPropertyHeight(property, label) * 0.33f;
            // labelText = FormatLabelString(labelText);

            // Button. Set type info!



            float height = position.height * (1.0f / HEIGHT);
            position.height = height;
            Actor actor = property.serializedObject.targetObject as Actor;
            if (property.propertyType == SerializedPropertyType.Generic && actor != null)
            {
                SerializedProperty dataString = property.FindPropertyRelative("mDataString");
                if(dataString == null)
                {
                    EditorGUI.LabelField(position, "Missing data string.");
                }
                else
                {
                    ObjectType type = actor.GetObjectType();
                    GameObject gameObject = actor.gameObject;
                    bool disableUI = !EditorUtils.IsPrefab(gameObject) || Application.isPlaying;
                    if(disableUI)
                    {
                        EditorUtils.DisableUI();
                    }
                    type.InternalInitFromDataString();
                    float width = position.width;
                    float x = position.x;
                    position.width = position.width * 0.7f;
                    string labelText = "Base=" + type.GetBaseName();
                    EditorGUI.LabelField(position, labelText);
                    position.x += position.width;
                    position.width = width * 0.3f;
                    if(GUI.Button(position, "Update"))
                    {
                        if(EditorUtils.IsPrefab(gameObject))
                        {
                            string path = AssetDatabase.GetAssetPath(gameObject);
                            // int subIndex = 16; //  "Assets/Resources".Length;
                            // int extLength = 7; // ".prefab".Length;
                            const int SUB_INDEX = 16;
                            const int SUB_LENGTH = 23; // subIndex + extLength
                            path = path.Substring(SUB_INDEX, path.Length - SUB_LENGTH);
                            string name = type.GetFullName();
                            string baseName = type.GetBaseName();
                            int id = type.GetID();
                            if(string.IsNullOrEmpty(name))
                            {
                                type.InternalInit(baseName, path, id);
                            }
                            EditorUtility.SetDirty(gameObject);
                            EditorUtility.SetDirty(actor);
                        }
                    }
                    position.x = x;
                    position.width = width;
                    position.y += height;

                    GameObject selectedObject = null;
                    selectedObject = EditorGUI.ObjectField(position, "", selectedObject, typeof(GameObject), false) as GameObject;
                    if(selectedObject != null && !EditorUtils.IsPrefab(selectedObject))
                    {
                        Log.Sys.Error("Cannot set base from an object which is not a prefab!");
                    }
                    else if(selectedObject == gameObject)
                    {
                        Log.Sys.Error("Cannot set base as self!");
                    }
                    else if(selectedObject != null && selectedObject.GetComponent(typeof(Actor)) == null)
                    {
                        Log.Sys.Error("Cannot set base, selected object is not an Actor!");
                    }
                    else if(selectedObject != null)
                    {
                        Actor selectedActor = selectedObject.GetComponent(typeof(Actor)) as Actor;
                        ObjectType selectedType = selectedActor.GetObjectType();
                        bool isDerived = false;
                        if (TypeWizard.sActiveWizard == null)
                        {
                            TypeMgr mgr = new TypeMgr();
                            Log.Sys.Warning("Creating a TypeMgr. Use the wizard window to speed this process up.");
                            mgr.Init();
                            isDerived = mgr.IsDerived(type, selectedType);
                            mgr.Shutdown(false);
                        }
                        else
                        {
                            isDerived = TypeWizard.sActiveWizard.GetTypeMgr().IsDerived(type, selectedType);
                        }
                        
                        if(isDerived && type.GetBaseName() != string.Empty)
                        {
                            Log.Sys.Error("Cannot set base, this would cause a cycle!");
                        }
                        else
                        {
                            string path = AssetDatabase.GetAssetPath(selectedObject);
                            // int subIndex = 16; //  "Assets/Resources".Length;
                            // int extLength = 7; // ".prefab".Length;
                            const int SUB_INDEX = 16;
                            const int SUB_LENGTH = 23; // subIndex + extLength
                            path = path.Substring(SUB_INDEX, path.Length - SUB_LENGTH);
                            type.InternalInit(path, type.GetFullName(), type.GetID());
                            EditorUtility.SetDirty(gameObject);
                            EditorUtility.SetDirty(actor);
                        }
                    }
                    position.y += height;
                    EditorGUI.PropertyField(position, dataString);
                    if (disableUI)
                    {
                        EditorUtils.EnableUI();
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(position, "Unsupported property type.");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * HEIGHT;
        }

    }

}

