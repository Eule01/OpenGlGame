#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using GameCore.GameObjects;
using GameCore.Map;
using GameCore.Render.Cameras;
using GameCore.Render.OpenGlHelper;
using GameCore.Render.RenderMaterial;
using GameCore.Render.RenderObjects;
using GameCore.Render.RenderObjects.ObjGroups;
using GameCore.UserInterface;
using GameCore.Utils;
using OpenGL;
using Tao.FreeGlut;

#endregion

namespace GameCore.Render.RenderLayers
{
    public class RenderLayerGame : RenderLayerBase
    {
        private ShaderProgram program;
        private List<IObjGroup> objMeshs;
        private bool wireframe;

        private bool mouseDown;
        private int downX, downY;
        private int prevX, prevY;

        private List<ObjObject> theTileObjects;
//        private List<ObjGameObject> theRenderGameObjects;
        private ObjGroupGameObject playerObjObject = null;

        private Matrix4 projectionMatrix;

        private ObjMaterial materialPoint;
        private ObjMaterial materialLineMarker;
        private Dictionary<Tile.TileIds, PlainBmpTexture> tileTextures;

        private const bool UseObjMap = false;
        private ObjMap objTileMap;

        private Environment theEnvironment;
        private Camera TheCamera;

        private ObjGroupPaths thePaths;
        private Vector3[] thePathVector3 = null;


        public override void OnLoad()
        {
            GameCore.TheGameCore.TheGameEventHandler += TheGameCore_TheGameEventHandler;


            // create our shader program
            program = new ShaderProgram(VertexShader, FragmentShader);

//            SetupCamera();
//            Camera.SetDirection(new Vector3(0, 0, -1));

            // set up the projection and view matrix
            program.Use();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(TheRenderStatus.Fov,
                (float) TheRenderStatus.Width/TheRenderStatus.Height, TheRenderStatus.ZNear,
                TheRenderStatus.ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
            program["model_matrix"].SetValue(Matrix4.Identity);
            program["light_direction"].SetValue(theEnvironment.LightDirection);
            program["enable_lighting"].SetValue(theEnvironment.Lighting);
            program["ambient"].SetValue(theEnvironment.LightAmbient);

            materialPoint = TheResourceManager.GetPlainColor(program, "GamePlainRed", Color.Red);
            materialLineMarker = TheResourceManager.GetPlainColor(program, "GamePlainGreenYellow", Color.GreenYellow);

            objMeshs = new List<IObjGroup>();

            ObjGroup tempObjMesh2 = ObjLoader.LoadObjFileToObjMesh(program, @"./Resources/Models/Turret1.obj");
            tempObjMesh2.Location = new Vector3(10, 0, 10);
            tempObjMesh2.Scale = Vector3.UnitScale*0.3f;
            objMeshs.Add(tempObjMesh2);

            ObjMaterial tempMaterial = TheResourceManager.GetPlainColor(program, "GamePlainGreen", Color.Green);

            tileTextures = RenderObjects.RenderObjects.CreateTileTextures(new Size(20, 20), program);


            ObjGroup tempObjGroup = new ObjGroup(program);
            ObjObject tempObj =
                new ObjObject(ObjectPrimitives.CreateCube(new Vector3(0, 0, 0), new Vector3(1, 1, 1), true))
                {
                    Material = tileTextures[Tile.TileIds.Grass].Material
                };
//            ObjObject tempObj = CreateCube(program, new Vector3(1, 1, 1), new Vector3(0, 0, 0));
            tempObjGroup.AddObject(tempObj);
            objMeshs.Add(tempObjGroup);


            tempObjGroup = new ObjGroup(program);
//            tempObj = new ObjObject(ObjectPrimitives.CreateCube(new Vector3(2, 0, 0), new Vector3(3, 1, 1), true))
            tempObj = new ObjObject(ObjectPrimitives.CreateCube(new Vector3(0, 0, 0), new Vector3(1, 1, 1), true))
            {
                Material = tileTextures[Tile.TileIds.Road].Material
            };

            tempObjGroup.AddObject(tempObj);
            tempObjGroup.Location = new Vector3(1, 0, 5);
            tempObjGroup.Orientation = Quaternion.FromAngleAxis((float) (Math.PI*0.25), Vector3.Up);
            objMeshs.Add(tempObjGroup);


            tempObjGroup = new ObjGroup(program);
            tempObj =
                new ObjObject(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(5, 1, 1), new Vector3(4, 0, 1),
                    true)) {Material = tempMaterial};
            tempObjGroup.AddObject(tempObj);

            tempObj =
                new ObjObject(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(-1, 1, 1), new Vector3(-2, 0, 0),
                    true));
            tempObjGroup.AddObject(tempObj);
            objMeshs.Add(tempObjGroup);


//            tempObjGroup = new ObjGroup(program);
//            theTileObjects = GetTileObjects();
//            tempObjGroup.AddObjects(theTileObjects);
//            objMeshs.Add(tempObjGroup);
//
            objMeshs.AddRange(GetGameObjects());

