using System.Collections.Generic;

namespace Brightstone
{
    public class OptionMgr : BaseObject
    {
        Option[] mOptions = null;
        private string mWriteLocation = "/Game/Options.txt";

        public void Init()
        {
            int size = Util.GetEnumCount<OptionName>();
            mOptions = new Option[size];
            for(int i = 0; i < size; ++i)
            {
                mOptions[i] = new Option();
            }
            mOptions[0].InitBool(OptionName.ON_NONE, "NONE", false);
            mOptions[1].InitBool(OptionName.ON_CLICK_TO_MOVE, "Click To Move", false);
            mOptions[2].InitFloat(OptionName.ON_CAMERA_SENSITIVITY, "Camera Sensitivity", 0.5f, 0.1f, 1.0f);
            mOptions[3].InitEnum(OptionName.ON_GRAPHICS_GENERAL, "Graphics Preset", new[] {
                new EnumDescriptor("Low", 0),
                new EnumDescriptor("Medium", 1),
                new EnumDescriptor("High", 2),
                new EnumDescriptor("Ultra", 3)
            });
            // SaveFile();

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
            List<Option> options = new List<Option>(mOptions);
            stream.Serialize("Options", ref options);
            if(stream.IsReading())
            {
                for(int i = 0; i < mOptions.Length; ++i)
                {
                    if(i < options.Count)
                    {
                        mOptions[i].LoadFrom(options[i]);
                    }
                }
            }
        }

        public Option GetOption(OptionName name)
        {
            return mOptions[(int)name];
        }
        public Option GetOption(string name)
        {
            for(int i = 0; i < mOptions.Length; ++i)
            {
                if(mOptions[i].GetName() == name)
                {
                    return mOptions[i];
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

