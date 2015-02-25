#region

using System;
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


        public static Vector3 QuaternionToDirectionVector(Quaternion aQuaternion, Vector3 aRefVector3)
        {
            return aQuaternion*aRefVector3;
        }

        /// <summary>
        /// http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-17-quaternions/
        /// 
        /// Use like this:
        /// CurrentOrientation = RotateTowards(CurrentOrientation, TargetOrientation, 3.14f * deltaTime );
        /// </summary>
        /// <param name="currentOrientation"></param>
        /// <param name="targetOrientation"></param>
        /// <param name="maxAngle"></param>
        /// <returns></returns>
        public static Quaternion RotateTowards(Quaternion currentOrientation, Quaternion targetOrientation, double maxAngle)
        {

            if (maxAngle < 0.001f)
            {
                // No rotation allowed. Prevent dividing by 0 later.
                return currentOrientation;
            }

//            float cosTheta = dot(currentOrientation, targetOrientation);
            float cosTheta = currentOrientation.Dot(targetOrientation);

            // currentOrientation and targetOrientation are already equal.
            // Force targetOrientation just to be sure
            if (cosTheta > 0.9999f)
            {
                return targetOrientation;
            }

            // Avoid taking the long path around the sphere
            if (cosTheta < 0)
            {
                currentOrientation = currentOrientation * -1.0f;
                cosTheta *= -1.0f;
            }

            double angle = Math.Acos(cosTheta);

            // If there is only a 2° difference, and we are allowed 5°,
            // then we arrived.
            if (angle < maxAngle)
            {
                return targetOrientation;
            }

            double fT = maxAngle / angle;
            angle = maxAngle;

            Quaternion res = ((float)(Math.Sin((1.0f - fT) * angle)) * currentOrientation + ((float)Math.Sin(fT * angle)) * targetOrientation) / (float) Math.Sin(angle);
//            res = normalize(res);
            res = res.Normalize();
            return res;

        }

        /// <summary>
        /// Angles are between 0 and pi.
        /// </summary>
        /// <param name="currentOrientation"></param>
        /// <param name="targetOrientationTower"></param>
        /// <param name="maxAngle"></param>
        /// <returns></returns>
        public static float RotateTowards(float currentOrientation, float targetOrientationTower, double maxAngle)
        {
            double deltaAngle = targetOrientationTower - currentOrientation;
            deltaAngle = deltaAngle%(Math.PI*2);
            if (Math.Abs(deltaAngle) < maxAngle)
            {
                return targetOrientationTower;
            }

            int deltaSign = Math.Sign(deltaAngle);


            if (deltaAngle > Math.PI)
            {
                deltaAngle = -(Math.PI * 2 - deltaAngle);
            }
            else if (deltaAngle < Math.PI)
            {
                deltaAngle = (Math.PI * 2 + deltaAngle);                
            }
            if (Math.Abs(deltaAngle) < maxAngle)
            {
                return targetOrientationTower;
            }

            if (deltaAngle < 0)
            {
                deltaAngle = - Math.Max(maxAngle, deltaAngle);
            }
            else
            {
                deltaAngle =  Math.Min(maxAngle, deltaAngle);
            }

            return (float) (currentOrientation + deltaAngle);
        }

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void Normalize(ref Vector3 vec, out Vector3 result)
        {
            float scale = 1.0f / vec.Length;
            result.x = vec.x * scale;
            result.y = vec.y * scale;
            result.z = vec.z * scale;
        }

        public static float AngleFromVectorAroundY(Vector3 tempVect)
        {
            Quaternion tempQuat =GetQuaternionFromDiretion(-tempVect);
            Vector4 tempAnglVec = tempQuat.ToAxisAngle();
            float angle = tempAnglVec.w;
            if (tempAnglVec.y < 0)
            {
                angle = -angle;
            }
            return angle;
        }


        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eye">Eye (camera) position in world space</param>
        /// <param name="target">Target position in world space</param>
        /// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A Matrix4 that transforms world space to camera space</returns>
        public static Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 z = (eye - target).Normalize();
            Vector3 x =(Vector3.Cross(up, z)).Normalize();
            Vector3 y = (Vector3.Cross(z, x)).Normalize();

            Vector4 Row0;
            Vector4 Row1;
            Vector4 Row2;
            Vector4 Row3;

            Row0.x = x.x;
            Row0.y = y.x;
            Row0.z = z.x;
            Row0.w = 0;
            Row1.x = x.y;
            Row1.y = y.y;
            Row1.z = z.y;
            Row1.w = 0;
            Row2.x = x.z;
            Row2.y = y.z;
            Row2.z = z.z;
            Row2.w = 0;
            Row3.x = -((x.x * eye.x) + (x.y * eye.y) + (x.z * eye.z));
            Row3.y = -((y.x * eye.x) + (y.y * eye.y) + (y.z * eye.z));
            Row3.z = -((z.x * eye.x) + (z.y * eye.y) + (z.z * eye.z));
            Row3.w = 1;
            Matrix4 result = new Matrix4(Row0,Row1,Row2,Row3);

            return result;
        }
    }
}