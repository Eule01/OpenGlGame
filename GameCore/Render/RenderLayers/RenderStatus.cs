using OpenGL;

namespace GameCore.Render.RenderLayers
{
    public class RenderStatus
    {
        /// <summary>
        /// The width of the display window.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the display window.
        /// </summary>
        public int Height;

        /// <summary>
        ///     The near clipping distance.
        /// </summary>
        public float ZNear = 0.1f;

        /// <summary>
        ///     The far clipping distance.
        /// </summary>
        public float ZFar = 1000f;

        /// <summary>
        ///     Field of view of the camera
        /// </summary>
        public float Fov = 0.45f;

        /// <summary>
        /// Render fullscreen.
        /// </summary>
        public bool Fullscreen;


        public override string ToString()
        {
            string outStr = "";
            outStr += "Width: " + Width;
            outStr += " Height: " + Height;
            outStr += " ZNear: " + ZNear;
            outStr += " ZFar: " + ZFar;
            outStr += " Fov: " + Fov;
            outStr += " Fullscreen: " + Fullscreen;
            return outStr;
        }
    }
}