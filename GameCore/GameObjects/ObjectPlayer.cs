#region

using GameCore.Utils;

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
        private Vector orientation = new Vector(1.0f, 0.0f);

        public Vector Orientation
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
                Location -= (Orientation.Perpendicular()) * 0.1f;
            }
            else if (TheUserInputPlayer.Left)
            {
                Location += (Orientation.Perpendicular()) * 0.1f;
            }
            if (!TheUserInputPlayer.MousePosition.IsEmpty)
            {
                Vector gameMousePos = TheUserInputPlayer.MousePosition;
                Vector playerMouseVec = gameMousePos - Location;
                playerMouseVec.Normalize();
                Orientation = playerMouseVec;
            }
        }
    }
}