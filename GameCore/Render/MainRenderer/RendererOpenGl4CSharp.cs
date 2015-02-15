#region

using System;
using System.Diagnostics;
using System.Threading;
using CodeToast;
using GameCore.Render.Cameras;
using GameCore.Render.RenderLayers;
using GameCore.Render.RenderMaterial;
using GameCore.UserInterface;
using OpenGL;
using Tao.FreeGlut;

#endregion

namespace GameCore.Render.MainRenderer
{
    public class RendererOpenGl4CSharp : RendererBase
    {
        /// <summary>
        ///     The initial form width and height.
        /// </summary>
        private int formWidth = 1280, formHeight = 720;

        /// <summary>
        ///     The current form width and height.
        /// </summary>
        private int width = 1280, height = 720;

        private RenderLayerSkyBox layerSky;

        private RenderLayerGame layerGame;

        private RenderLayerTextInfo layerInfo;

        private RenderLayerHud layerHud;

        private Stopwatch watch;

        private bool fullscreen;

        private bool exit;


        private float fps = 30;

        public Vector3 MouseWorld = Vector3.Zero;
        public Vector2 MouseCoord = Vector2.Zero;


        public RendererOpenGl4CSharp()
        {
            name = "RendererOpenGl4CSharp";
        }

        public override void Start()
        {
            theKeyBindings = KeyBindings.GetDefaultKeyBindings();
            theKeyBindings.Initialise();

            theMaterialManager = new MaterialManager();
            GameCore.TheGameCore.RaiseMessage("Loaded KeyBindings: " + Environment.NewLine + theKeyBindings);

           // StartOpenGl();
                       Async.Do(delegate { StartOpenGl(); });
        }

        private void StartOpenGl()
        {
            exit = false;
            Glut.glutInit();


            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH | Glut.GLUT_ALPHA | Glut.GLUT_STENCIL |
                                     Glut.GLUT_MULTISAMPLE);

            // http://www.lighthouse3d.com/cg-topics/glut-and-freeglut/
            // Note: glutSetOption is only available with freeglut
            Glut.glutSetOption(Glut.GLUT_ACTION_ON_WINDOW_CLOSE, Glut.GLUT_ACTION_GLUTMAINLOOP_RETURNS);

            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("OpenGL Test");


            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);

            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutSpecialFunc(OnSpecialKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            Glut.glutSpecialUpFunc(OnSpecialKeyboardUp);

            Glut.glutCloseFunc(OnClose);
            Glut.glutReshapeFunc(OnReshape);

            // add our mouse callbacks for this tutorial
            Glut.glutMouseFunc(OnMouse);
            Glut.glutMotionFunc(OnMove);

            #region GL_VERSION

            //this will return your version of opengl
            int major, minor;
            major = Gl.GetInteger(GetPName.MajorVersion);
            minor = Gl.GetInteger(GetPName.MinorVersion);
            GameCore.TheGameCore.RaiseMessage("Major " + major + " Minor " + minor);
//            Console.WriteLine("Major " + major + " Minor " + minor);
            //you can also get your GLSL version, although not sure if it varies from the above
            GameCore.TheGameCore.RaiseMessage("GLSL " + Gl.GetString(StringName.ShadingLanguageVersion));

            #endregion

            Gl.Enable(EnableCap.DepthTest);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            RenderObjects.RenderObjects.TheMaterialManager = theMaterialManager;

            layerSky = new RenderLayerSkyBox(width, height, TheGameStatus, TheUserInputPlayer, theKeyBindings,
                                            theMaterialManager);
            layerSky.OnLoad();

            layerGame = new RenderLayerGame(width, height, TheGameStatus, TheUserInputPlayer, theKeyBindings,
                                            theMaterialManager);
            layerGame.OnLoad();
            layerSky.Camera = layerGame.Camera;

            layerHud = new RenderLayerHud(width, height, TheGameStatus, TheUserInputPlayer, theKeyBindings,
                                          theMaterialManager);
            layerHud.OnLoad();

            layerInfo = new RenderLayerTextInfo(width, height, TheGameStatus, TheUserInputPlayer, theKeyBindings,
                                                theMaterialManager);
            layerInfo.OnLoad();

            watch = Stopwatch.StartNew();

            Glut.glutMainLoop();


            GameCore.TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.RendererExited));
        }


        public override void Close()
        {
            exit = true;
            Thread.Sleep(100);
//            OnClose();
//            Glut.glutLeaveMainLoop();
        }

        public override void MapLoaded()
        {
            Close();

//            foreach (ObjObject aObjObject in theTileObjects)
//            {
//                aObjObject.Dispose();
//            }
//            theTileObjects = GetTileObjects();
            Start();
        }

        private void OnDisplay()
        {
        }

        private void OnRenderFrame()
        {
            if (exit)
            {
                Glut.glutLeaveMainLoop();
            }
            else
            {
                watch.Stop();
                float deltaTime = (float) watch.ElapsedTicks/Stopwatch.Frequency;
                float tempfps = 1.0f/deltaTime;
                fps = fps*0.9f + tempfps*0.1f;

                if (layerInfo.ShowInfo)
                {
                    Camera tempCam = layerGame.Camera;
                    Vector4 tempViewDir = tempCam.Orientation.ToAxisAngle();
                    string tempText = string.Format(
                        "FPS: {0:0.00}, Mouse: [({1:0},{2:0}),{3:0.0},{4:0.0},{5:0.0}], Camera: Pos[{6:0.0},{7:0.0},{8:0.0}]",
//                        "FPS: {0:0.00}, Mouse: [({1:0},{2:0}),{3:0.0},{4:0.0},{5:0.0}], Camera: Pos[{6:0.0},{7:0.0},{8:0.0}] Dir[{9:0.0},{10:0.0},{11:0.0}]",
                        fps, MouseCoord.x, MouseCoord.y, MouseWorld.x, MouseWorld.y, MouseWorld.z,
                        tempCam.Position.x, tempCam.Position.y, tempCam.Position.z
//                        tempViewDir.x, tempViewDir.y, tempViewDir.z
                        );
                    layerInfo.GameInfo = tempText;
                }

                // linear interpolate retained fps with this frames fps with a strong weighting to former.
                watch.Restart();

                // set camForward the viewport and clear the previous depth and color buffers
                Gl.Viewport(0, 0, width, height);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

//                layerSky.OnRenderFrame(deltaTime);
                layerGame.OnRenderFrame(deltaTime);
//                layerHud.OnRenderFrame(deltaTime);
                layerInfo.OnRenderFrame(deltaTime);

                Glut.glutSwapBuffers();
            }
        }


