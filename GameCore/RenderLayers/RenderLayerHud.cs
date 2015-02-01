#region

using System;
using System.Collections.Generic;
using System.Drawing;
using GameCore.DrawingObjects;
using GameCore.Map;
using GameCore.UserInterface;
using GameCore.Utils;
using OpenGL;
using Tao.FreeGlut;

#endregion

namespace GameCore.RenderLayers
{
    public class RenderLayerHud : RenderLayerBase
    {
        private ShaderProgram hudProgram;
        private ObjLoader objectList;
        private List<ObjObject> theTileObjects;
        private List<ObjHudPanel> theHudPanels = new List<ObjHudPanel>();

        public Vector3 MouseWorld = Vector3.Zero;
        private Matrix4 projectionMatrix;


        public RenderLayerHud(int width, int height, GameStatus theGameStatus, UserInputPlayer theUserInputPlayer, KeyBindings theKeyBindings)
            : base(width, height, theGameStatus, theUserInputPlayer,theKeyBindings)
        {
            theTileObjects = new List<ObjObject>();
        }

        public override void OnLoad()
        {
            hudProgram = new ShaderProgram(VertexShader, FragmentShader);
//            hudProgram = new ShaderProgram(vertexShader2Source, fragmentShader2Source);

            hudProgram.Use();
            projectionMatrix = Matrix4.CreateOrthographic(Width, Height, 0, 1000);
            hudProgram["projection_matrix"].SetValue(projectionMatrix);
            hudProgram["model_matrix"].SetValue(Matrix4.Identity);

//            hudProgram["color"].SetValue(new Vector3(1, 1, 1));

            Dictionary<Tile.TileIds, PlainBmpTexture> tempTiletypeList =
                RenderObjects.CreateTileTextures(new Size(20, 20), hudProgram);
            List<ObjObject> tempObjList = new List<ObjObject>();
            int counter = 2;
            int zeroX = -Width/2;
            int zeroY = Height/2;
            Size tempSize = new Size(50, 50);
            foreach (KeyValuePair<Tile.TileIds, PlainBmpTexture> tempTile in tempTiletypeList)
            {
                Vector tempLoc = new Vector(zeroX + 10, zeroY - 10 - counter*(tempSize.Height + 10));

                ObjObject tempObjObject = RenderLayerGame.CreateSquare(hudProgram, new Vector3(tempLoc.X, tempLoc.Y, -0.1),
                                                                       new Vector3(tempLoc.X + tempSize.Width,
                                                                                   tempLoc.Y + tempSize.Height, -0.1));
                tempObjObject.Material = tempTiletypeList[tempTile.Key].Material;

                tempObjList.Add(tempObjObject);
                counter++;
            }
            theTileObjects.AddRange(tempObjList);

            tempSize = new Size(100, Height);
//            ObjHudPanel hudPanel = CreateHudPanel(tempSize, Color.Brown, ObjHudPanel.Anchors.TopRight);
            ObjHudPanel hudPanel = CreateHudPanel(@"./Resources/Images/HudPanelCreative.png", ObjHudPanel.Anchors.TopRight);

            theHudPanels.Add(hudPanel);


            counter = 0;
            int cellsPerRow = 2;
             Vector2 startLoc = new Vector2(120,200);
            int rowOffset = 100;
            tempSize = new Size(60, 60);
            foreach (KeyValuePair<Tile.TileIds, PlainBmpTexture> tempTile in tempTiletypeList)
            {
                int row = counter/cellsPerRow;
                int col = counter % cellsPerRow;
                Vector2 tempLoc = startLoc + new Vector2( -row * rowOffset,  col * rowOffset);

                ObjHudButton tempObjHudButton = RenderLayerGame.CreateSquareHudButton(hudProgram, new Vector3(0, 0, 0),
                                                                                      new Vector3(tempSize.Width,
                                                                                                  tempSize.Height, 0),
                                                                                      ObjHudButton.Anchors.TopRight,
                                                                                      tempLoc, tempSize);
                tempObjHudButton.Size = tempSize;
                tempObjHudButton.UpdatePosition(Width, Height);
                tempObjHudButton.Material = tempTiletypeList[tempTile.Key].Material;
                tempObjHudButton.Name += ":" + tempTile.Key;
                hudPanel.AddButton(tempObjHudButton);

                counter++;
            }
        }

