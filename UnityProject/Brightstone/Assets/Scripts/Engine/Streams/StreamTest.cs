using System.Collections.Generic;
using System;

namespace Brightstone
{
    [Test]
    public class StreamTest : Test
    {
        private bool mProfile = false;

        private static string[] sRandomNames = null;
        private static Random sRandom = new Random(0xFEFE);

        public class SItem : BaseObject
        {
            public string name;
            public string id;
            public int available;
            public int cost;
            public int rarity;

            public override void Serialize(BaseStream s)
            {
                s.SerializeString("Name", ref name);
                s.SerializeString("Id", ref id);
                s.SerializeInt("Available", ref available);
                s.SerializeInt("Cost", ref cost);
                s.SerializeInt("Rarity", ref rarity);
            }
        }
        public class SItemEx : SItem
        {
            public List<string> compatibility = new List<string>();

            public override void Serialize(BaseStream s)
            {
                base.Serialize(s);
                s.Serialize("Compatibility", ref compatibility);
            }
        }

        public class NestedItem : NestedObject<SItem>
        {
        }


        public class SInventory : BaseComponent
        {
            // Inheritence doesn't work. :/
            public List<NestedItem> items = new List<NestedItem>();

            public override void Serialize(BaseStream stream)
            {
                stream.Serialize("Items", ref items);
            }
        }

        private NestedItem GenerateRandomItem()
        {
            NestedItem nestedItem = new NestedItem();
            SItemEx item = new SItemEx();
            item.name = sRandomNames[sRandom.Next(0, sRandomNames.Length - 1)];
            item.id = Guid.NewGuid().ToString();
            item.rarity = sRandom.Next(0, 6);
            item.available = sRandom.Next(0, 1);
            item.cost = sRandom.Next(25, 600);
            int compat = sRandom.Next(0, 6);
            for(int i = 0; i < compat; ++i)
            {
                item.compatibility.Add(sRandomNames[sRandom.Next(0, sRandomNames.Length - 1)]);
            }
            nestedItem.SetInstance(item);
            return nestedItem;
        }

        public void ParseContext()
        {
            string s = "$[/Engine/StreamTestObj,/Engine/BaseObject]";
            int start = -1; // s.IndexOf('[');
            int end = -1; // s.IndexOf(']');
            int middle = -1; // s.IndexOf(',');

            for (int j = 0; j < s.Length; ++j)
            {
                char bufferChar = s[j];
                if (bufferChar == '[')
                {
                    start = j;
                }
                else if (bufferChar == ']')
                {
                    end = j;
                    break;
                }
                else if (bufferChar == ',')
                {
                    middle = j;
                }
            }
            TEST(start != -1 && end != -1 && middle != -1);

            string name = s.Substring(start + 1, middle - start - 1);
            string parentName = s.Substring(middle + 1, end - middle - 1);
            TEST(name == "/Engine/StreamTestObj");
            TEST(parentName == "/Engine/BaseObject");
        }

        public void ParseContent()
        {
            string line = "ObjectName:String(\"TopLevelString\")";
            string stripped = Util.StrStripWhitespace(line, true);

            int typeStart = -1;
            int dataStart = -1;
            int dataEnd = -1;
            for (int j = 0; j < stripped.Length; ++j)
            {
                char lineChar = stripped[j];
                if (lineChar == ':' && typeStart == -1)
                {
                    typeStart = j;
                }
                else if (lineChar == '(' && dataStart == -1)
                {
                    dataStart = j;
                    break;
                }
            }
            for (int j = stripped.Length - 1; j >= 0; --j)
            {
                char lineChar = stripped[j];
                if (lineChar == ')')
                {
                    dataEnd = j;
                    break;
                }
            }

            string propName = stripped.Substring(0, typeStart);
            string typeName = stripped.Substring(typeStart + 1, dataStart - typeStart - 1);
            string valueStr = stripped.Substring(dataStart + 1, dataEnd - dataStart - 1);
            TEST(propName == "ObjectName");
            TEST(typeName == "String");
            TEST(valueStr == "TopLevelString");
        }

        private string ReadText(string filename)
        {
            return System.IO.File.ReadAllText(Util.GetUserDataDirectory("/" + filename)); 
        }
        private void WriteText(string filename, string text)
        {
            System.IO.File.WriteAllText(Util.GetUserDataDirectory("/" + filename), text);
        }

