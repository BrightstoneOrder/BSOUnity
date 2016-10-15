#if UNITY_EDITOR
using UnityEngine;
#else 
using System;
#endif

namespace Brightstone
{
    /** Log class to write debug information. Declare Log types here! */
    public class Log
    {
        /** Add logs here. */
        public static Log Game = new Log("Game");
        public static Log Lib = new Log("Lib");
        public static Log Test = new Log("Test");
        public static Log Phys = new Log("Physics");
        public static Log Sys = new Log("System");

        /** Name of the log.*/
        private string mName = string.Empty;
        /** Flag enabling or disabling logging.*/
        private bool mEnabled = true;

        /** Ctor */
        public Log(string name)
        {
            mName = name;
        }

        /** Returns formatted log prefix string.  */
        private string GetStamp()
        {
#if UNITY_EDITOR
            string timeString = Time.time.ToString("n3");
            return "[" + timeString + "][" + mName + "]:";
#else
            return "[" + mName + "]:";
#endif
        }

        /** Writes an information log if Info log and this log are enabled.*/
        public void Info(string message)
        {
            if (mEnabled)
            {
#if UNITY_EDITOR
                Debug.Log(GetStamp() + message);
#else   
                Console.WriteLine(GetStamp() + message);
#endif
            }
        }

        /** Writes an information log if Warning log and this log are enabled.*/
        public void Warning(string message)
        {
            if (mEnabled)
            {
#if UNITY_EDITOR
                Debug.LogWarning(GetStamp() + message);
#else
                Console.WriteLine(GetStamp() + message);
#endif
            }
        }

        /** Writes an information log if Error log and this log are enabled.*/
        public void Error(string message)
        {
            if (mEnabled)
            {
#if UNITY_EDITOR
                Debug.LogError(GetStamp() + message);
#else
                Console.WriteLine(GetStamp() + message);
#endif
            }
        }

        public string GetName()
        {
            return mName;
        }

        public bool IsEnabled()
        {
            return mEnabled;
        }

        public void SetEnable(bool enable)
        {
            mEnabled = enable;
        }
    }
}