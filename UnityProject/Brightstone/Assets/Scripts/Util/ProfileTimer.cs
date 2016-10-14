namespace Brightstone
{
    public class ProfileTimer
    {
        private Timer mTimer = new Timer();

        public void Start()
        {
            mTimer.Start();
        }
        public float Stop(string message)
        {
            mTimer.Stop();
            float seconds = mTimer.GetElapsedSeconds();
            float ms = mTimer.GetElapsedMilliseconds();
            mTimer.Reset();
            if(seconds > 1.0f)
            {
                Log.Test.Info("Operation " + message + " completed in " + seconds + " seconds.");
            }
            else
            {
                Log.Test.Info("Operation " + message + " completed in " + ms +" ms.");
            }
            return ms;
        }
        
    }
}