        public void FunctionalTest()
        {
            BaseStream stream = new TextStream();
            string inText = ReadText("TestInput2.txt");
            stream.SetReadingMode(true);
            stream.ParseText(inText);
            stream.NextContext();
            SInventory inventoryA = new SInventory();
            inventoryA.Serialize(stream);
            
            
            TEST(inventoryA.items.Count == 2);
            if (HasError()) { return; }
            SItemEx itemA = inventoryA.items[0].GetInstance() as SItemEx;
            TEST(itemA != null);
            if (HasError()) { return; }
            TEST(itemA.name == "Lilas");
            TEST(itemA.id == "cc6a710b-ba6e-4ae2-b844-b5bc11b21512");
            TEST(itemA.available == 0);
            TEST(itemA.cost == 419);
            TEST(itemA.rarity == 3);
            TEST(itemA.compatibility.Count == 1);
            if(HasError()) { return; }
            TEST(itemA.compatibility[0] == "Samara");
            SItemEx itemAA = inventoryA.items[1].GetInstance() as SItemEx;
            TEST(itemAA != null);
            if(HasError()) { return; }
            TEST(itemAA.name == "Cally");
            TEST(itemAA.id == "61e7f645-ab03-4abc-8dd8-e3dd6a40c198");
            TEST(itemAA.available == 0);
            TEST(itemAA.cost == 374);
            TEST(itemAA.rarity == 2);
            TEST(itemAA.compatibility.Count == 3);
            TEST(itemAA.compatibility[0] == "Tonye");
            TEST(itemAA.compatibility[1] == "Annabell");
            TEST(itemAA.compatibility[2] == "Max");

            SInventory inventoryB = new SInventory();
            stream.NextContext();
            inventoryB.Serialize(stream);
            
            TEST(inventoryB.items.Count == 1);
            if(HasError()) { return; }
            SItemEx itemB = inventoryB.items[0].GetInstance() as SItemEx;
            TEST(itemA != null);
            if(HasError()) { return; }
            
            TEST(itemB.name == "Greed");
            TEST(itemB.id == "f22d0f78-dbde-4258-9df7-78322dbd2405");
            TEST(itemB.available == 32);
            TEST(itemB.cost == 250);
            TEST(itemB.rarity == 2);
            TEST(itemB.compatibility.Count == 2);
            if (HasError()) { return; }
            TEST(itemB.compatibility[0] == "Simon");
            TEST(itemB.compatibility[1] == "Luna");

            stream = new TextStream();
            stream.SetReadingMode(false);
            stream.StartContext("SInventory", "BaseObject");
            inventoryA.Serialize(stream);
            stream.StopContext();
            stream.StartContext("TestInventory", "SInventory");
            inventoryB.Serialize(stream);
            stream.StopContext();
            string outText = stream.WriteText();
            WriteText("TestOutput2.txt", outText);
            TEST(outText == inText);
            // SInventory inventory = new SInventory();
            // inventory.items.Add(GenerateRandomItem());
            // inventory.items.Add(GenerateRandomItem());
            // stream.SetReadingMode(false);
            // stream.StartContext("SInventory", "BaseObject");
            // inventory.Serialize(stream);
            // stream.StopContext();
            // string outText = stream.WriteText();
            // WriteText("TestOutput2.txt", outText);
        }

        public struct TimeStruct
        {
            public float serialized;
            public float generated;
            public float write;
            public float total;
        }

        public TimeStruct ProfileWrite<T>(string filename, SInventory inventory) where T : BaseStream, new()
        {
            TimeStruct time = new TimeStruct();
            BaseStream stream = new T();

            stream.SetReadingMode(false);
            stream.StartContext("SInventory", "BaseObject");
            ProfileTimer t = new ProfileTimer();
            t.Start();
            inventory.Serialize(stream);
            time.serialized = t.Stop("Serialized...");
            
            stream.StopContext();

            t.Start();
            string text = stream.WriteText();
            time.generated = t.Stop("Generated text...");
            
            t.Start();
            WriteText(filename, text);
            time.write = t.Stop("Wrote text...");
            time.total = time.serialized + time.generated + time.write;
            return time;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "Warning", MessageId = "v")]
        private void UNREFERENCED_VARIABLE<T>(T v)
        {

        }

