using System.Collections.Generic;

namespace Brightstone
{
    public class OptionMgr : BaseObject
    {
        NestedObject<Option>[] mOptions = null;
        private string mWriteLocation = "/Game/Options.txt";

        public void Init()
        {
            int size = Util.GetEnumCount<OptionName>();
            mOptions = new NestedObject<Option>[size];
            bool editorOnly = true;
            for(int i = 0; i < size; ++i)
            {
                mOptions[i] = new NestedObject<Option>();
                if(i == (int)OptionName.ON_CLICK_TO_MOVE)
                {
                    editorOnly = false;
                }
                else if(i == (int)OptionName.ON_PHYSICS_QUERIES_PER_UPDATE)
                {
                    editorOnly = true;
                }
                mOptions[i].SetInstance(new Option(editorOnly));
            }
            mOptions[0].GetInstance().InitBool(OptionName.ON_NONE, "NONE", false);
            mOptions[1].GetInstance().InitBool(OptionName.ON_CLICK_TO_MOVE, "Click To Move", false);
            mOptions[2].GetInstance().InitFloat(OptionName.ON_CAMERA_SENSITIVITY, "Camera Sensitivity", 0.5f, 0.1f, 1.0f);
            mOptions[3].GetInstance().InitEnum(OptionName.ON_GRAPHICS_GENERAL, "Graphics Preset", new[] {
                new EnumDescriptor("Low", 0),
                new EnumDescriptor("Medium", 1),
                new EnumDescriptor("High", 2),
                new EnumDescriptor("Ultra", 3)
            });
            mOptions[4].GetInstance().InitInt(OptionName.ON_PHYSICS_QUERIES_PER_UPDATE, "Physics.QueriesPerUpdate", 15, 1, 100);
            mOptions[5].GetInstance().InitFloat(OptionName.ON_INPUT_MOUSE_RAYCAST_DISTANCE, "Input.MouseRaycastDistance", 100.0f, 0.0f, 0.0f);
            SaveFile();

            // LoadFile();
        }

        private string ReadText()
        {
            return System.IO.File.ReadAllText(Util.GetUserDataDirectory(mWriteLocation));
        }
        private void WriteText(string text)
        {
            System.IO.File.WriteAllText(Util.GetUserDataDirectory(mWriteLocation), text);
        }

        public override void Serialize(BaseStream stream)
        {
            List<NestedObject<Option>> options = new List<NestedObject<Option>>(mOptions);
            stream.Serialize("Options", ref options);
            if(stream.IsReading())
            {
                for(int i = 0; i < mOptions.Length; ++i)
                {
                    if(i < options.Count)
                    {
                        mOptions[i].GetInstance().LoadFrom(options[i].GetInstance());
                    }
                }
            }
        }

        public Option GetOption(OptionName name)
        {
            return mOptions[(int)name].GetInstance();
        }
        public Option GetOption(string name)
        {
            for(int i = 0; i < mOptions.Length; ++i)
            {
                if(mOptions[i].GetName() == name)
                {
                    return mOptions[i].GetInstance();
                }
            }
            return null;
        }

        public void SaveFile()
        {
            TextStream stream = new TextStream();
            stream.SetReadingMode(false);
            stream.StartContext("OptionMgr", "BaseObject");
            Serialize(stream);
            stream.StopContext();
            WriteText(stream.WriteText());
        }

        public void LoadFile()
        {
            TextStream stream = new TextStream();
            string inText = ReadText();
            stream.SetReadingMode(true);
            stream.ParseText(inText);
            stream.NextContext();
            Serialize(stream);
            stream.StopContext();
        }
    }

}

