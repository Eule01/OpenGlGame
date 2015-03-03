#region

using System.Drawing;

#endregion

namespace GameCore.Utils
{
    /// ARGB components
    /// Note: PixelFormat.Format24bppRgb actually means BGRA format
    /// </summary>
    public class Argb
    {
        public const short AIndex = 3;
        public const short RIndex = 2;
        public const short GIndex = 1;
        public const short BIndex = 0;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Alpha;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Red;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Green;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Blue;

        // Color property
        public Color Color
        {
            get { return Color.FromArgb(Alpha, Red, Green, Blue); }
            set
            {
                Alpha = value.A;
                Red = value.R;
                Green = value.G;
                Blue = value.B;
            }
        }

        // Constructors
        public Argb()
        {
        }

        public Argb(byte alpha, byte red, byte green, byte blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return "A: " + Alpha + "R: " + Red + " G: " + Green + " B: " + Blue;
        }
    }
}