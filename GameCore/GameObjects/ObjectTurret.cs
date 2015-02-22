#region

using GameCore.Utils;

#endregion

namespace GameCore.GameObjects
{
    public class ObjectTurret : ObjectGame
    {
        public ObjectTurret(ObjcetIds aObjectId) : base(aObjectId)
        {
        }


        /// <summary>
        ///     The orientation of the player given by a vector.
        /// </summary>
        private Vector orientation = new Vector(1.0f, 0.0f);

        /// <summary>
        ///     The orientation of the player given by a vector.
        /// </summary>
        private Vector orientationTower = new Vector(1.0f, 0.0f);

        public Vector Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                Changed = true;
            }
        }

        public Vector OrientationTower
        {
            get { return orientationTower; }
            set
            {
                orientationTower = value;
                Changed = true;
            }
        }
    }
}