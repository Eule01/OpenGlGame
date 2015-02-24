#region

using OpenGL;

#endregion

namespace GameCore.Render.OpenGlHelper
{
    public static class RotationHelper
    {
        /// <summary>
        ///  Gets the quaternion from a direction vector3 by keeping up up.
        /// </summary>
        /// <param name="direction">The direction to have the camera look.</param>
        public static Quaternion GetQuaternionFromDiretion(Vector3 direction)
        {
            if (direction == Vector3.Zero) return Quaternion.Zero;

            Vector3 zvec = -direction.Normalize();
//            Vector3 xvec = -Vector3.Down.Cross(zvec).Normalize();
            Vector3 xvec = Vector3.Up.Cross(zvec).Normalize();
            Vector3 yvec = zvec.Cross(xvec).Normalize();
            return Quaternion.FromAxis(xvec, yvec, zvec);
        }


        public static Quaternion ReverseQuaternion(Quaternion aQuaternion)
        {
            return new Quaternion(aQuaternion.x,aQuaternion.y,aQuaternion.z,-aQuaternion.w);
        }

        public static Vector3 PerpendicularInXZ(Vector3 aVector3)
        {
            return new Vector3(-aVector3.z, aVector3.y, aVector3.x);
        }
    }
}