        private void ProfileRead<T>(string filename) where T : BaseStream, new()
        {
            BaseStream stream = new T();
            string text = ReadText(filename);
            stream.SetReadingMode(true);

            Timer t = new Timer();
            t.Start();
            stream.ParseText(text);
            t.Stop();
            float seconds = t.GetElapsedSeconds();
            float ms = t.GetElapsedMilliseconds();
            if (seconds > 1.0f)
            {
                Log.Test.Info("Parsed text in " + seconds + " seconds.");
            }
            else
            {
                Log.Test.Info("Parsed text in " + ms + " ms.");
            }
            t.Reset();

            t.Start();
            for(int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                UNREFERENCED_VARIABLE(c);
            }
            t.Stop();
            seconds = t.GetElapsedSeconds();
            ms = t.GetElapsedMilliseconds();
            if (seconds > 1.0f)
            {
                Log.Test.Info("Scanned text in " + seconds + " seconds.");
            }
            else
            {
                Log.Test.Info("Scanned text in " + ms + " ms.");
            }
            t.Reset();

            stream.NextContext();
            SInventory inventory = new SInventory();
            t.Start();
            inventory.Serialize(stream);
            t.Stop();
            seconds = t.GetElapsedSeconds();
            ms = t.GetElapsedMilliseconds();
            if (seconds > 1.0f)
            {
                Log.Test.Info("Deserialized in " + seconds + " seconds.");
            }
            else
            {
                Log.Test.Info("Deserialized in " + ms + " ms.");
            }
            stream.StopContext();
        }

        private void PassByValue(string s)
        {

        }

        private void PassByRef(ref string s)
        {

        }

        public class StreamPool
        {
            private StreamProperty[] mPropertyPool = null;
            private StreamObject[] mObjectPool = null;
            private int mObjectIndex = 0;
            private int mPropertyIndex = 0;

            public void InitPool(int objects, int properties)
            {
                mObjectPool = new StreamObject[objects];
                mPropertyPool = new StreamProperty[properties];

                for(int i = 0; i < objects; ++i)
                {
                    mObjectPool[i]=(new StreamObject());
                }
                for(int i = 0; i < properties; ++i)
                {
                    mPropertyPool[i]=(new StreamProperty());
                }
            }

            public StreamProperty AllocProperty()
            {
                if(mPropertyIndex >= mPropertyPool.Length)
                {
                    return null;
                }
                StreamProperty prop = mPropertyPool[mPropertyIndex];
                ++mPropertyIndex;
                return prop;
            }

            public StreamObject AllocObject()
            {
                if(mObjectIndex >= mObjectPool.Length)
                {
                    return null;
                }
                StreamObject obj = mObjectPool[mObjectIndex];
                ++mObjectIndex;
                return obj;
            }

        }

        public void Profile<T>() where T : BaseStream, new()
        {
            const int ITEM_COUNT = 10000;
            ProfileTimer t = new ProfileTimer();
            StreamPool pool = new StreamPool();
            pool.InitPool(ITEM_COUNT, ITEM_COUNT * 10);
            t.Start();
            for(int i = 0; i < ITEM_COUNT; ++i)
            {
                for(int j = 0; j < 10; ++j)
                {
                }
            }
            t.Stop("OVERHEAD");

            BaseStream stream = new T();
            stream.StartContext("TESTA", "TESTB");
            t.Start();
            for(int i = 0; i < ITEM_COUNT; ++i)
            {
                StreamContext context = stream.GetActiveContext();
                if (context == null)
                {
                    return;
                }
                StreamObject top = context.GetTop();
                if (top == null)
                {
                    return;
                }
            }
            t.Stop("MICROSERIALIZE"); 
        }

        class FieldVsPropertyVsMethodPerformanceTest
        {
            public int mFoo = 0;

            public int foo { get { return mFoo; } set { mFoo = value; } }

            public int GetFoo() { return mFoo; }
            public void SetFoo(int value) { mFoo = value; }

        }

        private void AccessTimeTest()
        {
            ProfileTimer t = new ProfileTimer();
            const int ITERATIONS = 100000;
            int target = -1;
            FieldVsPropertyVsMethodPerformanceTest obj = new FieldVsPropertyVsMethodPerformanceTest();

            t.Start();
            for (int i = 0; i < ITERATIONS; ++i)
            {
                target = obj.mFoo;
            }
            t.Stop("Field Get test...");

            t.Start();
            for (int i = 0; i < ITERATIONS; ++i)
            {
                target = obj.foo;
            }
            t.Stop("Property Get test...");

            t.Start();
            for (int i = 0; i < ITERATIONS; ++i)
            {
                target = obj.GetFoo();
            }
            t.Stop("Method Get test...");
            // 
            t.Start();
            for (int i = 0; i < ITERATIONS; ++i)
            {
                obj.mFoo = target;
            }
            t.Stop("Field Set test...");

            t.Start();
            for (int i = 0; i < ITERATIONS; ++i)
            {
                obj.foo = target;
            }
            t.Stop("Property Set test...");

            t.Start();
            for (int i = 0; i < ITERATIONS; ++i)
            {
                obj.SetFoo(target);
            }
            t.Stop("Method Set test...");
        }

