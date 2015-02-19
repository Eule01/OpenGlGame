#region

using System.Collections.Generic;
using GameCore.Render.Cameras;
using GameCore.Render.RenderMaterial;
using GameCore.UserInterface;
using OpenGL;

#endregion

namespace GameCore.Render.RenderLayers
{
    public class SceneManager : IRenderLayer
    {
        private List<IRenderLayer> theRenderLayers = new List<IRenderLayer>();
        private List<IRenderLayer> theRenderLayersRevered = new List<IRenderLayer>();
        public Camera theCamera;

        private int width = 1280;
        private int height = 720;
        private GameStatus theGameStatus;
        private UserInputPlayer theUserInputPlayer;
        private KeyBindings theKeyBindings;
        private MaterialManager theMaterialManager;
        public Vector3 MouseWorld = Vector3.Zero;

        public SceneManager(int width, int height, GameStatus theGameStatus, UserInputPlayer theUserInputPlayer,
            KeyBindings theKeyBindings, MaterialManager theMaterialManager)
        {
            this.width = width;
            this.height = height;
            this.theGameStatus = theGameStatus;
            this.theUserInputPlayer = theUserInputPlayer;
            this.theKeyBindings = theKeyBindings;
            this.theMaterialManager = theMaterialManager;
        }

        public void AddCamera(Camera aCamera)
        {
            theCamera = aCamera;
            foreach (IRenderLayer aRenderLayer in theRenderLayers)
            {
                RenderLayerBase tempLayerBase = (RenderLayerBase)aRenderLayer;
                tempLayerBase.TheCamera = theCamera;
            }

        }


        public void AddLayer(IRenderLayer aRenderLayer)
        {
            RenderLayerBase tempLayerBase = (RenderLayerBase) aRenderLayer;
            tempLayerBase.Width = width;
            tempLayerBase.Height = height;
            tempLayerBase.TheGameStatus = theGameStatus;
            tempLayerBase.TheUserInputPlayer = theUserInputPlayer;
            tempLayerBase.TheKeyBindings = theKeyBindings;
            tempLayerBase.TheMaterialManager = theMaterialManager;
            tempLayerBase.TheCamera = theCamera;

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
            this.width = width;
            this.height = height;
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnReshape( width,  height);
            }
        }

        public void OnClose()
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnClose();
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
                renderLayer.OnMove( x,  y);
            }
        }

        public void OnSpecialKeyboardDown(int key, int x, int y)
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnSpecialKeyboardDown( key,  x,  y);
            }
        }

        public void OnSpecialKeyboardUp(int key, int x, int y)
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnSpecialKeyboardUp(key, x, y);
            }
        }

        public void OnKeyboardDown(byte key, int x, int y)
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer. OnKeyboardDown( key,  x,  y);
            }
        }

        public void OnKeyboardUp(byte key, int x, int y)
        {
            foreach (IRenderLayer renderLayer in theRenderLayers)
            {
                renderLayer.OnKeyboardUp( key,  x,  y);
            }
        }
    }
}