namespace GameCore.Utils.Timers
{
    public delegate void TickEventDelegate();


    public class TickEngine : ITickEngine
    {
        private string name;

        /// <summary>
        ///     This is the timer that takes care of all the game action.
        /// </summary>
        private TimerBase theTimer;

        /// <summary>
        ///     The gamer time interval in milliseconds.
        /// </summary>
        private int timerTickIntervalMs = 10;

        private bool paused = true;

        /// <summary>
        ///     Measures the performance and reports it
        /// </summary>
        private OpsPerSecond framesPerSecond;

        /// <summary>
        ///     This is where all the work is done.
        /// </summary>
        private TickEventDelegate tickEventDelegate;

        /// <summary>
        ///     This reports the execution status.
        /// </summary>
        private StatusStringDelegate statusEventDelegate;

        private bool doingWork = false;

        public void Setup(string aName, TickEventDelegate aTickEventDelegate, StatusStringDelegate aStatusEventDelegate,
                          int aTimerTickIntervalMs)
        {
            name = aName;
            tickEventDelegate = aTickEventDelegate;
            statusEventDelegate = aStatusEventDelegate;
            timerTickIntervalMs = aTimerTickIntervalMs;
            Init();
        }

        /// <summary>
        ///     Initialise the game engine.
        /// </summary>
        private void Init()
        {
            theTimer = new FastTimer(timerTickIntervalMs, Tick);
            framesPerSecond = new OpsPerSecond(name) {StatusStringDelegate = statusEventDelegate};
            framesPerSecond.Start();
        }

        #region Timer

        /// <summary>
        ///     Starts the game.
        /// </summary>
        public void Start()
        {
            framesPerSecond.Start();
            theTimer.Start();
            paused = false;
        }

        /// <summary>
        ///     Shuts down the game.
        /// </summary>
        public virtual void Close()
        {
            theTimer.Stop();
            paused = true;

            framesPerSecond.Stop();
            framesPerSecond.StatusStringDelegate = null;
        }

        /// <summary>
        ///     Pauses the game.
        /// </summary>
        public void Pause()
        {
            theTimer.Pause();
            paused = true;
        }

        /// <summary>
        ///     Resumes the game.
        /// </summary>
        public void Resume()
        {
            theTimer.Start();
            paused = false;
        }

        /// <summary>
        ///     Here all the game action is computed. This is called every timerTickIntervalMs
        /// </summary>
        private void Tick()
        {
            if (!doingWork)
            {
                doingWork = true;
                framesPerSecond.StartOperation();
                if (tickEventDelegate != null)
                {
                    tickEventDelegate();
                }
                framesPerSecond.FinishedOperation();
                doingWork = false;
            }
            else
            {
                framesPerSecond.MissedFrame();
            }
        }

        #endregion
    }
}