using System.Drawing;

namespace GameCore.UserInterface
{
    /// <summary>
    ///     This class gets the user input and will be used by the engine to update the game status
    /// </summary>
    public class UserInput
    {
        /// <summary>
        ///     Move player forward.
        /// </summary>
        public bool Forward;

        /// <summary>
        ///     Move player backward.
        /// </summary>
        public bool Backward;

        public bool Right;

        public bool Left;



        public PointF MousePosition;
    }
}