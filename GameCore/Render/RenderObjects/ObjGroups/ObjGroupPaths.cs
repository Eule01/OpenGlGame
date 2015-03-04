#region

using System;
using System.Collections.Generic;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public class ObjGroupPaths : ObjGroup
    {
        public ObjGroupPaths(ShaderProgram program) : base(program)
        {

        }

        public override void Draw()
        {
            if (modelMatrixOld)
            {
                UpdateModelMatrix();
            }
            defaultProgram["model_matrix"].SetValue(modelMatrix);

            Gl.LineWidth(4f);
            Gl.PointSize(20f);

            foreach (ObjPath objPath in Objects)
            {
                objPath.Draw(defaultProgram);
            }

            Gl.LineWidth(1f);
            Gl.PointSize(1f);

        }
    }
}