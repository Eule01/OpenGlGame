namespace GameCore.Utils.Timers
{
    public interface ITickEngine : IFlowControl
    {
        void Setup(string aName, TickEventDelegate aTickEventDelegate, StatusStringDelegate aStatusEventDelegate,
                   int aTimerTickIntervalMs);

        /// <summary>
        ///     Starts the game.
        /// </summary>
        void Start();

        /// <summary>
        ///     Shuts down the game.
        /// </summary>
        void Close();

        /// <summary>
        ///     Pauses the game.
        /// </summary>
        void Pause();

        /// <summary>
        ///     Resumes the game.
        /// </summary>
        void Resume();
    }
}