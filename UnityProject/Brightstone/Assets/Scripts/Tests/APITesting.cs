using UnityEngine;

namespace Brightstone
{
	public class APITesting : BaseComponent 
	{
		private void Start()
        {
            TestMgr.RunTests(string.Empty);
        }

        private void Update()
        {
            
            // T for test?
            if(Input.GetKeyDown(KeyCode.T))
            {
                // Handle it via unity way..
                UtilTest();
                BaseTest();
                Log.Game.Info("Tests Complete");
            }
        }

        private void StartTest(string testname)
        {
            Log.Game.Info("Starting test " + testname);
        }

        private string GetStackTrace()
        {
            string trace = StackTraceUtility.ExtractStackTrace();
            int c = 0;
            for(int i = 0; i < trace.Length; ++i)
            {
                if(trace[i] == '\n')
                {
                    ++c;
                }
                if(c == 2)
                {
                    return trace.Substring(i + 1);
                }
            }
            return trace;
        }

        private void Test(bool condition)
        {
            if(condition == false)
            {
                string trace = GetStackTrace();

                Log.Game.Error("Failed a condition at\n" + trace);
            }
        }

        private void Test(bool condition, string extraMsg)
        {
            if (condition == false)
            {
                string trace = GetStackTrace();
                Log.Game.Error("Failed a condition. " + extraMsg + "\n" + trace);
            }
        }

        private void StringExtTest()
        {
            StartTest("StringExtTest");
            // StringExt: Find, FindLast
            string line0 = "Here is, a is bad sentence.";

            // first, middle, last
            Test(line0.Find('H') == 0);
            Test(line0.Find('.') == 26);
            Test(line0.Find('a') == 9);
            Test(line0.FindLast('H') == 0);
            Test(line0.FindLast('.') == 26);
            Test(line0.FindLast('a') == 15);
            
            Test(line0.Find("He") == 0);
            Test(line0.Find("ence.") == 22, line0.Find("ence.").ToString());
            Test(line0.Find("is") == 5);
            Test(line0.FindLast("He") == 0, line0.FindLast("He").ToString());
            Test(line0.FindLast("ence.") == 22, line0.FindLast("ence.").ToString());
            Test(line0.FindLast("is") == 11, line0.FindLast("is").ToString());


        }

        private void UtilTest()
        {
            StringExtTest();
            
        }

        void ObjectTypeTests()
        {
            // TypeMgr typeMgr = TypeMgr.GetInstance();
            

            StartTest("ObjectTypeTests");
            ObjectType type = new ObjectType();
            type.InternalInit("", "/Weapons/Axe/AxeOfSunder", 116);
            Test(type.GetScope() == "Weapons/Axe");
            Test(type.GetName() == "AxeOfSunder");
            Test(type.GetID() == 116);
            type.InternalInit("", "/Weapons/Sword/SwordOfPain", 32);
            Test(type.GetScope() == "Weapons/Sword");
            Test(type.GetName() == "SwordOfPain");
            Test(type.GetID() == 32);
        }

        private void BaseTest()
        {
            ObjectTypeTests();
        }
	}
}