//GLvoid displayFPS(GLvoid)
//{
//	static long lastTime = SDL_GetTicks();
//	static long frames = 0;
//	static GLfloat fps = 0.0f;
//
//	int newTime = SDL_GetTicks();
//
//	if (newTime - lastTime > 100)
//	{
//		float newFPS = (float)frames / float(newTime - lastTime) * 1000.0f;
//
//		fps = (fps + newFPS) / 2.0f;
//
//		//Show FPS in window title
//		char title[80];
//		sprintf(title, "OpenGl Demo - %.2f", fps);
//		SDL_WM_SetCaption(title, NULL);
//		
//		lastTime = newTime;
//		frames = 0;
//	}
//	frames++;
//}


        private void OnReshape(int width, int height)
        {
            this.width = width;
            this.height = height;

            layerSky.OnReshape(width,height);
            layerGame.OnReshape(width, height);
            layerHud.OnReshape(width, height);
            layerInfo.OnReshape(width, height);
        }

        private void OnClose()
        {
            layerSky.OnClose();
            layerGame.OnClose();
            layerHud.OnClose();
            layerInfo.OnClose();

            theMaterialManager.Close();
        }

        #region Controls

        private void OnMouse(int button, int state, int x, int y)
        {
            MouseCoord = new Vector2(x, y);
            if (layerHud.OnMouse(button, state, x, y))
            {
                MouseWorld = layerHud.MouseWorld;
            }
            else
            {
                if (layerGame.OnMouse(button, state, x, y))
                {
                    MouseWorld = layerGame.MouseWorld;
                }
            }
        }

        private void OnMove(int x, int y)
        {
            layerGame.OnMove(x, y);
        }

        private void OnSpecialKeyboardDown(int key, int x, int y)
        {
            layerGame.OnSpecialKeyboardDown(key, x, y);
        }

        private void OnSpecialKeyboardUp(int key, int x, int y)
        {
            layerGame.OnSpecialKeyboardUp(key, x, y);
        }

        private void OnKeyboardDown(byte key, int x, int y)
        {
            if (key == theKeyBindings.TheKeyLookUp[KeyBindings.Ids.GameExit])
            {
                exit = true;
//                Glut.glutLeaveMainLoop();
            }
            else
            {
                layerGame.OnKeyboardDown(key, x, y);
            }
//            else
//            {
//                //                char c = Convert.ToChar(key);
//                //                string b = Encoding.ASCII.GetString(new byte[] { (byte)key });
//                //                Console.WriteLine("Key: " + key + " : " + b);
//                //                Console.WriteLine("Key: " + key);
//            }
        }

        private void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == theKeyBindings.TheKeyLookUp[KeyBindings.Ids.DisplayToggleFullFrame])
            {
                fullscreen = !fullscreen;
                if (fullscreen)
                {
                    Glut.glutFullScreen();
                }
                else
                {
                    Glut.glutPositionWindow(0, 0);
                    Glut.glutReshapeWindow(formWidth, formHeight);
                }
            }
            layerGame.OnKeyboardUp(key, x, y);
            layerInfo.OnKeyboardUp(key, x, y);
        }

        #endregion

        #region Sample Shader

        public static string vertexShaderSource = @"
#version 330

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;
uniform mat4 view_matrix;
uniform float animation_factor;

in vec3 in_position;
in vec3 in_normal;
in vec2 in_uv;

out vec2 uv;

void main(void)
{
  vec4 pos2 = projection_matrix * modelview_matrix * vec4(in_normal, 1);
  vec4 pos1 = projection_matrix * modelview_matrix * vec4(in_position, 1);

  uv = in_uv;
  
  gl_Position = mix(pos2, pos1, animation_factor);
}";

        public static string fragmentShaderSource = @"
#version 330

uniform sampler2D active_texture;

in vec2 uv;

out vec4 out_frag_color;

void main(void)
{
  out_frag_color = mix(texture2D(active_texture, uv), vec4(1, 1, 1, 1), 0.05);
}";

        #endregion
    }
}