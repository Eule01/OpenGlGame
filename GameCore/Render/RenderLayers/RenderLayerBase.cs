#region

using GameCore.Render.Cameras;
using GameCore.Render.RenderMaterial;
using GameCore.UserInterface;
using OpenGL;

#endregion

namespace GameCore.Render.RenderLayers
{
    public abstract class RenderLayerBase : IRenderLayer
    {
        protected RenderLayerBase()
        {
        }

        public static SceneManager TheSceneManager;
        public static GameStatus TheGameStatus;
        public static KeyBindings TheKeyBindings;
        public static MaterialManager TheMaterialManager;
        public static RenderStatus TheRenderStatus;



        public int Width = 1280;
        public int Height = 720;
        public UserInputPlayer TheUserInputPlayer;
        public Vector3 MouseWorld = Vector3.Zero;

        public abstract void OnLoad();
        public abstract void OnDisplay();
        public abstract void OnRenderFrame(float deltaTime);
        public abstract void OnReshape(int width, int height);
        public abstract void OnClose();
        public abstract void ReInitialize();

        public abstract bool OnMouse(int button, int state, int x, int y);
        public abstract void OnMove(int x, int y);
        public abstract void OnSpecialKeyboardDown(int key, int x, int y);
        public abstract void OnSpecialKeyboardUp(int key, int x, int y);
        public abstract void OnKeyboardDown(byte key, int x, int y);
        public abstract void OnKeyboardUp(byte key, int x, int y);
    }
}