#region

using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.GameObjects
{
    public class ObjectPlayer : ObjectGame
    {
        public ObjectPlayer(ObjcetIds aObjectId) : base(aObjectId)
        {
        }

        /// <summary>
        ///     The orientation of the player given by a vector.
        /// </summary>
        private Vector3 orientation = new Vector3(1.0f, 0.0f, 0.0f);

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
            if (TheUserInputPlayer.Forward)
            {
                Location += Orientation * 0.1f;
            }
            else if (TheUserInputPlayer.Backward)
            {
                Location -= Orientation * 0.1f;
            }
            if (TheUserInputPlayer.Right)
            {
                Location += PerpendicularInXZ(Orientation) * 0.1f;
            }
            else if (TheUserInputPlayer.Left)
            {
                Location -= PerpendicularInXZ(Orientation) * 0.1f;
            }
            if (!TheUserInputPlayer.MousePosition.IsEmpty)
            {
                Vector3 gameMousePos = new Vector3(TheUserInputPlayer.MousePosition.X, 0.0f, TheUserInputPlayer.MousePosition.Y);
                Vector3 playerMouseVec = gameMousePos - Location;
                playerMouseVec.Normalize();
                Orientation = playerMouseVec;
            }
        }

        private Vector3 PerpendicularInXZ(Vector3 aVector3)
        {
            return new Vector3(-aVector3.z,0.0f,aVector3.x);
        }
    }
}