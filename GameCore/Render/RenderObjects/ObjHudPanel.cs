#region

using System;
using System.Collections.Generic;
using System.Drawing;
using GameCore.Render.OpenGlHelper;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class ObjHudPanel : ObjObject
    {
        private Anchors anchor = Anchors.TopLeft;

        private Vector2 position;

        private Size size;

        private RectangleF theRectangle = new RectangleF(0, 0, 1, 1);

        private List<ObjHudButton> theHudButtons = new List<ObjHudButton>();

        public enum Anchors
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }


        private Vector3 realPos;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Anchors Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }

        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                theRectangle.Size = size;
            }
        }

        public ObjHudPanel(Vector3[] vertexData, int[] elementData) : base(vertexData, elementData)
        {
            Name += "ObjHudPanel";
        }

        public ObjHudPanel(ObjectVectors anObjectVectors) : base(anObjectVectors)
        {
            Name += "ObjHudPanel";
        }

        public ObjHudPanel(List<string> lines, Dictionary<string, ObjMaterial> materials, int vertexOffset, int uvOffset)
            : base(lines, materials, vertexOffset, uvOffset)
        {
            Name += "ObjHudPanel";
        }


        public void AddButton(ObjHudButton anObjHudButton)
        {
            theHudButtons.Add(anObjHudButton);
        }

        public void UpdatePosition(int aWidth, int aHeight)
        {
            Vector3 orgin;
            Vector3 tempPos;
            float z = -0.1f;
            switch (anchor)
            {
                case Anchors.TopLeft:
                    orgin = new Vector3(-aWidth*0.5, aHeight*0.5, 0);
                    tempPos = new Vector3(position.x, -position.y, z);
                    realPos = orgin + tempPos;
                    break;
                case Anchors.TopRight:
                    orgin = new Vector3(aWidth*0.5, aHeight*0.5, 0);
                    tempPos = new Vector3(-position.x - size.Width, -position.y - size.Height, z);
                    realPos = orgin + tempPos;
                    break;
                case Anchors.BottomLeft:
                    orgin = new Vector3(-aWidth*0.5, -aHeight*0.5, 0);
                    tempPos = new Vector3(position.x, position.y - size.Height, z);
                    realPos = orgin + tempPos;
                    break;
                case Anchors.BottomRight:
                    orgin = new Vector3(aWidth*0.5, -aHeight*0.5, 0);
                    tempPos = new Vector3(-position.x - size.Width, position.y, z);
//                    tempPos = new Vector3(-position.x - size.Width, position.y - size.Height, 0);
                    realPos = orgin + tempPos;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            theRectangle.Location = new PointF(realPos.x, realPos.y);
            theRectangle.Size = size;

            foreach (ObjHudButton anHudButton in theHudButtons)
            {
                anHudButton.UpdatePosition(aWidth,aHeight);
            }

        }

        public ObjObject IsOn(int x, int y)
        {
            foreach (ObjHudButton anHudButton in theHudButtons)
            {
                ObjObject temp = anHudButton.IsOn(x, y);
                if (temp != null)
                {
                    return temp;
                }
            }
            return theRectangle.Contains(x, y) ? this : null;
        }

        public void Draw(ShaderProgram aProgram)
        {
            if (vertices == null || triangles == null) return;

            Gl.Disable(EnableCap.CullFace);
            if (Material != null) Material.Use();

            aProgram.Use();
            aProgram["model_matrix"].SetValue(Matrix4.CreateTranslation(realPos));
//            aProgram["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(Position.x, Position.y, 0)));

            Gl.BindBufferToShaderAttribute(vertices, Material.Program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(normals, Material.Program, "vertexNormal");
            if (uvs != null) Gl.BindBufferToShaderAttribute(uvs, Material.Program, "vertexUV");
            Gl.BindBuffer(triangles);

            Gl.DrawElements(BeginMode.Triangles, triangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            foreach (ObjHudButton anHudButton in theHudButtons)
            {
                anHudButton.Draw(aProgram);
            }
        }

        public override string ToString()
        {
            string outStr = base.ToString();
            return outStr;
        }
    }
}