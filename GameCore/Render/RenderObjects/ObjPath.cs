using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Render.RenderMaterial;
using OpenGL;

namespace GameCore.Render.RenderObjects
{
    public class ObjPath : IObjObject
    {
        private VBO<Vector3> verticesLines;
        private VBO<Vector3> verticesMarkers;
        public ObjMaterial LineMaterial;
        public ObjMaterial MarkerMaterial;

        public ObjPath(string name, Vector3[] linePoints, Vector3[] markerPoints)
        {
            Name = name;
            verticesLines = new VBO<Vector3>(linePoints);
            verticesMarkers = new VBO<Vector3>(markerPoints);
        }

        public string Name { get; set; }

        public ObjMaterial Material { get; set; }



        public void Draw()
        {
        }

        public void Draw(ShaderProgram aProgram)
        {
            if (LineMaterial != null) LineMaterial.Use();
            Gl.BindBufferToShaderAttribute(verticesLines, aProgram, "vertexPosition");
            Gl.DrawElements(BeginMode.LineStrip, verticesLines.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            if (MarkerMaterial != null) MarkerMaterial.Use();
            Gl.BindBufferToShaderAttribute(verticesMarkers, aProgram, "vertexPosition");
            Gl.DrawElements(BeginMode.Points, verticesLines.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void Dispose()
        {
            verticesLines.Dispose();
            verticesLines = null;
        }
    }
}
