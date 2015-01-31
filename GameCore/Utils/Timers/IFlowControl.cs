namespace GameCore.Utils.Timers
{
    public interface IFlowControl
    {
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