            thePaths = new ObjGroupPaths(program);
            objMeshs.Add(thePaths);

            Gl.UseProgram(0);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            if (UseObjMap)
            {
                objTileMap = new ObjMap(TheGameStatus.TheMap, TheCamera);
            }
        }

        private void TheGameCore_TheGameEventHandler(object sender, GameEventArgs args)
        {
        }

        public override void OnDisplay()
        {

       }

        public override void OnRenderFrame(float deltaTime)
        {
//            if (msaa) Gl.Enable(EnableCap.Multisample);
//            else Gl.Disable(EnableCap.Multisample);


            // apply our camera view matrix to the shader view matrix (this can be used for all objects in the scene)
            Gl.UseProgram(program);
            program["view_matrix"].SetValue(TheCamera.ViewMatrix);
//            program["model_matrix"].SetValue(Matrix4.Identity);
            program["enable_lighting"].SetValue(theEnvironment.Lighting);

            if (theEnvironment.LightMove)
            {
                theEnvironment.LightDirection = theEnvironment.LightDirection*Matrix4.CreateRotationY(deltaTime);
                program["light_direction"].SetValue(theEnvironment.LightDirection);
            }
 
            if (thePathVector3 != null)
            {
                ObjPath tempObjPath = new ObjPath("Path", thePathVector3,
                    new Vector3[] {thePathVector3[0], thePathVector3.Last()})
                {
                    LineMaterial = materialPoint,
                    MarkerMaterial = materialLineMarker
                };
                thePaths.AddObject(tempObjPath);
                thePathVector3 = null;
            }
 

            // now draw the object file
//            if (wireframe) Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            if (wireframe) Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            if (objMeshs != null)
            {
                foreach (IObjGroup anObjMesh in objMeshs)
                {
                    anObjMesh.Draw();
                }
            }

            // Reset the model matrix
            program["model_matrix"].SetValue(Matrix4.Identity);

            if (true)
            {
                // Draw a small test grid
                double delta = 5;
                double z = 0.1;
                Gl.PointSize(10);
                // For some reason EnableCap.PointSmooth = ((int)0x0B10), was commented out in OpenGL4CSharp.
                Gl.Enable(EnableCap.PointSmooth);

                // shift the mouse point a bit toward the camera
                Vector3 mousePoint = MouseWorld + ((TheCamera.Position - MouseWorld).Normalize())*0.01f;

                Vector3[] vertexData = new[]
                {
                    mousePoint, new Vector3(0, 0, z),
                    new Vector3(delta, z, delta), new Vector3(0, z, delta), new Vector3(delta, z, 0),
                };

                VBO<Vector3> vertices = new VBO<Vector3>(vertexData);

                if (materialPoint != null) materialPoint.Use();

                Gl.BindBufferToShaderAttribute(vertices, program, "vertexPosition");

                Gl.DrawElements(BeginMode.Points, vertices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                vertices.Dispose();
                vertices = null;
            }

//            if (theRenderGameObjects != null)
//            {
//                foreach (ObjGameObject renderGameObject in theRenderGameObjects)
//                {
//                    renderGameObject.Draw(program);
//                }
//                Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
//                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
//                Gl.UseProgram(0);
//            }

            if (UseObjMap)
            {
                objTileMap.Draw();
            }
        }

        public override void OnReshape(int width, int height)
        {
            Width = width;
            Height = height;


            Gl.UseProgram(program.ProgramID);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(TheRenderStatus.Fov,
                (float) TheRenderStatus.Width/TheRenderStatus.Height, TheRenderStatus.ZNear,
                TheRenderStatus.ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);

            Gl.UseProgram(0);
            Gl.BindBuffer(BufferTarget.DrawIndirectBuffer, 0);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        public override void OnClose()
        {
            if (objMeshs != null)
            {
                foreach (ObjGroup anObjMesh in objMeshs)
                {
                    anObjMesh.Dispose();
                }
            }
//            foreach (ObjGameObject aRenderGameObject in theRenderGameObjects)
//            {
//                aRenderGameObject.Dispose();
//            }
            if (theTileObjects != null)
            {
                foreach (ObjObject aObjObject in theTileObjects)
                {
                    aObjObject.Dispose();
                }
            }
            if (materialPoint != null) materialPoint.Dispose();
            foreach (KeyValuePair<Tile.TileIds, PlainBmpTexture> plainBmpTexture in tileTextures)
            {
                plainBmpTexture.Value.Material.Dispose();
            }
            program.DisposeChildren = true;
            program.Dispose();
        }

        public override void ReInitialize()
        {
            theEnvironment = TheGameStatus.TheEnvironment;
            TheCamera = TheGameStatus.TheCamera;
        }

        public override bool OnMouse(int button, int state, int x, int y)
        {
            if (button == Glut.GLUT_LEFT_BUTTON && state == Glut.GLUT_DOWN)
            {
                MouseWorld = ConvertScreenToWorldCoords(x, y, TheCamera.ViewMatrix, projectionMatrix, TheCamera.Position,
                    TheRenderStatus);
                Vector3 playerMouseVec =
                    (new Vector3(MouseWorld.x, 0.0, MouseWorld.z) - TheGameStatus.ThePlayer.Location).Normalize();

                TheGameStatus.ThePlayer.Orientation = playerMouseVec;
//                TheGameStatus.ThePlayer.Orientation = new Vector(playerMouseVec.x, playerMouseVec.y);

                int mod = Glut.glutGetModifiers();
                if (mod == Glut.GLUT_ACTIVE_CTRL)
                {
                    TheSceneManager.TileSelected(MouseWorld);
                }
            }
            else if (button == Glut.GLUT_RIGHT_BUTTON)
            {
                // this method gets called whenever a new mouse button event happens
                mouseDown = (state == Glut.GLUT_DOWN);

                // if the mouse has just been clicked then we hide the cursor and store the position
                if (mouseDown)
                {
                    Glut.glutSetCursor(Glut.GLUT_CURSOR_NONE);
                    prevX = downX = x;
                    prevY = downY = y;
                }
                else // unhide the cursor if the mouse has just been released
                {
                    Glut.glutSetCursor(Glut.GLUT_CURSOR_LEFT_ARROW);
                    Glut.glutWarpPointer(downX, downY);
                }
            }
            return true;
        }

        public override void OnMove(int x, int y)
        {
            // if the mouse move event is caused by glutWarpPointer then do nothing
            if (x == prevX && y == prevY) return;

            // move the camera when the mouse is down
            if (mouseDown)
            {
                float yaw = (prevX - x)*0.002f;
                TheCamera.Yaw(yaw);

                float pitch = (prevY - y)*0.002f;
                TheCamera.Pitch(pitch);

                prevX = x;
                prevY = y;
            }

            if (x < 0) Glut.glutWarpPointer(prevX = Width, y);
            else if (x > Width) Glut.glutWarpPointer(prevX = 0, y);

            if (y < 0) Glut.glutWarpPointer(x, prevY = Height);
            else if (y > Height) Glut.glutWarpPointer(x, prevY = 0);
        }

        public override void OnSpecialKeyboardDown(int key, int x, int y)
        {
        }

        public override void OnSpecialKeyboardUp(int key, int x, int y)
        {
            if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.CameraTurnAtPlayer])
                TheCamera.LookAt(playerObjObject.TheObjectGame.Location);
//                TheCamera.LookAt(new Vector3(playerObjObject.TheObjectGame.Location.x, 0.0f,
//                    playerObjObject.TheObjectGame.Location.Y));
        }

