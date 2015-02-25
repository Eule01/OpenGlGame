#region

using GameCore.Render.OpenGlHelper;
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
//        private Vector3 orientationTower = new Vector3(1.0f, 0.0f, 0.0f);
        private float orientationTower = 0.0f;

//        public Quaternion TargetOrientationTower =  new Quaternion(0.0f,0.0f,0.0f,1.0f);
        public float TargetOrientationTower = 0.0f;

//        public Quaternion OrientQuaternion = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);


        public Vector3 Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                Changed = true;
            }
        }

        public float OrientationTower
        {
            get { return orientationTower; }
            set
            {
                orientationTower = value;
                Changed = true;
            }
        }

//       public Vector3 OrientationTower
//        {
//            get { return orientationTower; }
//            set
//            {
//                orientationTower = value;
//                Changed = true;
//            }
//        }
//

        public override void Move(float deltaTime)
        {
            Vector3 tempVect = TheGameStatus.ThePlayer.Location - Location;
            if (tempVect.SquaredLength < 100)
            {
                TargetOrientationTower = RotationHelper.AngleFromVectorAroundY(tempVect);

//                TargetOrientationTower = tempVect.CalculateAngle(Vector3.Backward);
                orientationTower = TargetOrientationTower;

                // This is not quite optimal:
//                orientationTower = RotationHelper.RotateTowards(orientationTower, TargetOrientationTower, 0.6 * deltaTime);

//                TargetOrientationTower = RotationHelper.ReverseQuaternion(RotationHelper.GetQuaternionFromDiretion(-tempVect));
//                Quaternion curentQuat = RotationHelper.ReverseQuaternion(RotationHelper.GetQuaternionFromDiretion(-orientation));
//                Quaternion tempQuat = RotationHelper.RotateTowards(OrientQuaternion, TargetOrientationTower, 0.1 * deltaTime);
////                Quaternion tempQuat = RotationHelper.RotateTowards(curentQuat, TargetOrientationTower, 0.314*deltaTime);
//                OrientQuaternion = tempQuat;
//                OrientationTower = RotationHelper.QuaternionToDirectionVector(tempQuat,Vector3.Right);
////                OrientationTower = RotationHelper.QuaternionToDirectionVector(tempQuat,Vector3.Right);
////                OrientationTower = tempVect.Normalize();
            }
        }
    }
}