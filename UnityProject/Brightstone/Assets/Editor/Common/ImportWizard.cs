using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System.IO;


namespace Brightstone
{
    public class ImportWizard : BaseWindow
    {
        private class ImportData : BaseObject
        {
            public string externalName;
            public string internalName;

            public override void Serialize(BaseStream stream)
            {
                stream.SerializeString("ExternalName", ref externalName);
                stream.SerializeString("InternalName", ref internalName);
            }
        }

        private static ImportWizard sMasterWizard = null;
        private const string IMPORT_DATA_LOCATION = "/ImportWizard.txt";
        private List<ImportData> mImportData = new List<ImportData>();
        private SerializedListProperty mImportDataList = null;
        private Vector2 mScrollPosition = Vector2.zero;
        private string mImportLocation = "D:Game Development/Public Assets/Blender Models";

        private static string ReadText()
        {
            return File.ReadAllText(Util.GetUserDataDirectory(IMPORT_DATA_LOCATION));
        }
        private static void WriteText(string text)
        {
            File.WriteAllText(Util.GetUserDataDirectory(IMPORT_DATA_LOCATION), text);
        }

        [MenuItem("Brightstone/Import Wizard")]
        public static void ImportWizardWindow()
        {
            const bool UTILITY = true;
            const string TITLE = "Import Wizard";
            const bool FOCUS = true;
            ImportWizard window = GetWindow<ImportWizard>(UTILITY, TITLE, FOCUS);
            window.minSize = new Vector2(650.0f, 300.0f);
            window.maxSize = new Vector2(800.0f, 400.0f);
        }

