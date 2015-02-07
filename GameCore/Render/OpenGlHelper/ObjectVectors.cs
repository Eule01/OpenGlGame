#region

using OpenGL;

#endregion

namespace GameCore.Render.OpenGlHelper
{
    /// <summary>
    /// This holds the vectors that create object primitives.
    /// </summary>
    public class ObjectVectors
    {
        public Vector3[] Vertex;

        /// <summary>
        ///     the indexed of the vertexes that create triangles.
        /// </summary>
        public int[] ElementData;

        public Vector2[] Uvs;

        public Vector3[] normalData;
    }
}