        private ObjHudPanel CreateHudPanel(Size tempSize, Color aColor, ObjHudPanel.Anchors anAnchor)
        {
            Vector2 tempLoc2 = new Vector2(0, 0);

            SolidBrush tempBrush = new SolidBrush(aColor);
            Bitmap tempBmp = BitmapHelper.CreatBitamp(new Size(20, 20), tempBrush);
            ObjMaterial tempMaterial = new ObjMaterial(hudProgram) {DiffuseMap = new Texture(tempBmp)};


            ObjHudPanel tempObjObject2 = RenderLayerGame.CreateSquareHudPanel(hudProgram, new Vector3(0, 0, 0),
                                                                              new Vector3(tempSize.Width,
                                                                                          tempSize.Height,
                                                                                          0),
                                                                              anAnchor, tempLoc2,
                                                                              tempSize);
            tempObjObject2.Size = tempSize;
            tempObjObject2.UpdatePosition(Width, Height);
            tempObjObject2.Material = tempMaterial;
            return tempObjObject2;
        }

        private ObjHudPanel CreateHudPanel(string aBmpPath, ObjHudPanel.Anchors anAnchor)
        {
            Texture tempTexture = new Texture(aBmpPath);
            Size tempSize = tempTexture.Size;

            ObjMaterial tempMaterial = new ObjMaterial(hudProgram) { DiffuseMap = tempTexture };

            Vector2 tempLoc2 = new Vector2(0, 0);

            ObjHudPanel tempObjObject2 = RenderLayerGame.CreateSquareHudPanel(hudProgram, new Vector3(0, 0, 0),
                                                                              new Vector3(tempSize.Width,
                                                                                          tempSize.Height,
                                                                                          0),
                                                                              anAnchor, tempLoc2,
                                                                              tempSize);
            tempObjObject2.Size = tempSize;
            tempObjObject2.UpdatePosition(Width, Height);
            tempObjObject2.Material = tempMaterial;
            return tempObjObject2;
        }

        public override void OnDisplay()
        {
        }

        public override void OnRenderFrame(float deltaTime)
        {
            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            // bind the font program as well as the font texture
            Gl.UseProgram(hudProgram.ProgramID);
//            Gl.BindTexture(font.FontTexture);

//            if (objectList != null)
//            {
//                objectList.Draw();
//            }


//            if (theTileObjects != null)
//            {
//                foreach (ObjObject theTileObject in theTileObjects)
//                {
//                    theTileObject.Draw();
//                }
//            }

            foreach (ObjHudPanel theHudObject in theHudPanels)
            {
                theHudObject.Draw(hudProgram);
            }
        }

        public override void OnReshape(int width, int height)
        {
            Width = width;
            Height = height;

            Gl.UseProgram(hudProgram.ProgramID);
            projectionMatrix = Matrix4.CreateOrthographic(Width, Height, 0, 1000);
            hudProgram["projection_matrix"].SetValue(projectionMatrix);

            foreach (ObjHudPanel theHudObject in theHudPanels)
            {
                theHudObject.UpdatePosition(Width, Height);
            }


//
//            information.Position = new Vector2(-width / 2 + 10, height / 2 - font.Height - 10);
        }

        public override void OnClose()
        {
            if (theTileObjects != null)
            {
                foreach (ObjObject aObjObject in theTileObjects)
                {
                    aObjObject.Dispose();
                }
            }
            foreach (ObjHudPanel aHudObject in theHudPanels)
            {
                aHudObject.Dispose();
            }

            if (objectList != null)
            {
                objectList.Dispose();
            }
            hudProgram.DisposeChildren = true;
            hudProgram.Dispose();
        }

        public override bool OnMouse(int button, int state, int x, int y)
        {
            if (button == Glut.GLUT_LEFT_BUTTON && (state == Glut.GLUT_DOWN || state == Glut.GLUT_UP))
            {
//                MouseWorld = RenderLayerGame.ConvertScreenToWorldCoordsNoDepth(x, y, Camera.ViewMatrix, projectionMatrix, Vector3.Zero);
                MouseWorld = RenderLayerGame.ConvertScreenToWorldCoordsNoDepth(x, y, Matrix4.Identity, projectionMatrix,
                                                                               Vector3.Zero);

                foreach (ObjHudPanel aHudObject in theHudPanels)
                {
                    ObjObject tempObj = aHudObject.IsOn((int) MouseWorld.x, (int) MouseWorld.y);
                    if (tempObj != null)
                    {
                        GameCore.TheGameCore.RaiseMessage("Mouse: [" + x + "," + y + "] on HUD: " + tempObj + ".");
                        return true;
                    }
                }
            }
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
        }

        #region Sample Shader

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

   // gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
    gl_Position = projection_matrix * model_matrix * vec4(vertexPosition, 1);
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
    vec3 light_direction = normalize(vec3(1, 1, 0));
    float light = max(0.5, dot(normal, light_direction));
    vec4 sample = (useTexture ? texture2D(texture, uv) : vec4(1, 1, 1, 1));
    fragment = vec4(light * diffuse * sample.xyz, transparency * sample.a);
//    fragment = vec4(diffuse * sample.xyz, transparency * sample.a);
}
";

        #endregion
    }
}