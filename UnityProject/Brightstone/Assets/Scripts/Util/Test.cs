using System;
using System.Diagnostics; 

namespace Brightstone
{
    public class Test
    {
        private string mTestName = string.Empty;
        private int mErrors = 0;

        

        /** Call upon this method to test a condition.*/
        protected void TEST(bool result)
        {

            if(!result)
            {
                ++mErrors;
#if UNITY_EDITOR
                Log.Test.Error("An error has occured during test " + GetTestName());
#else 
                StackTrace stackTrace = new StackTrace(true);
                string fileInfo = "Line: " + stackTrace.GetFrame(1).GetFileLineNumber().ToString() + " File: " + stackTrace.GetFrame(1).GetFileName();
                
                Log.Test.Error("An error has occured during test " + GetTestName() + " \n" + fileInfo);
#endif               
            }
            // TODO: Output an error.. Do a stack trace..
        }

        /** Override this method to hook in your test!*/
        public virtual void RunTest()
        {

        }

        public virtual void EnableFlags(string flagString)
        {

        }

        public void SetTestName(string name) { mTestName = name; }
        public string GetTestName() { return mTestName; }
        public bool HasError() { return mErrors != 0; }
        public int GetErrors() { return mErrors; }
    }
}