        [MenuItem("Brightstone/CreateTexture32")]
        public static void CreateTexture32()
        {
            const string TEXTURE_PATH = "NewTextureR32.bsd";
            if(File.Exists(EditorUtils.GetPath(TEXTURE_PATH)))
            {
                Log.Sys.Error("File already exists! Delete " + TEXTURE_PATH);
                return;
            }
            Texture2D texture = new Texture2D(32, 32, TextureFormat.RFloat, false, true);
            QuadHeightmap.Save(EditorUtils.GetPath(TEXTURE_PATH), texture);
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Reimport Wizard")]
        public static void ReimportWizard()
        {
            Object selectedObject = Selection.activeObject;
            if(selectedObject == null)
            {
                return;
            }
            string filename = AssetDatabase.GetAssetPath(selectedObject);
            if(filename == string.Empty)
            {
                return;
            }
            filename = filename.Substring(6); // remove "Assets"

            List<ImportData> importData = new List<ImportData>();

            TextStream stream = new TextStream();
            stream.SetReadingMode(true);
            stream.ParseText(ReadText());
            stream.NextContext();
            stream.Serialize("ImportData", ref importData);
            stream.StopContext();

            ImportData data = null;
            for(int i = 0; i < importData.Count; ++i)
            {
                if(importData[i].internalName == filename)
                {
                    data = importData[i];
                    break;
                }
            }

            if(data != null)
            {
                string externalName = data.externalName;
                string internalName = data.internalName;
                internalName = Application.dataPath + internalName;
                // copy external to internal.
                bool exists = File.Exists(internalName);
                if (!exists)
                {
                    Log.Sys.Error("Failed to re-import asset " + filename + ". File has been deleted? " + data.internalName);
                    return;
                }
                exists = File.Exists(externalName);
                if (!exists)
                {
                    Log.Sys.Error("Failed to re-import asset " + filename + ". No file at location " + data.externalName);
                    return;
                }
                

                // byte[] bytes = File.ReadAllBytes(Util.GetUserDataDirectory(IMPORT_DATA_LOCATION));
                
                try
                {
                    File.Copy(externalName, internalName, true);
                    AssetDatabase.Refresh();
                    Selection.activeObject = selectedObject;
                    EditorUtility.FocusProjectWindow();
                    Log.Sys.Info("Reimported " + data.internalName);
                }
                catch(System.Exception ex)
                {
                    Log.Sys.Error("Failed to re-import asset " + ex.Message + "\nSrc=" + externalName + "\nDest=" + internalName);
                }
            }
            else
            {
                Log.Sys.Error("Failed to re-import asset " + filename + ". There is no import data. Set this up using Brigthstone/Import Wizard!");
            }
            
        }

        private void Awake()
        {
            OnInit();
        }

        private void OnDestroy()
        {
            OnClose();
        }

        private void OnGUI()
        {
            InternalDraw();
        }

        protected override void OnInit()
        {
            if(sMasterWizard != null)
            {
                Log.Sys.Error("Multiple Import Wizards not allowed!");
                Close();
                return;
            }
            sMasterWizard = this;
            mImportDataList = CreateList<ImportData>("Import Data", mImportData);
            Load();
        }

        protected override void OnClose()
        {
            if(sMasterWizard == this)
            {
                Save();
                sMasterWizard = null;
            }
            
        }

        protected override void OnDraw()
        {
            if(mImportDataList == null)
            {
                Close();
                return;
            }
            EditorGUILayout.BeginScrollView(mScrollPosition, false, true, GUILayout.Height(300.0f));
            mImportDataList.list.DoLayoutList();
            EditorGUILayout.EndScrollView();
            AddString("Import Location", ref mImportLocation);
            AddButton("Load", LoadButton);
            AddButton("Save", SaveButton);
            AddButton("Set Path", SetPathButton);
        }

        private void LoadButton(bool pressed)
        {
            if(pressed)
            {
                Load();
            }
        }

        private void SaveButton(bool pressed)
        {
            if(pressed)
            {
                Save();
            }
        }

        private void SetPathButton(bool pressed)
        {
            if(pressed)
            {
                Object activeObject = Selection.activeObject;
                if(activeObject != null && mImportDataList.list.index >= 0 && mImportDataList.list.index < mImportData.Count)
                {
                    string path = AssetDatabase.GetAssetPath(activeObject);
                    path = path.Substring(6); // remove "Assets"
                    mImportData[mImportDataList.list.index].internalName = path;
                    mImportData[mImportDataList.list.index].externalName = mImportLocation + path;
                }
            }
        }

        private void Serialize(BaseStream stream)
        {
            stream.SerializeString("ImportLocation", ref mImportLocation);
            stream.Serialize("ImportData", ref mImportData);
        }

        private void Load()
        {
            TextStream stream = new TextStream();
            stream.SetReadingMode(true);
            stream.ParseText(ReadText());
            stream.NextContext();
            Serialize(stream);
            stream.StopContext();
        }

        private void Save()
        {
            TextStream stream = new TextStream();
            stream.SetReadingMode(false);
            stream.StartContext("ImportWizard", "BaseWindow");
            Serialize(stream);
            stream.StopContext();
            WriteText(stream.WriteText());
        }

        private static void ListDraw_ImportData(ReorderableList list)
        {
            const float NAME_WIDTH = 100.0f;
            const float VAR_WIDTH = 650.0f;
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                ImportData element = list.list[index] as ImportData;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, NAME_WIDTH, singleLineHeight), "External:");
                element.externalName = EditorGUI.TextField(new Rect(rect.x + NAME_WIDTH, rect.y, VAR_WIDTH, singleLineHeight), element.externalName);
                EditorGUI.LabelField(new Rect(rect.x, rect.y + singleLineHeight, NAME_WIDTH, singleLineHeight), "Internal:");
                element.internalName = EditorGUI.TextField(new Rect(rect.x + NAME_WIDTH, rect.y + singleLineHeight, VAR_WIDTH, singleLineHeight), element.internalName);
            };
            list.elementHeight = EditorGUIUtility.singleLineHeight * 2.0f;

            // to volatile?

            // list.onRemoveCallback = (ReorderableList l) =>
            // {
            //     if (sMasterWizard != null)
            //     {
            //         sMasterWizard.Save();
            //     }
            // };
            
        }        


    }

}

