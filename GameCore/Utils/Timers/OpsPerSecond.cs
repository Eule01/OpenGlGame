#region

using System;
using System.Timers;

#endregion

namespace GameCore.Utils.Timers
{
    public delegate void StatusStringDelegate(OpStatus opStatus);

    public class OpsPerSecond
    {
        private long lastStatusTime;
        private long lastOpsCount;
        private long opsCount;

        private readonly Timer timerStatus;
        private int timerStatusIntervall = 1000;

        private long lastOpStartTime;
        private long missedFrames;
        private long sumOpTime;
        private long maxTime;
        private long intervalMaxTime;

        private StatusStringDelegate statusStringDelegate;

        private OpStatus opStatus;
        private string name;

        public OpsPerSecond(string aName)
        {
            name = aName;
            opStatus = new OpStatus(name);
            lastStatusTime = -1;
            //            lastStatusTime = Environment.TickCount;
            lastOpsCount = 0;
            timerStatus = new Timer(timerStatusIntervall);
            timerStatus.Elapsed += OnStatusTimerEvent;
        }

        public void Start()
        {
            lastStatusTime = Environment.TickCount;
            timerStatus.Start();
        }

        public void Stop()
        {
            timerStatus.Stop();
        }


        public void StartOperation()
        {
            lastOpStartTime = AccurateStopWatch.GetTimeStamp();
            //            lastOpStartTime = DateTime.Now.Ticks;
            //            lastOpStartTime = Environment.TickCount;
        }

        public void FinishedOperation()
        {
            long deltTime = AccurateStopWatch.GetTimeStamp() - lastOpStartTime;
            sumOpTime += deltTime;

            if (deltTime > intervalMaxTime)
            {
                intervalMaxTime = deltTime;
            }

            opsCount++;
        }

        public StatusStringDelegate StatusStringDelegate
        {
            get { return statusStringDelegate; }
            set { statusStringDelegate = value; }
        }

        private void OnStatusTimerEvent(object sender, ElapsedEventArgs e)
        {
            long now = AccurateStopWatch.GetTimeStamp();
            if (lastStatusTime > 0)
            {
                long timeSpan = now - lastStatusTime;
                float deltOpCount = (opsCount - lastOpsCount);
                float frameRate = (float) (deltOpCount/(timeSpan*AccurateStopWatch.OneOverFreq));
                if (intervalMaxTime > maxTime)
                {
                    maxTime = intervalMaxTime;
                }

                opStatus.Ops = frameRate;
                opStatus.Load = (float)sumOpTime /timeSpan * 100.0f;

                opStatus.MissedFrames = missedFrames;
                opStatus.AvrOpTime = (float) (sumOpTime/deltOpCount*AccurateStopWatch.OneOverFreq);
                opStatus.MaxTime = (float) (maxTime*AccurateStopWatch.OneOverFreq);
                opStatus.IntervalMaxTime = (float) (intervalMaxTime*AccurateStopWatch.OneOverFreq);
                // Report status.
                if (statusStringDelegate != null)
                {
                    statusStringDelegate(opStatus);
                }
            }
            lastStatusTime = now;
            lastOpsCount = opsCount;
            sumOpTime = 0;
            intervalMaxTime = 0;
        }

        public void MissedFrame()
        {
            missedFrames++;
        }
    }
}