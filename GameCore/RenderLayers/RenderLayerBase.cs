using GameCore.UserInterface;

namespace GameCore.RenderLayers
{
    public abstract class RenderLayerBase : IRenderLayer
    {
        protected RenderLayerBase(int width, int height, GameStatus theGameStatus, UserInput theUserInput)
        {
            Width = width;
            Height = height;
            TheGameStatus = theGameStatus;
            TheUserInput = theUserInput;
        }

        public int Width = 1280;
        public int Height = 720;
        public GameStatus TheGameStatus;
        public UserInput TheUserInput;
        public abstract void OnLoad();
        public abstract void OnDisplay();
        public abstract void OnRenderFrame(float deltaTime);
        public abstract void OnReshape(int width, int height);
        public abstract void OnClose();
        public abstract bool OnMouse(int button, int state, int x, int y);
        public abstract void OnMove(int x, int y);
        public abstract void OnSpecialKeyboardDown(int key, int x, int y);
        public abstract void OnSpecialKeyboardUp(int key, int x, int y);
        public abstract void OnKeyboardDown(byte key, int x, int y);
        public abstract void OnKeyboardUp(byte key, int x, int y);
    }
}