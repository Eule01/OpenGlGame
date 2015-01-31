using System.Diagnostics;

namespace GameCore.Utils.Timers
{
    public class AccurateStopWatch
    {
        public static double OneOverFreq = 1.0/Stopwatch.Frequency;

        public static readonly long Frequency = Stopwatch.Frequency;

        /// <summary>
        /// Gets a time in ticks (convert to seconds by multiplying with OneOverFreq).
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            return Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Returns the time difference in seconds.
        /// </summary>
        /// <param name="time0"></param>
        /// <returns></returns>
        public static double GetDiffSec(long time0)
        {
            return (GetTimeStamp() - time0)*OneOverFreq;
        }
    }
}