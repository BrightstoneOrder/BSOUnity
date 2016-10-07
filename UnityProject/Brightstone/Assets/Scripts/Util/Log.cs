using UnityEngine;

namespace Brightstone
{
    /** Log class to write debug information. Declare Log types here! */
	public class Log 
	{
        /** Add logs here. */
        public static Log Game = new Log("Game");

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
            string timeString = Time.time.ToString("n3");
            return "[" + timeString + "][" + mName + "]:"; 
        }

        /** Writes an information log if Info log and this log are enabled.*/
        public void Info(string message)
        {
            if(mEnabled)
            {
                Debug.Log(GetStamp() + message);
            }
        }

        /** Writes an information log if Warning log and this log are enabled.*/
        public void Warning(string message)
        {
            if (mEnabled)
            {
                Debug.LogWarning(GetStamp() + message);
            }
        }

        /** Writes an information log if Error log and this log are enabled.*/
        public void Error(string message)
        {
            if (mEnabled)
            {
                Debug.LogError(GetStamp() + message);
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