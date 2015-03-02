using System.Drawing;
using System.Xml.Serialization;

namespace GameCore.Map
{
    [XmlType(AnonymousType = true)]
    public class MapDetail
    {
        public RectangleF TheBoundingBox;
    }
}