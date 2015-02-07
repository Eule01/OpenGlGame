#region

using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Render.RenderObjects;
using OpenGL;

#endregion

namespace GameCore.Render.OpenGlHelper
{
    /**
         * <summary>
         * A class containing all the necessary data for a mesh: Points, normal vectors, UV coordinates,
         * and indices into each.
         * Regardless of how the mesh file represents geometry, this is what we load it into,
         * because this is most similar to how OpenGL represents geometry.
         * We store data as arrays of vertices, UV coordinates and normals, and then a list of Triangle
         * structures.  A Triangle is a struct which contains integer offsets into the vertex/normal/texcoord
         * arrays to define a face.
         * </summary>
         */
    // XXX: Sources: http://www.opentk.com/files/ObjMeshLoader.cs, OOGL (MS3D), Icarus (Colladia)
    public class MeshData
    {
        public Vector3[] Vertices;

        /// <summary>
        ///     Uv coordinates
        /// </summary>
        public Vector2[] TexCoords;

        public Vector3[] Normals;
        public Tri[] Tris;

        /// <summary>
        ///     Creates a new MeshData object
        /// </summary>
        /// <param name="vert">
        ///     A <see cref="Vector3[]" />
        /// </param>
        /// <param name="norm">
        ///     A <see cref="Vector3[]" />
        /// </param>
        /// <param name="tex">
        ///     A <see cref="Vector2[]" />
        /// </param>
        /// <param name="tri">
        ///     A <see cref="Tri[]" />
        /// </param>
        public MeshData(Vector3[] vert, Vector3[] norm, Vector2[] tex, Tri[] tri)
        {
            Vertices = vert;
            TexCoords = tex;
            Normals = norm;
            Tris = tri;

            Verify();
        }

        /// <summary>
        ///     Returns an array containing the coordinates of all the
        ///     <value>Vertices</value>
        ///     .
        ///     So {<1,1,1>, <2,2,2>} will turn into {1,1,1,2,2,2}
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Double[]" />
        /// </returns>
        public double[] VertexArray()
        {
            double[] verts = new double[Vertices.Length*3];
            for (int i = 0; i < Vertices.Length; i++)
            {
                verts[i*3] = Vertices[i].x;
                verts[i*3 + 1] = Vertices[i].y;
                verts[i*3 + 2] = Vertices[i].z;
            }

            return verts;
        }

        /// <summary>
        ///     Returns an array containing the coordinates of the
        ///     <value>Normals
        ///         <,value>, similar to VertexArray.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Double[]" />
        /// </returns>
        public double[] NormalArray()
        {
            double[] norms = new double[Normals.Length*3];
            for (int i = 0; i < Normals.Length; i++)
            {
                norms[i*3] = Normals[i].x;
                norms[i*3 + 1] = Normals[i].y;
                norms[i*3 + 2] = Normals[i].z;
            }

            return norms;
        }

        /// <summary>
        ///     Returns an array containing the coordinates of the
        ///     <value>TexCoords
        ///         <value>, similar to VertexArray.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Double[]" />
        /// </returns>
        public double[] TexcoordArray()
        {
            double[] tcs = new double[TexCoords.Length*2];
            for (int i = 0; i < TexCoords.Length; i++)
            {
                tcs[i*3] = TexCoords[i].x;
                tcs[i*3 + 1] = TexCoords[i].y;
            }

            return tcs;
        }

        /*
            public void IndexArrays(out int[] verts, out int[] norms, out int[] texcoords) {
                List<int> v = new List<int>();
                List<int> n = new List<int>();
                List<int> t = new List<int>();
                foreach(Face f in Faces) {
                    foreach(Point p in f.Points) {
                        v.Add(p.VertexIndex);
                        n.Add(p.NormalIndex);
                        t.Add(p.TexCoordIndex);
                    }
                }
                verts = v.ToArray();
                norms = n.ToArray();
                texcoords = t.ToArray();
            }
            */


        /// <summary>
        ///     Turns the Triangles into an array of Points.
        /// </summary>
        /// <returns>
        ///     A <see cref="Point[]" />
        /// </returns>
        protected Point[] Points()
        {
            List<Point> points = new List<Point>();
            foreach (Tri t in Tris)
            {
                points.Add(t.P1);
                points.Add(t.P2);
                points.Add(t.P3);
            }
            return points.ToArray();
        }

        // OpenGL's vertex buffers use the same index to refer to vertices, normals and floats,
        // and just duplicate data as necessary.  So, we do the same.
        // XXX: This... may or may not be correct, and is certainly not efficient.
        // But when in doubt, use brute force.
        public void OpenGLArrays(out float[] verts, out float[] norms, out float[] texcoords, out uint[] indices)
        {
            Point[] points = Points();
            verts = new float[points.Length*3];
            norms = new float[points.Length*3];
            texcoords = new float[points.Length*2];
            indices = new uint[points.Length];

            for (uint i = 0; i < points.Length; i++)
            {
                Point p = points[i];
                verts[i*3] = (float) Vertices[p.VertexIndex].x;
                verts[i*3 + 1] = (float) Vertices[p.VertexIndex].y;
                verts[i*3 + 2] = (float) Vertices[p.VertexIndex].z;

                norms[i*3] = (float) Normals[p.NormalIndex].x;
                norms[i*3 + 1] = (float) Normals[p.NormalIndex].y;
                norms[i*3 + 2] = (float) Normals[p.NormalIndex].z;

                texcoords[i*2] = (float) TexCoords[p.TexCoordIndex].x;
                texcoords[i*2 + 1] = (float) TexCoords[p.TexCoordIndex].y;

                indices[i] = i;
            }
        }


