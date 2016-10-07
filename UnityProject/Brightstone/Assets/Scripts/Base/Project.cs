using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.IO;

namespace Brightstone
{
    /**
    * -- Useful functions--  
    * GetAssetPath(path)
    * GetAssetPath(path, fullPath)
    * GetResourcePath(path)
    * GetResourcePath(path, fullPath)
    * ReadAllLines(path)
    */
	public static class Project  
	{
        private const int POP_FAILED = 0;
        private const int POP_GOOD = 1;
        private const int POP_FINISHED = 2;

        private static int PopDirectory(ref string dir, out string result)
        {
            result = string.Empty;
            if (dir.Length <= 1)
            {
                return POP_FAILED;
            }

            int index = dir.IndexOf('/');
            if (index == -1)
            {
                result = dir;
                return POP_FINISHED;
            }
            int next = dir.IndexOf('/', index + 1);

            if (next == -1)
            {
                result = dir.Substring(index + 1);
                return POP_FINISHED;
            }

            int length = next - index - 1;
            result = dir.Substring(index + 1, length);
            dir = dir.Substring(next);

            return POP_GOOD;
        }

        /** Assumes path is prefixed with / */
        public static string GetAssetPath(string path)
        {
            return Application.dataPath + path;
        }

        public static string GetAssetPath(string path, bool fullPath)
        {
            if (fullPath)
            {
                return GetAssetPath(path);
            }
            return "Assets " + path;
        }

        /** Assumes path is prefixed with / */
        public static string GetResourcePath(string path)
        {
            return Application.dataPath + "/Resources" + path;
        }

        public static string GetResourcePath(string path, bool fullPath)
        {
            if (fullPath)
            {
                return GetResourcePath(path);
            }
            return "Assets/Resources" + path;
        }

        public static string StripToAssets(string path)
        {
            int index = path.IndexOf("Assets");
            if (index != -1)
            {
                return path.Substring(index);
            }
            return path;
        }

        public static string StripToTypes(string path)
        {
            string typesPath = "Assets/Resources";
            int index = path.IndexOf(typesPath);
            if (index == -1)
            {
                return path;
            }
            index += typesPath.Length;
            return path.Substring(index);
        }


        /**
        * Generate a folder structure to the path. The 
        */
        public static string GenerateFolderStructure(string path)
        {

#if UNITY_EDITOR
            if (path.Length < 1 || path[0] != '/')
            {
                Debug.LogError("Invalid argument in ProjectileUtil.GenerateFolderStructure. Path must start with / ");
                return string.Empty;
            }

            List<string> targets = new List<string>();
            string temp = string.Empty;
            while (PopDirectory(ref path, out temp) == POP_GOOD)
            {
                targets.Add(temp);
            }

            if (targets.Count > 0)
            {
                string guid = AssetDatabase.CreateFolder("Assets", targets[0]);
                string newPath = AssetDatabase.GUIDToAssetPath(guid);

                for (int i = 1; i < targets.Count; ++i)
                {
                    guid = AssetDatabase.CreateFolder(newPath, targets[i]);
                    newPath = AssetDatabase.GUIDToAssetPath(guid);
                }
            }
            return temp;
#else
            return string.Empty;
#endif
        }

        public static string[] GetSubDirectories(string dir)
        {
            return Directory.GetDirectories(dir);
        }

        public static string[] GetFilesInDirectory(string dir)
        {
            return Directory.GetFiles(dir);
        }

        public static string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}