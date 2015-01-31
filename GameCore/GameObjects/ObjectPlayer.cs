#region

using GameCore.Utils;

#endregion

namespace GameCore.GameObjects
{
    public class ObjectPlayer : GameObject
    {
        public ObjectPlayer(ObjcetIds aObjectId) : base(aObjectId)
        {
        }

        /// <summary>
        ///     The orientation of the player given by a vector.
        /// </summary>
        private Vector orientation = new Vector(1.0f, 0.0f);

        public Vector Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                changed = true;
            }
        }
    }
}