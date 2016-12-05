using UnityEngine;
using UnityEditor;

namespace Brightstone
{
    [CustomPropertyDrawer(typeof(PrefabViewAttribute))]
    public class PrefabViewPropertyDrawer : PropertyDrawer
    {
        static TypeMap sTypeMap = null;

        private TypeMap GetTypeMap()
        {
            if(sTypeMap == null)
            {
                sTypeMap = new TypeMap();
                sTypeMap.Load();
            }
            return sTypeMap;
        }

        const float HEIGHT_MULTIPLIER = 3.0f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // This property should be a generic type..
            
            // Find children! 

            Actor actor = property.serializedObject.targetObject as Actor;
            if(property.name == "mType" && actor != null)
            {
                DrawActorType(position,property);
            }
            else
            {
                DrawResourceType(position, property, label.text);
            }
        }

        private GameObject FindGameObjectForName(string name)
        {
            return EditorUtils.GetAssetForPath("Assets/Resources/" + name);
        }

        /**
         * Called to draw the actor's type assignment GUI. Note this is for PREFAB only not PREFAB INSTANCE
         * 
         * 1. Draw a drag and drop item to get the base
         * 2. Draw Type Name
         * 3. Draw Base Name
         * 
         * -- On base name change.. Open type map, submit change!
         * 
        */
        private void DrawActorType(Rect position, SerializedProperty property)
        {
            float height = position.height * (1.0f / HEIGHT_MULTIPLIER);
            position.height = height;
            SerializedProperty editorInstanceProp = property.FindPropertyRelative("mEditorInstance");
            SerializedProperty nameProp = property.FindPropertyRelative("mName");
            SerializedProperty baseNameProp = property.FindPropertyRelative("mBaseName");
            TypeMap map = GetTypeMap();

            // Verify name.
            string originalName = nameProp.stringValue;
            Actor actor = property.serializedObject.targetObject as Actor;
            GameObject selected = actor != null ? actor.gameObject : null;
            if(selected && EditorUtils.IsPrefab(selected))
            {

                string path = AssetDatabase.GetAssetPath(selected);
                // int subIndex = 17; //  "Assets/Resources".Length;
                // int extLength = 7; // ".prefab".Length;
                const int SUB_INDEX = 17;
                const int SUB_LENGTH = 24; // subIndex + extLength
                path = path.Substring(SUB_INDEX, path.Length - SUB_LENGTH);
                nameProp.stringValue = path;
            }

            if(originalName != nameProp.stringValue)
            {
                map.SetName(originalName, nameProp.stringValue);
            }
            // 1. Get Base
            string originalBaseName = baseNameProp.stringValue;
            editorInstanceProp.objectReferenceValue = EditorGUI.ObjectField(position, "Set Base", editorInstanceProp.objectReferenceValue, typeof(GameObject), false) as GameObject;
            GameObject target = editorInstanceProp.objectReferenceValue as GameObject;
            if(target != null && EditorUtils.IsPrefab(target))
            {
                string path = AssetDatabase.GetAssetPath(target);
                // int subIndex = 17; //  "Assets/Resources".Length;
                // int extLength = 7; // ".prefab".Length;
                const int SUB_INDEX = 17;
                const int SUB_LENGTH = 24; // subIndex + extLength
                path = path.Substring(SUB_INDEX, path.Length - SUB_LENGTH);
                baseNameProp.stringValue = path;
            }

            if(baseNameProp.stringValue != originalBaseName)
            {
                map.SetBaseName(originalBaseName, baseNameProp.stringValue);
            }
            map.Save();

            EditorUtils.DisableUI();
            // 2.
            position.y += height;
            EditorGUI.LabelField(position, "Type: " + nameProp.stringValue);
            //EditorGUI.ObjectField(position, "Type:", selected, typeof(GameObject), false);
            // 3.
            position.y += height;
            if(target == null && baseNameProp.stringValue != string.Empty)
            {
                target = FindGameObjectForName(baseNameProp.stringValue);
            }
            EditorGUI.LabelField(position, "Base: " + baseNameProp.stringValue);
            //EditorGUI.ObjectField(position, "Base:", target, typeof(GameObject), false);
            EditorUtils.EnableUI();

            property.serializedObject.ApplyModifiedProperties();
        }

