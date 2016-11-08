using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Brightstone
{
    public static class EditorUtils
    {
        public static void DisableUI()
        {
            GUI.enabled = false;
        }
        public static void EnableUI()
        {
            GUI.enabled = true;
        }

        public static T ObjectField<T>(T tObject) where T : Object
        {
            return EditorGUILayout.ObjectField(tObject, typeof(T), true) as T; ;
        }

        public static T ObjectField<T>(T tObject, float width) where T : Object
        {
            return EditorGUILayout.ObjectField(tObject, typeof(T), true, GUILayout.Width(width)) as T;
        }

        public static Rect GetScreenCenterRect(float width, float height)
        {
            float scWidth = (float)Screen.width;
            float scHEight = (float)Screen.height;
            return new Rect(scWidth * 0.5f - width * 0.5f, scHEight * 0.5f - height * 0.5f, width, height);
        }

        public static bool Button(string name, float width)
        {
            return GUILayout.Button(name, GUILayout.Width(width));
        }

        private const string ENGINE_SCENE_PATH = "Assets/Scenes/EngineLevels/";

        private static void EditorLoadLevel(string name)
        {
            if (!Application.isPlaying)
            {
                EditorApplication.SaveAssets();
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                EditorSceneManager.OpenScene(name);
            }
        }

        private static T NewAsset<T>(string name, string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            asset.name = name;

            AssetDatabase.CreateAsset(asset, path + name + ".asset");
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            Debug.Log("Created new asset " + name + " at " + path);
            return asset;
        }

        public static bool IsPrefab(Object context)
        {
            PrefabType type = PrefabUtility.GetPrefabType(context);
            if (type == PrefabType.Prefab)
            {
                return true;
            }
            return false;
        }

        public static GameObject GetAssetForPath(string path)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        public static Object GetAnyAssetForPath(string path)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture != null)
            {
                return texture;
            }
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (mesh != null)
            {
                return mesh;
            }
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip != null)
            {
                return clip;
            }
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
            {
                return mat;
            }
            return null;

        }

        public static string FormatLabelString(string label)
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

    }
}


