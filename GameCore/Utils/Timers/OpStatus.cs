namespace GameCore.Utils.Timers
{
    public class OpStatus
    {
        public const string TEXT_FPS = "FPS: {0:0.0}Hz";
        public const string TEXT_AVR_TIME = "Avr. time: ";
        public const string TEXT_MISSED_FRAMES = "Missed frames: {0}";
        public const string TEXT_MAX_TIME = "Max time: ";
        public const string TEXT_INTERVAL_MAX_TIME = "Int. max time: ";
        public const string TEXT_LOAD = "Load: {0:0.00}%";
        public const string TEXT_Name = "Name: {0}";

        private float ops = 0.0f;
        private float avrOpTime = 0.0f;
        private long missedFrames = 0;
        private float maxTime = 0.0f;
        private float intervalMaxTime = 0.0f;
        private string name;

        /// <summary>
        ///     The work load of the process [%].
        /// </summary>
        public float Load;

        public OpStatus(string aName)
        {
            name = aName;
        }

        public string Name
        {
            get { return name; }
        }

        public float Ops
        {
            get { return ops; }
            set { ops = value; }
        }

        public float AvrOpTime
        {
            get { return avrOpTime; }
            set { avrOpTime = value; }
        }


        public long MissedFrames
        {
            get { return missedFrames; }
            set { missedFrames = value; }
        }

        public float MaxTime
        {
            get { return maxTime; }
            set { maxTime = value; }
        }

        public float IntervalMaxTime
        {
            get { return intervalMaxTime; }
            set { intervalMaxTime = value; }
        }

        public static string GetNiceTime(double timeInSec)
        {
            if (timeInSec >= 1.0)
            {
                return timeInSec.ToString("0.0") + "s";
            }
            else if (timeInSec >= 0.001)
            {
                return (timeInSec*1000.0).ToString("0.0") + "ms";
            }
            else if (timeInSec >= 0.000001)
            {
                return (timeInSec*1000000.0).ToString("0.0") + "us";
            }
            else
                //            else if (timeInSec >= 0.000000001)
            {
                return (timeInSec*1000000000.0).ToString("0.0") + "ns";
            }
        }

        public override string ToString()
        {
            string outStr = name + ":";
            outStr += " FPS: " + ops.ToString("0.0") + "Hz";
            outStr += " load: " + Load.ToString("0.0") + "%";
            outStr += " avr. Time: " + GetNiceTime(avrOpTime);
            outStr += " missed frames: " + missedFrames;
            outStr += " interval max time: " + GetNiceTime(intervalMaxTime);
            outStr += " max time: " + GetNiceTime(maxTime);

            return outStr;
        }
    }
}