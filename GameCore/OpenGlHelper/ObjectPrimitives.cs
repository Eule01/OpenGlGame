#region

using System.Collections.Generic;
using OpenGL;

#endregion

namespace GameCore.OpenGlHelper
{
    public static class ObjectPrimitives
    {
        /// <returns></returns>
        public static ObjectVectors CreateCube( Vector3 min, Vector3 max, bool createUv)
        {
            ObjectVectors tempObjectVectors = new ObjectVectors();
            tempObjectVectors.Vertex = new[]
                {
                    new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z)
                    , new Vector3(max.x, max.y, max.z), // top
                    new Vector3(max.x, min.y, max.z), new Vector3(min.x, min.y, max.z), new Vector3(min.x, min.y, min.z)
                    , new Vector3(max.x, min.y, min.z), // bottom
                    new Vector3(max.x, max.y, max.z), new Vector3(min.x, max.y, max.z), new Vector3(min.x, min.y, max.z)
                    , new Vector3(max.x, min.y, max.z), // front face
                    new Vector3(max.x, min.y, min.z), new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z)
                    , new Vector3(max.x, max.y, min.z), // back face
                    new Vector3(min.x, max.y, max.z), new Vector3(min.x, max.y, min.z), new Vector3(min.x, min.y, min.z)
                    , new Vector3(min.x, min.y, max.z), // left
                    new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z), new Vector3(max.x, min.y, max.z)
                    , new Vector3(max.x, min.y, min.z) // right
                };

            List<int> triangles = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                triangles.Add(i*4);
                triangles.Add(i*4 + 1);
                triangles.Add(i*4 + 2);
                triangles.Add(i*4);
                triangles.Add(i*4 + 2);
                triangles.Add(i*4 + 3);
            }

            tempObjectVectors.ElementData = triangles.ToArray();

            if (createUv)
            {
                tempObjectVectors.Uvs = new Vector2[]
                    {
                        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
                    };
            }

            return tempObjectVectors;
        }

        /// <summary>
        /// This uses less vertices but uv can not be added.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static ObjectVectors CreateCube(Vector3 min, Vector3 max)
        {
            ObjectVectors tempObjectVectors = new ObjectVectors();
            tempObjectVectors.Vertex = new[]
                {
                    new Vector3(min.x, min.y, max.z), // 0
                    new Vector3(max.x, min.y, max.z), // 1
                    new Vector3(min.x, max.y, max.z), // 2
                    new Vector3(max.x, max.y, max.z), // 3
                    new Vector3(max.x, min.y, min.z), // 4
                    new Vector3(max.x, max.y, min.z), // 5
                    new Vector3(min.x, max.y, min.z), // 6
                    new Vector3(min.x, min.y, min.z) // 7
                };

            tempObjectVectors.ElementData = new[]
                {
                    0, 1, 2, 1, 3, 2, // Top
                    1, 4, 3, 4, 5, 3, // Right
                    4, 7, 5, 7, 6, 5, // Bottom
                    7, 0, 6, 0, 2, 6, // Left
                    7, 4, 0, 4, 1, 0, // Back
                    2, 3, 6, 3, 5, 6 // Front
                };

            return tempObjectVectors;
        }


        /// <returns></returns>
        public static ObjectVectors CreateSquare( Vector3 min, Vector3 max, bool createUv)
        {
            ObjectVectors tempObjectVectors = new ObjectVectors();
            tempObjectVectors.Vertex = new[]
                {
                    new Vector3(min.x, min.y, min.z),
                    new Vector3(max.x, min.y, min.z),
                    new Vector3(min.x, max.y, max.z),
                    new Vector3(max.x, max.y, max.z),
                };

            tempObjectVectors.ElementData = new[]
                {
                    0, 1, 3,
                    0, 2, 3,
                };

            if (createUv)
            {
                tempObjectVectors.Uvs = new Vector2[]
                    {
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(0, 1),
                        new Vector2(1, 1),
                    };
            }
            return tempObjectVectors;
        }
    }
}