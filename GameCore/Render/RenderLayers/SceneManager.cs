#region

using System;
using System.Collections.Generic;
using GameCore.Map;
using GameCore.Render.Cameras;
using GameCore.Render.RenderMaterial;
using GameCore.UserInterface;
using OpenGL;

#endregion

namespace GameCore.Render.RenderLayers
{
    public class SceneManager : IRenderLayer
    {
        private readonly List<IRenderLayer> theRenderLayers = new List<IRenderLayer>();
        private List<IRenderLayer> theRenderLayersRevered = new List<IRenderLayer>();

        public RenderStatus TheRenderStatus;

        public Camera TheCamera;

        private int width = 1280;
        private int height = 720;
        private GameStatus theGameStatus;
        private UserInputPlayer theUserInputPlayer;
        private KeyBindings theKeyBindings;
        private ResourceManager theResourceManager;
        public Vector3 MouseWorld = Vector3.Zero;

        public SceneManager(GameStatus theGameStatus, UserInputPlayer theUserInputPlayer,
            KeyBindings theKeyBindings, ResourceManager theResourceManager, RenderStatus theRenderStatus)
        {
            GameCore.TheGameCore.TheGameEventHandler += TheGameCore_TheGameEventHandler;

            this.width = theRenderStatus.Width;
            this.height = theRenderStatus.Height;
            this.theGameStatus = theGameStatus;
            this.theUserInputPlayer = theUserInputPlayer;
            this.theKeyBindings = theKeyBindings;
            this.theResourceManager = theResourceManager;
            this.TheRenderStatus = theRenderStatus;

            ReInitialize();
        }

        void TheGameCore_TheGameEventHandler(object sender, GameEventArgs args)
        {
            switch (args.TheType)
            {
                case GameEventArgs.Types.StatusGameEngine:
                    break;
                case GameEventArgs.Types.Message:
                    break;
                case GameEventArgs.Types.MapLoaded:
//                    NewMapLoaded();
                    break;
                case GameEventArgs.Types.MapSaved:
                    break;
                case GameEventArgs.Types.RendererExited:
                    break;
                case GameEventArgs.Types.MapTileSelected:
                    break;
                case GameEventArgs.Types.MapTileChanged:
                    break;
            }
        }

        public void AddCamera(Camera aCamera)
        {
            TheCamera = aCamera;
            TheCamera.Width = width;
            TheCamera.Height = height;
            TheCamera.TheUserInputPlayer = theUserInputPlayer;

            foreach (IRenderLayer aRenderLayer in theRenderLayers)
            {
                RenderLayerBase tempLayerBase = (RenderLayerBase) aRenderLayer;
            }
            theRenderLayers.Insert(0, TheCamera);
            TheCamera.ReInitialize();
        }


        public void AddLayer(IRenderLayer aRenderLayer)
        {
            RenderLayerBase tempLayerBase = (RenderLayerBase) aRenderLayer;
            tempLayerBase.Width = width;
            tempLayerBase.Height = height;
            tempLayerBase.TheUserInputPlayer = theUserInputPlayer;

            tempLayerBase.ReInitialize();

            theRenderLayers.Add(aRenderLayer);

            theRenderLayersRevered = new List<IRenderLayer>(theRenderLayers);
            theRenderLayersRevered.Reverse();
        }

        public void OnLoad()
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnLoad();
            }
        }

        public void OnDisplay()
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnDisplay();
            }
        }

        public void OnRenderFrame(float deltaTime)
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnRenderFrame(deltaTime);
            }
        }

        public void OnReshape(int width, int height)
        {
            TheRenderStatus.Width = width;
            TheRenderStatus.Height = height;
            this.width = width;
            this.height = height;
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnReshape(width, height);
            }
        }

        public void OnClose()
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnClose();
            }
        }

        public void ReInitialize()
        {
            RenderLayerBase.TheGameStatus = theGameStatus;
            RenderLayerBase.TheRenderStatus = TheRenderStatus;
            RenderLayerBase.TheKeyBindings = theKeyBindings;
            RenderLayerBase.TheResourceManager = theResourceManager;
            RenderLayerBase.TheSceneManager = this;

            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.ReInitialize();
                width = TheRenderStatus.Width;
                height = TheRenderStatus.Height;
            }
        }

        public bool OnMouse(int button, int state, int x, int y)
        {
            bool res = false;
            foreach (IRenderLayer renderLayer in theRenderLayersRevered)
            {
                res = renderLayer.OnMouse(button, state, x, y);
                MouseWorld = ((RenderLayerBase) renderLayer).MouseWorld;
                if (res)
                {
                    break;
                }
            }
            return res;
        }

        public void OnMove(int x, int y)
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnMove(x, y);
            }
        }

        public void OnSpecialKeyboardDown(int key, int x, int y)
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnSpecialKeyboardDown(key, x, y);
            }
        }

        public void OnSpecialKeyboardUp(int key, int x, int y)
        {
//            if (key == theKeyBindings.TheKeyLookUp[KeyBindings.Ids.CameraTurnAtPlayer])
//                TheCamera.LookAt(new Vector3(playerObjObject.theObjectGame.Location.X, 0.0f,
//                    playerObjObject.theObjectGame.Location.Y));

            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnSpecialKeyboardUp(key, x, y);
            }
        }

        public void OnKeyboardDown(byte key, int x, int y)
        {
//            GameCore.TheGameCore.RaiseMessage("Key: [" + x + "," + y + "]: " + key + ".");
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnKeyboardDown(key, x, y);
            }
        }

        public void OnKeyboardUp(byte key, int x, int y)
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnKeyboardUp(key, x, y);
            }
        }

        public override string ToString()
        {
            string outStr = "";
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                outStr += renderLayer.ToString() + System.Environment.NewLine;
            }
            outStr += TheCamera.ToString() + System.Environment.NewLine;
            return outStr;
        }

        public void TileSelected(Vector3 mouseWorld)
        {
            Tile tempSelectedTile = theGameStatus.TheMap[(int) mouseWorld.x, (int) mouseWorld.z];
            theGameStatus.TheMap.SelectTile(mouseWorld);



            GameCore.TheGameCore.RaiseMessage("Tile: " + tempSelectedTile);
        }
    }
}