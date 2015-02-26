#region

using GameCore.Render.OpenGlHelper;
using OpenGL;

#endregion

namespace GameCore.GameObjects
{
    public class ObjectEnemy : ObjectGame
    {
        public ObjectEnemy(ObjcetIds aObjectId)
            : base(aObjectId)
        {
        }

        /// <summary>
        ///     The orientation of the player given by a vector.
        /// </summary>
        private Vector3 orientation = new Vector3(1.0f, 0.0f, 0.0f);

        private float moveSpeed = 0.1f;

        public Vector3 Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                Changed = true;
            }
        }

        public override void Move(float deltaTime)
        {
//            if (TheUserInputPlayer.Forward)
//            {
//                Location += Orientation*moveSpeed;
//            }
//            else if (TheUserInputPlayer.Backward)
//            {
//                Location -= Orientation*moveSpeed;
//            }
//            if (TheUserInputPlayer.Right)
//            {
//                Location += RotationHelper.PerpendicularInXZ(Orientation)*moveSpeed;
//            }
//            else if (TheUserInputPlayer.Left)
//            {
//                Location -= RotationHelper.PerpendicularInXZ(Orientation)*moveSpeed;
//            }
//            if (!TheUserInputPlayer.MousePosition.IsEmpty)
//            {
//                Vector3 gameMousePos = new Vector3(TheUserInputPlayer.MousePosition.X, 0.0f,
//                    TheUserInputPlayer.MousePosition.Y);
//                Vector3 playerMouseVec = gameMousePos - Location;
//                playerMouseVec.Normalize();
//                Orientation = playerMouseVec;
//            }
        }
    }
}