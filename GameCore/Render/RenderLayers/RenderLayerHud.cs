#region

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameCore.Map;
using GameCore.Render.OpenGlHelper;
using GameCore.Render.RenderMaterial;
using GameCore.Render.RenderObjects;
using OpenGL;
using Tao.FreeGlut;

#endregion

namespace GameCore.Render.RenderLayers
{
    public class RenderLayerHud : RenderLayerBase
    {
        private ShaderProgram hudProgram;
        private List<ObjHudPanel> theHudPanels = new List<ObjHudPanel>();

        private Matrix4 projectionMatrix;

        private Tile.TileIds selectedTileId = Tile.TileIds.Desert;
        private ObjHudButton selectedObjHudButton;

        public override void OnLoad()
        {
            GameCore.TheGameCore.TheGameEventHandler += TheGameCore_TheGameEventHandler;

            hudProgram = new ShaderProgram(VertexShader, FragmentShader);
//            hudProgram = new ShaderProgram(vertexShader2Source, fragmentShader2Source);

            hudProgram.Use();
            projectionMatrix = Matrix4.CreateOrthographic(Width, Height, 0, 10);
            hudProgram["projection_matrix"].SetValue(projectionMatrix);
            hudProgram["model_matrix"].SetValue(Matrix4.Identity);


            Dictionary<Tile.TileIds, PlainBmpTexture> tempTiletypeList =
                RenderObjects.RenderObjects.CreateTileTextures(new Size(20, 20), hudProgram);

            ObjHudPanel hudPanel = CreateHudPanel("HudPanelCreative.png",
                ObjHudPanel.Anchors.TopRight);

            theHudPanels.Add(hudPanel);


            Size tempButtonSize = new Size(60, 60);
            Tile.TileIds tempTileKey = tempTiletypeList.Keys.First();
            Vector2 tempMainButton = new Vector2(70, 100);
            ObjHudButton tempObjHudButton =
                new ObjHudButton(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(0, 0, 0),
                    new Vector3(
                        tempButtonSize.Width,
                        tempButtonSize.Height, 0),
                    true))
                {
                    Anchor = ObjHudButton.Anchors.TopRight,
                    Position = tempMainButton,
                    Size = tempButtonSize
                };
            tempObjHudButton.Size = tempButtonSize;
            tempObjHudButton.UpdatePosition(Width, Height);
            tempObjHudButton.Material = tempTiletypeList[tempTileKey].Material;
            tempObjHudButton.Name += ":" + tempTileKey;
            hudPanel.AddButton(tempObjHudButton);
            selectedObjHudButton = tempObjHudButton;


            int counter = 0;
            int cellsPerRow = 2;
            Vector2 startLoc = new Vector2(120, 200);
            int rowOffset = 100;
            foreach (KeyValuePair<Tile.TileIds, PlainBmpTexture> tempTile in tempTiletypeList)
            {
                int row = counter%cellsPerRow;
                int col = counter/cellsPerRow;
                Vector2 tempLoc = startLoc + new Vector2(-row*rowOffset, col*rowOffset);

                tempObjHudButton = new ObjHudButton(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(0, 0, 0),
                    new Vector3(
                        tempButtonSize.Width,
                        tempButtonSize.Height, 0),
                    true))
                {
                    Anchor = ObjHudButton.Anchors.TopRight,
                    Position = tempLoc,
                    Size = tempButtonSize
                };
                tempObjHudButton.Size = tempButtonSize;
                tempObjHudButton.UpdatePosition(Width, Height);
                tempObjHudButton.Material = tempTiletypeList[tempTile.Key].Material;
                tempObjHudButton.Name += ":" + tempTile.Key;
                tempObjHudButton.Tag = tempTile.Key.ToString();
                hudPanel.AddButton(tempObjHudButton);

                counter++;
            }
        }

        private void TheGameCore_TheGameEventHandler(object sender, GameEventArgs args)
        {
            if (args.TheType == GameEventArgs.Types.MapTileSelected)
            {
                Tile tempSelectedTile = args.TheTile;
                tempSelectedTile.TheTileId = selectedTileId;

                GameCore.TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.MapTileChanged)
                {
                    TheTile = tempSelectedTile
                });
            }
        }

        private ObjHudPanel CreateHudPanel(Size tempSize, Color aColor, ObjHudPanel.Anchors anAnchor)
        {
//            Vector2 tempLoc2 = new Vector2(0, 0);
//
//            SolidBrush tempBrush = new SolidBrush(aColor);
//            Bitmap tempBmp = BitmapHelper.CreatBitamp(new Size(20, 20), tempBrush);
//            ObjMaterial tempMaterial = new ObjMaterial(hudProgram) {DiffuseMap = new Texture(tempBmp)};
            ObjMaterial tempMaterial = TheMaterialManager.GetPlainColor(hudProgram, "HudPanelPlain" + aColor.Name,
                aColor);
            Vector2 tempLoc2 = new Vector2(0, 0);

            ObjHudPanel tempObjObject2 =
                new ObjHudPanel(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(0, 0, 0),
                    new Vector3(tempSize.Width,
                        tempSize.Height,
                        0), true))
                {
                    Anchor = anAnchor,
                    Position =
                        tempLoc2,
                    Size = tempSize
                };
            tempObjObject2.Size = tempSize;
            tempObjObject2.UpdatePosition(Width, Height);
            tempObjObject2.Material = tempMaterial;
            return tempObjObject2;
        }

        private ObjHudPanel CreateHudPanel(string aBmpPath, ObjHudPanel.Anchors anAnchor)
        {
            ObjMaterial tempMaterial = TheMaterialManager.GetFromFile(hudProgram, aBmpPath);
            Size tempSize = tempMaterial.DiffuseMap.Size;

            Vector2 tempLoc2 = new Vector2(0, 0);

            ObjHudPanel tempObjObject2 =
                new ObjHudPanel(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(0, 0, 0),
                    new Vector3(tempSize.Width,
                        tempSize.Height,
                        0), true))
                {
                    Anchor = anAnchor,
                    Position =
                        tempLoc2,
                    Size = tempSize
                };
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
        }

        public override void OnClose()
        {
            foreach (ObjHudPanel aHudObject in theHudPanels)
            {
                aHudObject.Dispose();
            }
            hudProgram.DisposeChildren = true;
            hudProgram.Dispose();
        }

        public override void ReInitialize()
        {
        }

        public override bool OnMouse(int button, int state, int x, int y)
        {
            if (button == Glut.GLUT_LEFT_BUTTON && (state == Glut.GLUT_DOWN || state == Glut.GLUT_UP))
            {
//                MouseWorld = RenderLayerGame.ConvertScreenToWorldCoordsNoDepth(x, y, Camera.ViewMatrix, projectionMatrix, Vector3.Zero);
                MouseWorld = RenderLayerGame.ConvertScreenToWorldCoordsNoDepth(x, y, Matrix4.Identity, projectionMatrix,
                    Vector3.Zero, TheRenderStatus);

                foreach (ObjHudPanel aHudObject in theHudPanels)
                {
                    ObjObject tempObj = aHudObject.IsOn((int) MouseWorld.x, (int) MouseWorld.y);
                    if (tempObj != null)
                    {
                        if (tempObj is ObjHudButton)
                        {
                            Tile.TileIds tempTileId;
                            string tempTileIdName = (string) ((ObjHudButton) tempObj).Tag;
                            if (Tile.TileIds.TryParse(tempTileIdName, true, out tempTileId))
                            {
                                selectedTileId = tempTileId;
                                selectedObjHudButton.Material = TheMaterialManager.Get("Tile_" + tempTileIdName);
                            }
                        }

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