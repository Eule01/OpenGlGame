using System.Runtime.InteropServices;

namespace GameCore.Utils.Timers
{
    public delegate void TimerEventDelegate();

    
    /// <summary>
    /// This is a fast timer that will execute a delegate at a given interval.
    /// </summary>
    public class FastTimer : TimerBase
    {
        private ulong count;
        private uint fastTimerID;

        private TimerEventHandler timerEventHandler;

        /// <summary>
        ///     Interval in ms.
        /// </summary>
        /// <param name="interval"></param>
        public FastTimer(int anIntervalMs, TimerEventDelegate aTimerEventDelegate)
            : base(anIntervalMs, aTimerEventDelegate)
        {
        }

       

        private void GetCapabilities(out uint minimum, out uint maximum)
        {
            TimeCaps timeCaps = new TimeCaps(0, 0);
            uint result = timeGetDevCaps(out timeCaps, Marshal.SizeOf(timeCaps));
            minimum = timeCaps.minimum;
            maximum = timeCaps.maximum;
        }


        public override void Start()
        {
            int myData = 0; // dummy data
            timerEventHandler = tickHandler;
            fastTimerID = timeSetEvent(intervalMs, intervalMs, timerEventHandler, ref myData, 1); // type=periodic
        }

//        public void StartWithoutDelay()
//        {
//            int myData = 0; // dummy data
//            timerEventHandler = tickHandler;
//            fastTimerID = timeSetEvent(intervalMs, intervalMs, timerEventHandler, ref myData, 1); // type=periodic
//            // TODO this is not good.
//            Async.Do(delegate { tickHandler(0, 0, ref myData, 0, 0); });
//        }

        public override void Stop()
        {
            timerEventDelegate = null;
            timeKillEvent(fastTimerID);
            timerEventHandler = null;
        }

        public override void Pause()
        {
            timeKillEvent(fastTimerID);
            timerEventHandler = null;
        }


        private void tickHandler(uint id, uint msg, ref int userCtx, int rsv1, int rsv2)
        {
            count++;
            if (timerEventDelegate != null)
            {
                timerEventDelegate();
            }
        }

        #region MultimediaTimer wrapper

        public delegate void TimerEventHandler(uint id, uint msg, ref int userCtx,
                                               int rsv1, int rsv2);

        /// <summary>
        ///     The timeGetTime function retrieves the system time, in milliseconds.
        ///     The system time is the time elapsed since Windows was started.
        /// </summary>
        /// <returns></returns>
        [DllImport("Winmm.dll")]
        private static extern int timeGetTime();

        /// <summary>
        ///     The timeGetDevCaps function queries the timer device to determine its resolution.
        /// </summary>
        /// <param name="timeCaps"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        private static extern uint timeGetDevCaps(out TimeCaps timeCaps, int size);

        /// <summary>
        ///     The timeSetEvent function starts a specified timer event.
        ///     The multimedia timer runs in its own thread.
        ///     After the event is activated, it calls the specified callback
        ///     function or sets or pulses the specified event object.
        /// </summary>
        /// <param name="msDelay"></param>
        /// <param name="msResolution"></param>
        /// <param name="handler"></param>
        /// <param name="userCtx"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        [DllImport("WinMM.dll", SetLastError = true)]
        private static extern uint timeSetEvent(int msDelay, int msResolution,
                                                TimerEventHandler handler, ref int userCtx, int eventType);

        /// <summary>
        ///     The timeKillEvent function cancels a specified timer event.
        /// </summary>
        /// <param name="timerEventId"></param>
        /// <returns></returns>
        [DllImport("WinMM.dll", SetLastError = true)]
        private static extern uint timeKillEvent(uint timerEventId);

        /// <summary>
        ///     The TIMERCAPS structure contains information about the resolution of the timer.
        /// </summary>
        private struct TimeCaps
        {
            public readonly uint maximum;
            public readonly uint minimum;

            public TimeCaps(uint minimum, uint maximum)
            {
                this.minimum = minimum;
                this.maximum = maximum;
            }
        }

        #endregion
    }
}