        // OpenGL's vertex buffers use the same index to refer to vertices, normals and floats,
        // and just duplicate data as necessary.  So, we do the same.
        // XXX: This... may or may not be correct, and is certainly not efficient.
        // But when in doubt, use brute force.
        public void OpenGLArrays(out Vector3[] verts, out Vector3[] norms, out Vector2[] texcoords, out int[] indices)
        {
            Point[] points = Points();
            verts = new Vector3[points.Length];
            norms = new Vector3[points.Length];
            texcoords = new Vector2[points.Length];
            indices = new int[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                Point p = points[i];
                verts[i] = Vertices[p.VertexIndex];

                norms[i] = Normals[p.NormalIndex];

                texcoords[i] = TexCoords[p.TexCoordIndex];

                indices[i] = i;
            }
        }


        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("Vertices:");
            foreach (Vector3 v in Vertices)
            {
                s.AppendLine(v.ToString());
            }

            s.AppendLine("Normals:");
            foreach (Vector3 n in Normals)
            {
                s.AppendLine(n.ToString());
            }
            s.AppendLine("TexCoords:");
            foreach (Vector2 t in TexCoords)
            {
                s.AppendLine(t.ToString());
            }
            s.AppendLine("Tris:");
            foreach (Tri t in Tris)
            {
                s.AppendLine(t.ToString());
            }
            return s.ToString();
        }

        // XXX: Might technically be incorrect, since a (malformed) file could have vertices
        // that aren't actually in any face.
        // XXX: Don't take the names of the out parameters too literally...
        public void Dimensions(out double width, out double length, out double height)
        {
            double maxx, minx, maxy, miny, maxz, minz;
            maxx = maxy = maxz = minx = miny = minz = 0;
            foreach (Vector3 vert in Vertices)
            {
                if (vert.x > maxx) maxx = vert.x;
                if (vert.y > maxy) maxy = vert.y;
                if (vert.z > maxz) maxz = vert.z;
                if (vert.x < minx) minx = vert.x;
                if (vert.y < miny) miny = vert.y;
                if (vert.z < minz) minz = vert.z;
            }
            width = maxx - minx;
            length = maxy - miny;
            height = maxz - minz;
        }

        /// <summary>
        ///     Does some simple sanity checking to make sure that the offsets of the Triangles
        ///     actually refer to real points.  Throws an
        ///     <exception cref="IndexOutOfRangeException">IndexOutOfRangeException</exception> if not.
        /// </summary>
        public void Verify()
        {
            foreach (Tri t in Tris)
            {
                foreach (Point p in t.Points())
                {
                    if (p.VertexIndex >= Vertices.Length)
                    {
                        string message = String.Format("VertexIndex {0} >= length of vertices {1}", p.VertexIndex,
                            Vertices.Length);
                        throw new IndexOutOfRangeException(message);
                    }
                    if (p.NormalIndex >= Normals.Length)
                    {
                        string message = String.Format("NormalIndex {0} >= number of normals {1}", p.NormalIndex,
                            Normals.Length);
                        throw new IndexOutOfRangeException(message);
                    }
                    if (p.TexCoordIndex > TexCoords.Length)
                    {
                        string message = String.Format("TexCoordIndex {0} > number of texcoords {1}", p.TexCoordIndex,
                            TexCoords.Length);
                        throw new IndexOutOfRangeException(message);
                    }
                }
            }
        }

        public ObjObject ToObjObject()
        {
            Vector3[] verts;
            Vector3[] norms;
            Vector2[] texcoords;
            int[] indices;
            OpenGLArrays(out verts, out norms, out texcoords, out indices);

            ObjObject tempObjObject = new ObjObject(new ObjectVectors()
            {
                Vertex = verts,
                Uvs = texcoords,
                ElementData = indices,
                normalData = norms
            });
            return tempObjObject;
        }
    }

    public struct Point
    {
        public int VertexIndex;
        public int NormalIndex;

        /// <summary>
        ///     Also the UV
        /// </summary>
        public int TexCoordIndex;

        public Point(int v, int n, int t)
        {
            VertexIndex = v;
            NormalIndex = n;
            TexCoordIndex = t;
        }

        public override string ToString()
        {
            return String.Format("Point: {0},{1},{2}", VertexIndex, NormalIndex, TexCoordIndex);
        }
    }

    public class Tri
    {
        public Point P1, P2, P3;

        public Tri()
        {
            P1 = new Point();
            P2 = new Point();
            P3 = new Point();
        }

        public Tri(Point a, Point b, Point c)
        {
            P1 = a;
            P2 = b;
            P3 = c;
        }

        public Point[] Points()
        {
            return new Point[3] {P1, P2, P3};
        }

        public override string ToString()
        {
            return String.Format("Tri: {0}, {1}, {2}", P1, P2, P3);
        }
    }
}