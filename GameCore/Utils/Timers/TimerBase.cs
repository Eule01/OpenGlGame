namespace GameCore.Utils.Timers
{
    public abstract class TimerBase
    {
        protected int intervalMs = 1000;
        protected TimerEventDelegate timerEventDelegate;

        /// <summary>
        ///     Interval in ms.
        /// </summary>
        /// <param name="interval"></param>
        public TimerBase(int anIntervalMs, TimerEventDelegate aTimerEventDelegate)
        {
            intervalMs = anIntervalMs;
            timerEventDelegate = aTimerEventDelegate;
        }

        public abstract void Start();
        public abstract void Stop();
        public abstract void Pause();

    }
}