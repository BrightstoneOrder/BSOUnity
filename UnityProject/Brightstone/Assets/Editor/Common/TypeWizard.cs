using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace Brightstone
{
    // How to use:
    // Brightstone > Type Wizard
    // Load Types: Will load the type map in from the file
    // Save Types: Will save the type map to the file (This is not necessary if you assign a base type)
    // Compile: Will check the resource directory for all types.
    // 
    // 
    public class TypeWizard
    {
        [MenuItem("Brightstone/Type Wizard/Compile")]
        public static void ImportWizardWindow()
        {
            Compile();
        }

        /** Read all "Types" in Target Directory.. Eg resources*/
        static private void Compile()
        {
            TypeMap map = new TypeMap();
            List<string> typeNames = new List<string>();
            RecursiveSearchTypes(Project.GetResourcePath(string.Empty), typeNames);
            Log.Sys.Info("Found " + typeNames.Count + " prefabs.");
            for(int i = 0; i < typeNames.Count; ++i)
            {
                ProcessType(typeNames[i], map.GetTypes());
            }
            map.SetDirty();
            map.Save();
        }

        private static void RecursiveSearchTypes(string path, List<string> filenames)
        {
            // Get contained directories or files:
            string[] dirs = Project.GetSubDirectories(path);
            string[] files = Project.GetFilesInDirectory(path);
            // Add files .prefab non .meta only!:
            for(int i= 0; i < files.Length; ++i)
            {
                string strippedPath = Project.StripToAssets(files[i]);
                if(!strippedPath.Contains(".meta") && strippedPath.Contains(".prefab"))
                {
                    filenames.Add(strippedPath);
                }
            }
            // Check directories:
            for(int i = 0; i < dirs.Length; ++i)
            {
                RecursiveSearchTypes(dirs[i], filenames);
            }
        }

        private static void ProcessType(string filename, List<TypeData> types)
        {
            GameObject obj = EditorUtils.GetAssetForPath(filename);
            if(obj == null)
            {
                return;
            }
            if(!EditorUtils.IsPrefab(obj))
            {
                return; // This shouldn't happen as were pulling from the assets dir directly.
            }

            Actor actor = obj.GetComponent<Actor>();
            if(actor == null)
            {
                Log.Sys.Error("Prefab is missing an actor! " + filename);
                return;
            }
            Prefab type = actor.GetObjectPrefabType();
            if(type == null)
            {
                string fullname = Project.StripToTypes(filename);
                fullname = fullname.Replace('\\', '/').Replace(".prefab", string.Empty);
                Log.Sys.Error("Actor is missing type information! " + fullname);
            }

            else
            {
                string fullname = type.GetName();
                string basename = type.GetBaseName();

                // Set filename:
                if (fullname == string.Empty)
                {
                    fullname = filename;
                    // int subIndex = 17; //  "Assets/Resources".Length;
                    // int extLength = 7; // ".prefab".Length;
                    const int SUB_INDEX = 17;
                    const int SUB_LENGTH = 24; // subIndex + extLength
                    fullname = fullname.Substring(SUB_INDEX, fullname.Length - SUB_LENGTH);
                    type.SetName(fullname);
                }

                // Verify it has a base: Unless accepted type.
                if (string.IsNullOrEmpty(basename) && fullname != "Engine/Actor")
                {
                    Log.Sys.Error("Actor is missing base name! " + (fullname == string.Empty ? filename : fullname));
                }


                TypeData typeData = new TypeData();
                typeData.name = fullname;
                typeData.baseName = basename;

                types.Add(typeData);
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(actor);
            }

        }
    }
}



