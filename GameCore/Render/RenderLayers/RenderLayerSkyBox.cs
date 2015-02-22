#region

using System.Collections.Generic;
using System.IO;
using GameCore.Render.Cameras;
using GameCore.Render.RenderObjects.ObjGroups;
using GameCore.UserInterface;
using OpenGL;

#endregion

namespace GameCore.Render.RenderLayers
{
    public class RenderLayerSkyBox : RenderLayerBase
    {
        private ShaderProgram program;
        private List<ObjGroup> objGroups;

        /// <summary>
        ///     The near clipping distance.
        /// </summary>
        private const float ZNear = 0.1f;

        /// <summary>
        ///     The far clipping distance.
        /// </summary>
        private const float ZFar = 1000f;

        /// <summary>
        ///     Field of view of the camera
        /// </summary>
        private const float Fov = 0.45f;

        private Matrix4 projectionMatrix;
        private ObjGroupSkyBox skyBoxObjGroup;
        private Camera theCamera;

        private List<ObjGroupSkyBox> skyBoxes = new List<ObjGroupSkyBox>();

        private string skypBoxFolder = "SkyBoxes";

        public override void OnLoad()
        {
            // create our shader program
            program = new ShaderProgram(VertexShader, FragmentShader);
            // set up the projection and view matrix
            program.Use();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(Fov, (float) Width/Height, ZNear,
                ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
            program["model_matrix"].SetValue(Matrix4.Identity);


            objGroups = new List<ObjGroup>();

            // Add all skyBoxes to the list.           
            AddAllSkyBoxes();

            // Use the first one in the list.
            skyBoxObjGroup = skyBoxes[0];
//            skyBoxObjGroup.Scale = Vector3.UnitScale*0.7f;

            objGroups.Add(skyBoxObjGroup);
        }

        private void AddAllSkyBoxes()
        {
            skyBoxes.Clear();
            string tempSkyBoxFolder = TheMaterialManager.SkyBoxDirectory;
            string[] files = Directory.GetFiles(tempSkyBoxFolder, "SkyBox_T*.*");
            List<string> fileList = new List<string>();
            foreach (string aFile in files)
            {
                string extension = Path.GetExtension(aFile);
                if (extension != null)
                {
                    string tempExt = extension.ToLower();

                    if (tempExt == ".jpg" || tempExt == ".png")
                    {
                        string tempRelPath = Path.Combine(skypBoxFolder, Path.GetFileName(aFile));
                        fileList.Add(tempRelPath);
                    }
                }
            }

            foreach (string tempSkyBoxPath in fileList)
            {
                ObjGroupSkyBox tempSkyBoxGroup = ObjGroupSkyBox.GetNewSkyBoxTypeT(program, tempSkyBoxPath);
                tempSkyBoxGroup.Name = Path.GetFileNameWithoutExtension(tempSkyBoxPath);
                skyBoxes.Add(tempSkyBoxGroup);
            }
        }


        private void CycleToNextSkyBox()
        {
            int tempCurrentIndex = skyBoxes.IndexOf(skyBoxObjGroup);
            tempCurrentIndex++;
            if (tempCurrentIndex >= skyBoxes.Count)
            {
                tempCurrentIndex = 0;
            }
            ChangeToSkyBox(skyBoxes[tempCurrentIndex]);
        }

        private void ChangeToSkyBox(ObjGroupSkyBox aObjGroupSkyBox)
        {
            skyBoxObjGroup = aObjGroupSkyBox;
            objGroups[0] = skyBoxObjGroup;
        }

        public override void OnDisplay()
        {
        }

        public override void OnRenderFrame(float deltaTime)
        {
            Gl.Disable(EnableCap.DepthTest);
            Gl.DepthMask(false);
            Gl.Disable(EnableCap.DepthClamp);
            Vector3 tempLoc = theCamera.Position;
            skyBoxObjGroup.Location = tempLoc;
            Gl.UseProgram(program);
            // apply our camera view matrix to the shader view matrix (this can be used for all objects in the scene)
            program["view_matrix"].SetValue(theCamera.ViewMatrix);
//            program["model_matrix"].SetValue(Matrix4.CreateScaling(new Vector3(0.7,0.7,0.7))*Matrix4.CreateTranslation(new Vector3(tempLoc.x, tempLoc.y, tempLoc.z)));


            // now draw the object file
//            if (wireframe) Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            if (objGroups != null)
            {
                foreach (ObjGroup anObjMesh in objGroups)
                {
                    anObjMesh.Draw();
                }
            }

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthMask(true);
            Gl.Enable(EnableCap.DepthClamp);
        }

        public override void OnReshape(int width, int height)
        {
            Height = height;
            Width = width;
            Gl.UseProgram(program.ProgramID);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(Fov, (float) Width/Height, ZNear,
                ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
        }

        public override void OnClose()
        {
            if (objGroups != null)
            {
                foreach (ObjGroup anObjMesh in objGroups)
                {
                    anObjMesh.Dispose();
                }
            }
            program.DisposeChildren = true;
            program.Dispose();
        }

        public override void ReInitialize()
        {
            theCamera = TheGameStatus.TheCamera;
        }

        public override bool OnMouse(int button, int state, int x, int y)
        {
            return false;
        }

        public override void OnMove(int x, int y)
        {
        }

        public override void OnSpecialKeyboardDown(int key, int x, int y)
        {
        }

        public override void OnSpecialKeyboardUp(int key, int x, int y)
        {
        }

        public override void OnKeyboardDown(byte key, int x, int y)
        {
        }

        public override void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.DisplayCycleThroughSkyboxes]) CycleToNextSkyBox();
        }


        private const string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;

out vec3 normal;
out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    normal = (length(vertexNormal) == 0 ? vec3(0, 0, 0) : normalize((model_matrix * vec4(vertexNormal, 0)).xyz));
    uv = vertexUV;

    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        private const string FragmentShader = @"
#version 130

in vec3 normal;
in vec2 uv;

out vec4 fragment;

uniform vec3 diffuse;
uniform sampler2D texture;
uniform float transparency;
uniform bool useTexture;

void main(void)
{
 //   vec3 light_direction = normalize(vec3(1, 1, 0));
//    float light = max(0.5, dot(normal, light_direction));
    vec4 sample = (useTexture ? texture2D(texture, uv) : vec4(1, 1, 1, 1));
//    fragment = vec4(sample.xyz, sample.a);
    fragment = vec4(sample.xyz, transparency * sample.a);
//    fragment = vec4(light * diffuse * sample.xyz, transparency * sample.a);
}
";
    }

//        private const string FragmentShader = @"
//#version 130
//
//in vec3 normal;
//in vec2 uv;
//
//out vec4 fragment;
//
//uniform vec3 diffuse;
//uniform sampler2D texture;
//uniform float transparency;
//uniform bool useTexture;
//
//void main(void)
//{
//    vec3 light_direction = normalize(vec3(1, 1, 0));
//    float light = max(0.5, dot(normal, light_direction));
//    vec4 sample = (useTexture ? texture2D(texture, uv) : vec4(1, 1, 1, 1));
//    fragment = vec4(light * diffuse * sample.xyz, transparency * sample.a);
//}
//";
}