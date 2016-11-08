using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace Brightstone
{
    public class TypeWizard : BaseWindow
    {
        private TypeMgr mTypeMgr = null;
        public static TypeWizard sActiveWizard = null;

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

        // Load -- Save
        // 
        // Click and drag Prefab onto "Object Type" .. sets type for prefab.
        // Search type
        // Assign ID - Compile
        // 
        [MenuItem("Brightstone/Type Wizard")]
        public static void ImportWizardWindow()
        {
            const bool UTILITY = true;
            const string TITLE = "Type Wizard";
            const bool FOCUS = true;
            TypeWizard window = GetWindow<TypeWizard>(UTILITY, TITLE, FOCUS);
            window.minSize = new Vector2(650.0f, 300.0f);
            window.maxSize = new Vector2(800.0f, 400.0f);
        }

        protected override void OnInit()
        {
            if(sActiveWizard != null)
            {
                Close();
                return;
            }
            base.OnInit();
            // mTypeMgr = new TypeMgr();
            // mTypeMgr.Init();
            sActiveWizard = this;
        }

        protected override void OnClose()
        {
            base.OnClose();
            if(mTypeMgr != null)
            {
                //mTypeMgr.Shutdown(true);
            }
            if(sActiveWizard == this)
            {
                sActiveWizard = null;
            }
        }

        protected override void OnDraw()
        {
            if(!Application.isPlaying)
            {
                AddButton("Load", LoadButton);
                AddButton("Save", SaveButton);
                AddButton("Compile", CompileButton);
            }
        }

        protected override void OnDrawFrozen()
        {
            if (Application.isPlaying)
            {
                AddButton("Load", LoadButton);
                AddButton("Save", SaveButton);
                AddButton("Compile", CompileButton);
            }
        }

        private void CompileButton(bool pressed)
        {
            if (pressed && !Application.isPlaying)
            {
                Compile();
            }
        }

        private void LoadButton(bool pressed)
        {
            if(pressed && !Application.isPlaying)
            {
                mTypeMgr.Shutdown(false);
                mTypeMgr.Init();
            }
        }

        private void SaveButton(bool pressed)
        {
            if (pressed && !Application.isPlaying)
            {
                mTypeMgr.Shutdown(true);
                mTypeMgr.Init();
            }
        }

        /** Read all "Types" in Target Directory.. Eg resources*/
        private void Compile()
        {
            TypeMap map = new TypeMap();
            RecursiveSearchTypes(Project.GetResourcePath(string.Empty), map.GetTypes());
            Log.Sys.Info("Found " + map.GetTypes().Count + " types.");
            map.SetDirty();
            map.Save();
            // TextStream stream = new TextStream();
            // stream.SetReadingMode(false);
            // stream.StartContext("TypeMap", "BaseObject");
            // map.Serialize(stream);
            // stream.StopContext();
            // System.IO.File.WriteAllText(Util.GetUserDataDirectory(TypeMgr.IMPORT_DATA_LOCATION), stream.WriteText());
        }

        private static void RecursiveSearchTypes(string path, List<TypeData> types)
        {
            // const int STRIP_AMOUNT = 22; // == ("Assets/Resources/Types").Length;

            string[] dirs = Project.GetSubDirectories(path);
            string[] files = Project.GetFilesInDirectory(path);
            for (int i = 0; i < files.Length; ++i)
            {
                string strippedPath = Project.StripToAssets(files[i]);
                GameObject obj = EditorUtils.GetAssetForPath(strippedPath);
                if (obj != null)
                {
                    Actor actor = obj.GetComponent<Actor>();
                    if(actor != null)
                    {
                        Prefab type = actor.GetObjectPrefabType();
                        if (type == null)
                        {
                            string fullname = Project.StripToTypes(files[i]);
                            fullname = fullname.Replace('\\', '/').Replace(".prefab", string.Empty); // remove .prefab extension
                            Log.Sys.Error("Actor is missing type information! " + fullname);
                        }
                        else
                        {
                            string fullname = type.GetName();
                            string basename = type.GetBaseName();

                            if (fullname == string.Empty)
                            {
                                fullname = strippedPath;
                                // int subIndex = 17; //  "Assets/Resources".Length;
                                // int extLength = 7; // ".prefab".Length;
                                const int SUB_INDEX = 17;
                                const int SUB_LENGTH = 24; // subIndex + extLength
                                fullname = fullname.Substring(SUB_INDEX, fullname.Length - SUB_LENGTH);
                                type.SetName(fullname);
                            }

                            if (string.IsNullOrEmpty(basename) && fullname != "Engine/Actor")
                            {
                                Log.Sys.Error("Actor is missing base name! " + (fullname == string.Empty ? strippedPath : fullname));
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
                else if (!files[i].Contains(".meta"))
                {
                    // Object unityObj = EditorUtils.GetAnyAssetForPath(strippedPath);
                    // if (unityObj != null)
                    // {
                    //     ObjectType type = new ObjectType();
                    //     // Strip To Assets
                    //     string fullName = Project.StripToAssets(files[i]);
                    //     fullName = fullName.Replace('\\', '/');
                    //     // Remove Extension
                    //     int extensionStart = fullName.LastIndexOf('.');
                    //     if (extensionStart != -1)
                    //     {
                    //         int size = fullName.Length - extensionStart;
                    //         fullName = fullName.Substring(0, fullName.Length - size);
                    //     }
                    //     // Assign
                    //     type.fullName = fullName.Substring(STRIP_AMOUNT);
                    //     int index = type.fullName.LastIndexOf('/');
                    //     type.name = type.fullName.Substring(index + 1);
                    //     type.id = types.Count;
                    //     types.Add(type);
                    // }
                    // Not sure how to bind type to them yet..
                }
            }
            for (int i = 0; i < dirs.Length; ++i)
            {
                RecursiveSearchTypes(dirs[i], types);
            }
        }

        public TypeMgr GetTypeMgr()
        {
            return mTypeMgr;
        }
    }
}



