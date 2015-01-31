using System.Drawing;

namespace GameCore.Map
{
    public class TileType
    {
        private static readonly Color defaultColor = Color.Gray;

        private string name;

        public Color Color = defaultColor;

        public TileType(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }
}