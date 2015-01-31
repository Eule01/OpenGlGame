#region

using System.Threading;
using CodeToast;

#endregion

namespace GameCore.Utils.Timers
{
    public class TickEngineThread : ITickEngine
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
        private Thread workerThread;
        private volatile bool cancel;
        private readonly AutoResetEvent threadWait = new AutoResetEvent(true);
        private bool threadRunning = true;


        public void Setup(string aName, TickEventDelegate aTickEventDelegate, StatusStringDelegate aStatusEventDelegate,
                          int aTimerTickIntervalMs)
        {
            name = aName;
            tickEventDelegate = aTickEventDelegate;
            statusEventDelegate = aStatusEventDelegate;
            timerTickIntervalMs = aTimerTickIntervalMs;
            Init();
        }


        private void Init()
        {
            workerThread = new Thread(TickEngineWorker) {Name = name + "_worker"};

            theTimer = new FastTimer(timerTickIntervalMs, Tick);
            framesPerSecond = new OpsPerSecond(name) {StatusStringDelegate = statusEventDelegate};
            framesPerSecond.Start();
        }


        private void Tick()
        {
            if (!doingWork)
            {
                threadWait.Set();
            }
            else
            {
                framesPerSecond.MissedFrame();
            }
        }

        #region Threading

        private void StartThread()
        {
            if (workerThread == null)
            {
                workerThread = new Thread(TickEngineWorker) {Name = name + "_worker"};
            }
            threadRunning = true;
            workerThread.Start();
        }

        private void StopThread()
        {
            threadRunning = false;
            threadWait.Set();
//            while (doingWork)
//            {
//                Thread.Sleep(10);
//            }
            if (workerThread != null)
            {
                if (workerThread.Join(100))
                {
                    GameCore.TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.Message)
                        {
                            Message = "Stopped " + name + " workerThread"
                        });
                }
                else
                {
                    GameCore.TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.Message)
                        {
                            Message = "Stopping " + name + " workerThread failed, aborting now."
                        });
                    workerThread.Abort();
                }
                workerThread = null;
            }
        }

        private void TickEngineWorker()
        {
            while (threadRunning)
            {
                doingWork = true;
                framesPerSecond.StartOperation();
                if (tickEventDelegate != null)
                {
                    tickEventDelegate();
                }
                framesPerSecond.FinishedOperation();
                doingWork = false;

                threadWait.WaitOne();
            }
        }

        #endregion

        /// <summary>
        ///     Starts the game.
        /// </summary>
        public void Start()
        {
            framesPerSecond.Start();
            StartThread();
            theTimer.Start();
            paused = false;
        }

        public void Close()
        {
            theTimer.Stop();
            StopThread();
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
    }
}