        public override void OnKeyboardDown(byte key, int x, int y)
        {
//            if (key == 'w') TheUserInputPlayer.Forward = true;
            if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.PlayerForward]) TheUserInputPlayer.Forward = true;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.PlayerBackward])
                TheUserInputPlayer.Backward = true;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.PlayerRight])
                TheUserInputPlayer.Right = true;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.PlayerLeft])
                TheUserInputPlayer.Left = true;
        }

        public override void OnKeyboardUp(byte key, int x, int y)
        {
//            if (key == 'w') TheUserInputPlayer.Forward = false;
            if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.PlayerForward]) TheUserInputPlayer.Forward = false;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.PlayerBackward])
                TheUserInputPlayer.Backward = false;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.PlayerRight])
                TheUserInputPlayer.Right = false;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.PlayerLeft])
                TheUserInputPlayer.Left = false;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.DisplayToggleRenderWireFrame])
                wireframe = !wireframe;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.DisplayToggleLighting])
                theEnvironment.Lighting = !theEnvironment.Lighting;
            else if (key == TheKeyBindings.TheKeyLookUp[KeyBindings.Ids.DisplayToggleLightingRotate])
                theEnvironment.LightMove = !theEnvironment.LightMove;
        }

        #region Game objects

        public void AddPath(Vector3[] tempVect3)
        {
            thePathVector3 = tempVect3;
        }

        private List<ObjGroup> GetGameObjects()
        {
            List<ObjGroup> tempTileObj = CreateRenderGameObjects();
            return tempTileObj;
        }

        private List<ObjGroup> CreateRenderGameObjects()
        {
            Dictionary<ObjectGame.ObjcetIds, PlainBmpTexture> gameObjectsTextures =
                RenderObjects.RenderObjects.CreateGameObjectsTextures(new Size(20, 20), program);
            List<ObjGroup> tempObjList = new List<ObjGroup>();
            List<ObjectGame> gameObjects = TheGameStatus.GameObjects;

            foreach (ObjectGame gameObject in gameObjects)
            {
                Vector tempLoc;
                ObjGroupGameObject tempGroup = null;
                ObjGameObject tempObjObject;
                switch (gameObject.TheObjectId)
                {
                    case ObjectGame.ObjcetIds.Player:
                        tempGroup = new ObjGroupGameObjectPlayer(program) {TheObjectGame = gameObject};
                        tempLoc = new Vector(0.0f, 0.0f);
                        tempLoc -= new Vector(gameObject.Diameter*0.5f, gameObject.Diameter*0.5f);
                        tempObjObject =
                            new ObjGameObject(ObjectPrimitives.CreateCube(new Vector3(tempLoc.X, 0, tempLoc.Y),
                                new Vector3(tempLoc.X + gameObject.Diameter, gameObject.Diameter,
                                    tempLoc.Y + gameObject.Diameter),
                                true));
                        ObjMaterial tempMaterial = TheResourceManager.GetFromFile(program, "tileTestMike200x200.png");
                        tempObjObject.Material = tempMaterial;
                        tempGroup.AddObject(tempObjObject);
                        playerObjObject = tempGroup;
                        tempGroup.Location = gameObject.Location;
                        break;
                   case ObjectGame.ObjcetIds.Enemy:
                        tempGroup = new ObjGroupGameObjectEnemy(program) {TheObjectGame = gameObject};
                        tempLoc = new Vector(0.0f, 0.0f);
                        tempLoc -= new Vector(gameObject.Diameter*0.5f, gameObject.Diameter*0.5f);
                        tempObjObject =
                            new ObjGameObject(ObjectPrimitives.CreateCube(new Vector3(tempLoc.X, 0, tempLoc.Y),
                                new Vector3(tempLoc.X + gameObject.Diameter, gameObject.Diameter,
                                    tempLoc.Y + gameObject.Diameter),
                                true));
                        ObjMaterial tempMaterial1 = TheResourceManager.GetFromFile(program, "tileTestMike200x200.png");
                        tempObjObject.Material = tempMaterial1;
                        tempGroup.AddObject(tempObjObject);
                        tempGroup.Location = gameObject.Location;
                        break;
                    case ObjectGame.ObjcetIds.Turret:

                        ObjGroup tempGroup1 = ObjLoader.LoadObjFileToObjMesh(program, @"./Resources/Models/Turret1.obj");
                        ObjectTurret tempTurret = (ObjectTurret) gameObject;
                        tempGroup = new ObjGroupGameObjectTurret(tempGroup1)
                        {
                            Location = gameObject.Location,
                            Scale = Vector3.UnitScale*0.3f,
                            TheObjectGame = tempTurret
                        };
                        //                        tempGroup.Orientation = tempTurret.Orientation;

                        break;
                    default:
                        tempLoc = new Vector(0.0f, 0.0f);
                        tempLoc -= new Vector(gameObject.Diameter*0.5f, gameObject.Diameter*0.5f);
                        tempObjObject =
                            new ObjGameObject(ObjectPrimitives.CreateCube(new Vector3(tempLoc.X, 0, tempLoc.Y),
                                new Vector3(tempLoc.X + gameObject.Diameter, gameObject.Diameter,
                                    tempLoc.Y + gameObject.Diameter),
                                true))
                            {
                                Material = gameObjectsTextures[gameObject.TheObjectId].Material
                            };

                        tempGroup.AddObject(tempObjObject);
                        tempGroup.TheObjectGame = gameObject;
                        break;
                }

                tempObjList.Add(tempGroup);
            }
            return tempObjList;
        }

        #endregion

        #region Mouse to World

        /// <summary>
        ///     http://gamedev.stackexchange.com/questions/51820/how-can-i-convert-screen-coordinatess-to-world-coordinates-in-opentk
        ///     https://sites.google.com/site/vamsikrishnav/gluunproject
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector3 ConvertScreenToWorldCoords(int x, int y)
        {
            int[] viewport = new int[4];
            Matrix4 modelViewMatrix, projectionMatrix;
            float[] modelViewMatrixF = new float[16];
            float[] projectionMatrixF = new float[16];
            Gl.GetFloatv(GetPName.ModelviewMatrix, modelViewMatrixF);
            modelViewMatrix = new Matrix4(modelViewMatrixF);
            Gl.GetFloatv(GetPName.ProjectionMatrix, projectionMatrixF);
            projectionMatrix = new Matrix4(projectionMatrixF);

            Gl.GetIntegerv(GetPName.Viewport, viewport);

            //Read the window z co-ordinate 
            //(the z value on that point in unit cube)		
            //            glReadPixels(x, viewport[3] - y, 1, 1,
            //     GL_DEPTH_COMPONENT, GL_FLOAT, &z);
            //
            //            float[] z = new float[1];
            int[] zInt = new int[1];
            Gl.ReadPixels(x, viewport[3] - y, 1, 1, PixelFormat.DepthComponent,
                PixelType.Float, zInt);
            byte[] bytes = BitConverter.GetBytes(zInt[0]);
            float z = BitConverter.ToSingle(bytes, 0);

            //            Gl.GetFloat(GetPName.ModelviewMatrix, out modelViewMatrix);
            //            Gl.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
            //            Gl.GetInteger(GetPName.Viewport, viewport);
            Vector3 mouse;
            mouse.x = x;
            //            mouse.y = viewport[3] - y;
            mouse.y = y;
            mouse.z = z;
            Vector4 vector = UnProject(projectionMatrix, modelViewMatrix, new Size(viewport[2], viewport[3]), mouse);
            Vector3 coords = new Vector3(vector.x, vector.y, vector.z);
            return coords;
        }

        /// <summary>
        ///     This one is nearly working perfectly. There is a small error which I think can be corrected using something like
        ///     this
        ///     mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="modelViewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraPosition"></param>
        /// <returns></returns>
        public static Vector3 ConvertScreenToWorldCoordsNoDepth(int x, int y, Matrix4 modelViewMatrix,
            Matrix4 projectionMatrix, Vector3 cameraPosition, RenderStatus renderStatus)
        {
            int[] viewport = new int[4];
            Gl.GetIntegerv(GetPName.Viewport, viewport);

            int[] zInt = new int[1];
//            Gl.ReadPixels(x, viewport[3] - y, 1, 1, PixelFormat.DepthComponent,
//                          PixelType.Float, zInt);
//            byte[] bytes = BitConverter.GetBytes(zInt[0]);
//            float z = BitConverter.ToSingle(bytes, 0);
            // http://www.songho.ca/opengl/gl_projectionmatrix.html
            // http://web.archive.org/web/20130416194336/http://olivers.posterous.com/linear-depth-in-glsl-for-real
            // The depth stored in the buffer [0 1].
//            float z_b = z;
            float z_b = 0.0f;

            // The depth in the normalized device coordinates [-1 1].
            float z_n = 2.0f*z_b - 1.0f;

            // The distance to the camera plane in grid units.
            float z_e = 2.0f*renderStatus.ZFar*renderStatus.ZNear/
                        (renderStatus.ZFar + renderStatus.ZNear -
                         (renderStatus.ZFar - renderStatus.ZNear)*(2.0f*z_b - 1.0f));

            Vector3 mouse;
            //            mouse.y = viewport[3] - y;
            //                        mouse.y =-( viewport[3] - y );
            //            mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);

            mouse.x = x;
            mouse.y = y; //B
            mouse.z = z_n; //C
            Vector4 vector = UnProject(projectionMatrix, modelViewMatrix, new Size(viewport[2], viewport[3]), mouse);

            Vector3 distanceVec = -cameraPosition + vector.Xyz;
            if (distanceVec.Length > renderStatus.ZFar)
            {
                Vector3 distNormVec = distanceVec.Normalize();
                vector.Xyz = cameraPosition + (distNormVec*renderStatus.ZFar*0.99f);
            }

            Vector3 coords = new Vector3(vector.x, vector.y, vector.z);

            return coords;
        }

        /// <summary>
        ///     This one is nearly working perfectly. There is a small error which I think can be corrected using something like
        ///     this
        ///     mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="modelViewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraPosition"></param>
        /// <returns></returns>
        public static Vector3 ConvertScreenToWorldCoords(int x, int y, Matrix4 modelViewMatrix, Matrix4 projectionMatrix,
            Vector3 cameraPosition, RenderStatus renderStatus)
        {
            int[] viewport = new int[4];
            Gl.GetIntegerv(GetPName.Viewport, viewport);

            int[] zInt = new int[1];
            Gl.ReadPixels(x, viewport[3] - y, 1, 1, PixelFormat.DepthComponent,
                PixelType.Float, zInt);
            byte[] bytes = BitConverter.GetBytes(zInt[0]);
            float z = BitConverter.ToSingle(bytes, 0);

            // http://www.songho.ca/opengl/gl_projectionmatrix.html
            // http://web.archive.org/web/20130416194336/http://olivers.posterous.com/linear-depth-in-glsl-for-real
            // The depth stored in the buffer [0 1].
            float z_b = z;

            // The depth in the normalized device coordinates [-1 1].
            float z_n = 2.0f*z_b - 1.0f;

            // The distance to the camera plane in grid units.
            float z_e = 2.0f*renderStatus.ZFar*renderStatus.ZNear/
                        (renderStatus.ZFar + renderStatus.ZNear -
                         (renderStatus.ZFar - renderStatus.ZNear)*(2.0f*z_b - 1.0f));

            Vector3 mouse;
            //            mouse.y = viewport[3] - y;
            //                        mouse.y =-( viewport[3] - y );
            //            mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);

            mouse.x = x;
            mouse.y = y; //B
            mouse.z = z_n; //C
            Vector4 vector = UnProject(projectionMatrix, modelViewMatrix, new Size(viewport[2], viewport[3]), mouse);

            Vector3 distanceVec = -cameraPosition + vector.Xyz;
            if (distanceVec.Length > renderStatus.ZFar)
            {
                Vector3 distNormVec = distanceVec.Normalize();
                vector.Xyz = cameraPosition + (distNormVec*renderStatus.ZFar*0.99f);
            }

            Vector3 coords = new Vector3(vector.x, vector.y, vector.z);

            return coords;
        }

        /// <summary>
        ///     mouse.z has to be in normalized form [-1 1]
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="view"></param>
        /// <param name="viewport"></param>
        /// <param name="mouse"></param>
        /// <returns></returns>
        private static Vector4 UnProject(Matrix4 projection, Matrix4 view, Size viewport, Vector3 mouse)
        {
            Vector4 vec;

            vec.x = 2.0f*mouse.x/viewport.Width - 1; //B
            vec.y = -(2.0f*mouse.y/viewport.Height - 1); //B
            vec.z = mouse.z; //B
            vec.w = 1.0f;

            Matrix4 viewInv = view.Inverse();
            Matrix4 projInv = projection.Inverse();

            vec = vec*projInv*viewInv; //B

            if (vec.w > float.Epsilon || vec.w < float.Epsilon)
            {
                vec.x /= vec.w;
                vec.y /= vec.w;
                vec.z /= vec.w;
            }

            return vec;
        }

        #endregion

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
//    normal = normalize((model_matrix * vec4(floor(vertexNormal), 0)).xyz);
    uv = vertexUV;

    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        private const string FragmentShader = @"
#version 130

in vec3 normal;
in vec2 uv;

out vec4 fragment;

uniform float ambient;
uniform vec3 diffuse;
uniform sampler2D texture;
uniform vec3 light_direction;
uniform float transparency;
uniform bool useTexture;
uniform bool enable_lighting;

void main(void)
{
    float light = (enable_lighting ? max(ambient, dot(normal, light_direction)) : 1);
    vec4 sample = (useTexture ? texture2D(texture, uv) : vec4(1, 1, 1, 1));
    fragment = vec4(light * diffuse * sample.xyz, transparency * sample.a);
}
";
    }
}