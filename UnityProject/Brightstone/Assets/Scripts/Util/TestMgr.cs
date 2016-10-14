using System.Collections.Generic;
using System;

namespace Brightstone
{
    public class TestMgr
    {
        public static void RunTests(string args)
        {
            List<Type> types = Util.GetAttributeTypes(typeof(TestAttribute));
            for(int i = 0; i < types.Count; ++i)
            {
                Type type = types[i];
                Test instance = Activator.CreateInstance(type) as Test;
                if(instance != null)
                {
                    instance.SetTestName(type.Name);
                    instance.EnableFlags(args);
                    Log.Test.Info("Running test " + instance.GetTestName() + "...");
                    instance.RunTest();
                    Log.Test.Info("Test " + instance.GetTestName() + " completed with Errors=" + instance.GetErrors().ToString() + ".");
                }
            }
        }
    }
}
