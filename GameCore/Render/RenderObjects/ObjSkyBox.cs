using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Render.OpenGlHelper;
using GameCore.Render.RenderMaterial;
using OpenGL;

namespace GameCore.Render.RenderObjects
{
    public class ObjSkyBox : ObjObject
    {
        public ObjSkyBox(Vector3[] vertexData, int[] elementData) : base(vertexData, elementData)
        {
        }

        public ObjSkyBox(ObjectVectors anObjectVectors) : base(anObjectVectors)
        {
        }

        public ObjSkyBox(List<string> lines, Dictionary<string, ObjMaterial> materials, int vertexOffset, int uvOffset) : base(lines, materials, vertexOffset, uvOffset)
        {
        }
    }
}