        /**
         * Called to draw anyones 'Resource' type of Prefab.
         * 
         * 1. Draw a drag and drop item to get the type
         * 2. Draw the type name
         * 3. Draw the base name.
         * 
         * This should not be modifying any type map data.
         */
        private void DrawResourceType(Rect position, SerializedProperty property, string label)
        {
            float height = position.height * (1.0f / HEIGHT_MULTIPLIER);
            position.height = height;
            SerializedProperty editorInstanceProp = property.FindPropertyRelative("mEditorInstance");
            SerializedProperty nameProp = property.FindPropertyRelative("mName");
            SerializedProperty baseNameProp = property.FindPropertyRelative("mBaseName");
            TypeMap map = GetTypeMap();
            PrefabViewAttribute view = attribute as PrefabViewAttribute;
            string constraintName = view.constraint.Name;
            // Our type... is what is selected if were a prefab.

            string originalName = nameProp.stringValue;
            GUIContent propName = new GUIContent(label + ": Set Type (" + constraintName + ")", property.tooltip);
            editorInstanceProp.objectReferenceValue = EditorGUI.ObjectField(position, propName, editorInstanceProp.objectReferenceValue, typeof(GameObject), false) as GameObject;
            GameObject target = editorInstanceProp.objectReferenceValue as GameObject;
            if(target == null)
            {
                return;
            }
            if(!EditorUtils.IsPrefab(target))
            {
                Log.Sys.Error("Set type must be a prefab!");
                editorInstanceProp.objectReferenceValue = null;
                return;
            }
            
            Actor actor = target.GetComponent(view.constraint) as Actor;
            if(actor == null)
            {
                Log.Sys.Error("Set type must be a " + constraintName);
                editorInstanceProp.objectReferenceValue = null;
                return;
            }


            if(target != null && EditorUtils.IsPrefab(target))
            {
                string path = AssetDatabase.GetAssetPath(target);
                // int subIndex = 17; //  "Assets/Resources".Length;
                // int extLength = 7; // ".prefab".Length;
                const int SUB_INDEX = 17;
                const int SUB_LENGTH = 24; // subIndex + extLength
                path = path.Substring(SUB_INDEX, path.Length - SUB_LENGTH);
                nameProp.stringValue = path;
            }

            if(nameProp.stringValue != originalName)
            {
                string baseName = map.GetBaseName(nameProp.stringValue);
                baseNameProp.stringValue = baseName;
            }

            EditorUtils.DisableUI();
            // 2.
            position.y += height;
            EditorGUI.LabelField(position, "Type: " + nameProp.stringValue);
            //EditorGUI.ObjectField(position, "Type:", selected, typeof(GameObject), false);
            // 3.
            position.y += height;
            if (target == null && baseNameProp.stringValue != string.Empty)
            {
                target = FindGameObjectForName(baseNameProp.stringValue);
            }
            EditorGUI.LabelField(position, "Base: " + baseNameProp.stringValue);
            //EditorGUI.ObjectField(position, "Base:", target, typeof(GameObject), false);
            EditorUtils.EnableUI();

            property.serializedObject.ApplyModifiedProperties();
        }

        private string DrawDragAndDrop(SerializedProperty prop, Rect rect)
        {
            prop.objectReferenceValue = EditorGUI.ObjectField(rect, EditorUtils.FormatLabelString(prop.name), prop.objectReferenceValue, typeof(GameObject), false);
            GameObject target = prop.objectReferenceValue as GameObject;
            if(target == null && prop.objectReferenceValue != null)
            {
                prop.objectReferenceValue = null;
                Log.Sys.Error("Cannot set object type because source is not a GameObject!");
                return string.Empty;
            }
            if(target == null)
            {
                return string.Empty;
            }
            if(!EditorUtils.IsPrefab(target))
            {
                prop.objectReferenceValue = null;
                Log.Sys.Error("Cannot set object type because source it not a prefab!");
                return string.Empty;
            }
            string path = AssetDatabase.GetAssetPath(target);
            // int subIndex = 17; //  "Assets/Resources".Length;
            // int extLength = 7; // ".prefab".Length;
            const int SUB_INDEX = 17;
            const int SUB_LENGTH = 24; // subIndex + extLength
            path = path.Substring(SUB_INDEX, path.Length - SUB_LENGTH);

            return path;
        }

        private void DrawBaseDragAndDrop(SerializedProperty prop, Rect rect)
        {
            GameObject target = null;
            target = EditorGUI.ObjectField(rect, "Drag And Drop Base", target, typeof(GameObject), false) as GameObject;
            if(target == null)
            {
                return;
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * HEIGHT_MULTIPLIER;
        }

    }
}