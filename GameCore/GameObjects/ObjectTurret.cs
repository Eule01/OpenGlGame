#region

using OpenGL;

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
        private Vector3 orientation = new Vector3(1.0f, 0.0f, 0.0f);

        /// <summary>
        ///     The orientation of the player given by a vector.
        /// </summary>
        private Vector3 orientationTower = new Vector3(1.0f, 0.0f, 0.0f);

        public Vector3 Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                Changed = true;
            }
        }

        public Vector3 OrientationTower
        {
            get { return orientationTower; }
            set
            {
                orientationTower = value;
                Changed = true;
            }
        }

        public override void Move(float deltaTime)
        {
            Vector3 tempVect = TheGameStatus.ThePlayer.Location - Location;
            if (tempVect.SquaredLength < 100)
            {
                OrientationTower = tempVect.Normalize();
            }
        }
    }
}