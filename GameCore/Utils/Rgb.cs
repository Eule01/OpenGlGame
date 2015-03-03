using System.Drawing;

namespace GameCore.Utils
{
    /// RGB components
    /// Note: PixelFormat.Format24bppRgb actually means BGR format
    /// </summary>
    public class Rgb
    {
        public const short RIndex = 2;
        public const short GIndex = 1;
        public const short BIndex = 0;

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
            get { return Color.FromArgb(Red, Green, Blue); }
            set
            {
                Red = value.R;
                Green = value.G;
                Blue = value.B;
            }
        }

        // Constructors
        public Rgb()
        {
        }

        public Rgb(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return "R: " + Red + " G: " + Green + " B: " + Blue;
        }
    }
}