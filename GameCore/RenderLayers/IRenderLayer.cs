namespace GameCore.RenderLayers
{
    public interface IRenderLayer
    {
        void OnLoad();
        void OnDisplay();
        void OnRenderFrame(float deltaTime);
        void OnReshape(int width, int height);
        void OnClose();

        #region UI

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="state"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if the mouse event has been processesd. In that case its not passed on</returns>
        bool OnMouse(int button, int state, int x, int y);
        void OnMove(int x, int y);
        void OnSpecialKeyboardDown(int key, int x, int y);
        void OnSpecialKeyboardUp(int key, int x, int y);
        void OnKeyboardDown(byte key, int x, int y);
        void OnKeyboardUp(byte key, int x, int y);

        #endregion
    }
}