        public enum TimeStructType
        {
            GENERATED,
            SERIALIZED,
            WRITE,
            TOTAL
        }

        public void PrintAverage(TimeStructType mode, List<TimeStruct> list, Log log)
        {
            float min = 1000.0f;
            float max = -1000.0f;
            float total = 0.0f;
            for(int i = 0; i < list.Count; ++i)
            {
                float t = 0.0f;
                switch(mode)
                {
                    case TimeStructType.GENERATED: t = list[i].generated; break;
                    case TimeStructType.SERIALIZED: t = list[i].serialized; break;
                    case TimeStructType.WRITE: t = list[i].write; break;
                    case TimeStructType.TOTAL: t = list[i].total; break;
                }
                if(t < min)
                {
                    min = t;
                }
                if(t > max)
                {
                    max = t;
                }
                total += t;
            }
            total = total / list.Count;
            log.Info(mode.ToString() + ":Average=" + total.ToString("n4") + " , Min=" + min.ToString("n4") + " , Max= " + max.ToString("n4"));
        }

        public override void EnableFlags(string flagString)
        {
            mProfile = false;
            if(flagString.Contains("-profile"))
            {
                mProfile = true;
            }
        }

        public override void RunTest()
        {
            sRandomNames = System.IO.File.ReadAllLines(Util.GetUserDataDirectory("/RandomNames.txt"));

            // Log.Test.Info("Starting SubTest \"ParseContext\"");
            // ParseContext();
            // Log.Test.Info("Completed SubTest \"ParseContext\"");
            // Log.Test.Info("Starting SubTest \"ParseContent\"");
            // ParseContent();
            // Log.Test.Info("Completed SubTest \"ParseContent\"");
            ProfileTimer t = new ProfileTimer();
            t.Start();
            FunctionalTest();
            t.Stop("Functional tests...");

            if(mProfile)
            {
                const int ITEM_COUNT = 10000;
                SInventory inventory = new SInventory();
                t.Start();
                for (int i = 0; i < ITEM_COUNT; ++i)
                {
                    inventory.items.Add(GenerateRandomItem());
                }
                t.Stop("Generated items...");

                List<TimeStruct> textTimes = new List<TimeStruct>();
                List<TimeStruct> numTimes = new List<TimeStruct>();

                Log.Test.Info("***Profile TEXTSTREAM***");
                for (int i = 0; i < 200; ++i)
                {
                    TimeStruct time = ProfileWrite<TextStream>("ProfileTextOutput.txt", inventory);
                    textTimes.Add(time);
                }

                // ProfileRead<TextStream>("ProfileTextInput.txt");
                // Profile<TextStream>();
                Log.Test.Info("***Profile NUMERICAL STREAM***");
                for (int i = 0; i < 200; ++i)
                {
                    TimeStruct time = ProfileWrite<NumericalStream>("ProfileNumericalOutput.txt", inventory);
                    numTimes.Add(time);
                }
                // ProfileRead<NumericalStream>("ProfileNumericalInput.txt");
                // Profile<NumericalStream>();

                Log.Test.Info("Sample Iterations=200");

                PrintAverage(TimeStructType.GENERATED, textTimes, Log.Test);
                PrintAverage(TimeStructType.SERIALIZED, textTimes, Log.Test);
                PrintAverage(TimeStructType.WRITE, textTimes, Log.Test);
                PrintAverage(TimeStructType.TOTAL, textTimes, Log.Test);

                PrintAverage(TimeStructType.GENERATED, numTimes, Log.Test);
                PrintAverage(TimeStructType.SERIALIZED, numTimes, Log.Test);
                PrintAverage(TimeStructType.WRITE, numTimes, Log.Test);
                PrintAverage(TimeStructType.TOTAL, numTimes, Log.Test);
            }
            

            // RECENT: 
            // Completed TextStream
            // Added NestedObject<T> which can be used to nest an object type.. So that it can be serialized.
            // 

            // Numerical Stream writes faster than TextStream on average.. and it creates smaller text.
            // At 10000 items over 200 iterations the difference was 39ms on average.
            // Chcked out what hex was like.. And the difference in speed is nothing to go crazy over..
            // But perhaps size?
            // TODO: 
            // - Write profile test that shows Reads ... [ Parsing, Deserializing, Raw-Scanning]
            // - Output file size difference.. 
            // - Post Serialization Compression
            // - [FEATURE] - Add Encryption Option...
            // - Add more safety checks to NumericalStream.. currently just copy pasted TextStream
            // - Add line number debugging to NumericalStream
            // - Figure out a way to add options to test.



        }
    }
}