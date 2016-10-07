using System.Diagnostics;
namespace Brightstone
{
    /** This is a basic clock class. Used to measure time.*/
    public class Timer
    {
        private Stopwatch mStopWatch = new Stopwatch();

        public void Start()
        {
            mStopWatch.Start();
        }

        public void Reset()
        {
            mStopWatch.Stop();
            mStopWatch.Reset();
        }

        public void Stop()
        {
            mStopWatch.Stop();
        }

        public float GetElapsedSeconds()
        {
            return (float)mStopWatch.Elapsed.TotalSeconds;
        }

        public float GetElapsedMilliseconds()
        {
            return (float)mStopWatch.Elapsed.TotalMilliseconds;
        }

        public bool IsRunning()
        {
            return mStopWatch.IsRunning;
        }
    }
}