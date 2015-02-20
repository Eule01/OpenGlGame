#region

using OpenGL;

#endregion

namespace GameCore
{
    public class Environment
    {
        /// <summary>
        ///     The ambient lighting level [0-1]
        /// </summary>
        public float LightAmbient = 0.3f;

        /// <summary>
        ///     Use light source.
        /// </summary>
        public bool Lighting = true;

        /// <summary>
        ///     The direction of the lighting source.
        /// </summary>
        public Vector3 LightDirection = new Vector3(10, 10, 10).Normalize();


        /// <summary>
        ///     Does the light source move.
        /// </summary>
        public bool LightMove = true;

        public override string ToString()
        {
            string outStr = "";
            outStr += "LightAmbient: " + LightAmbient;
            outStr += " Lighting: " + Lighting;
            outStr += " LightDirection: " + LightDirection;
            outStr += " LightMove: " + LightMove;
            return outStr;
        }
    }
}