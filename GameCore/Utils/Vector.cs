using System;
using System.Drawing;

namespace GameCore.Utils
{
    public struct Vector
    {
        public static readonly Vector Empty = new Vector(0, 0, true);
        public float X;
        public float Y;


        public Vector(float x, float y, bool isEmpty)
        {
            X = x;
            Y = y;
        }

        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector(double x, double y)
        {
            X = (float) x;
            Y = (float) y;
        }

        public float Magnitude
        {
            get { return (float) Math.Sqrt(X*X + Y*Y); }
        }

        /// <summary>
        /// Returns the angle of the vector in RAD.
        /// </summary>
        public float Angle
        {
            get { return (float)Math.Atan2(Y, X); }
        }


        public static Vector FromPoint(Point p)
        {
            return FromPoint(p.X, p.Y);
        }

        public static Vector FromPoint(int x, int y)
        {
            return new Vector(x, y);
        }

        public void Normalize()
        {
            float mag = Magnitude;
            if (mag < 0.0000000001f)
            {
                // TODO return error
                return;
            }
            float oneOverMagnitude = 1.0f/mag;
            X = X*oneOverMagnitude;
            Y = Y*oneOverMagnitude;
        }

        public float DotProduct(Vector vector)
        {
            return X*vector.X + Y*vector.Y;
        }

        public float CrossProduct(Vector vector)
        {
            return X*vector.Y - Y*vector.X;
        }

        /// <summary>
        ///     Returns the distance between this vector end point and another vector end point.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceTo(Vector vector)
        {
            return (float) Math.Sqrt(Math.Pow(vector.X - X, 2) + Math.Pow(vector.Y - Y, 2));
        }

        /// <summary>
        ///     Returns a new perpendicular vector to this one.
        /// </summary>
        /// <returns></returns>
        public Vector Perpendicular()
        {
            return new Vector(-Y, X);
        }


        /// <summary>
        ///     Rotate by a given angle in radians. 1 deg  =1 rad * pi / 180
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(float angle)
        {
            float tx = X;
            X = X*(float) Math.Cos(angle) - Y*(float) Math.Sin(angle);
            Y = tx*(float) Math.Sin(angle) + Y*(float) Math.Cos(angle);
        }

        /// <summary>
        ///     Rotate by an angle the translate by a vector.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="rot"></param>
        public void Transform(Vector trans, float rot)
        {
            Rotate(rot);
            Add(trans);
        }

        public void Add(Vector aVector)
        {
            X += aVector.X;
            Y += aVector.Y;
        }


        public static implicit operator Point(Vector p)
        {
            return new Point((int) p.X, (int) p.Y);
        }

        public static implicit operator Vector(Point p)
        {
            return new Vector(p.X, p.Y);
        }

        public static implicit operator PointF(Vector p)
        {
            return new PointF(p.X, p.Y);
        }

        public static implicit operator Vector(PointF p)
        {
            return new Vector(p.X, p.Y);
        }

        public static implicit operator SizeF(Vector p)
        {
            return new SizeF(p.X, p.Y);
        }

        public static implicit operator Vector(SizeF p)
        {
            return new Vector(p.Width, p.Height);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a.X, -a.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(Vector a, float b)
        {
            return new Vector(a.X*b, a.Y*b);
        }

        public static Vector operator /(Vector a, float b)
        {
            return new Vector(a.X / b, a.Y / b);
        }

        public static Vector operator *(Vector a, int b)
        {
            return new Vector(a.X*b, a.Y*b);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector((float) (a.X*b), (float) (a.Y*b));
        }

        public override bool Equals(object obj)
        {
            var v = (Vector) obj;

            return X == v.X && Y == v.Y;
        }

        public bool Equals(Vector v)
        {
            return X == v.X && Y == v.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override string ToString()
        {
            return "(" + Math.Round(X,3) + ", " + (int)Math.Round(Y,3) + ")";
        }

        public string ToString(bool rounded)
        {
            if (rounded)
            {
                return "(" + (int)Math.Round(X) + ", " + (int)Math.Round(Y) + ")";
            }
            else
            {
                return ToString();
            }
        }

        public void Swap(ref Vector other)
        {
            Vector temp = this;
            this = other;
            other = temp;
        }

        public void Randomise(Vector min, Vector max)
        {
            var ran = (float) RandomNumGen.Ran.NextDouble();
            X = ran*(max.X - min.X) + min.X;
            ran = (float) RandomNumGen.Ran.NextDouble();
            Y = ran*(max.Y - min.Y) + min.Y;
        }
    }
}