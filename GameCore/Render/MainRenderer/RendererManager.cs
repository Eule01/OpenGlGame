#region

using System.Collections.Generic;
using GameCore.UserInterface;

#endregion

namespace GameCore.Render.MainRenderer
{
    public class RendererManager
    {
        private readonly List<RendererBase> theRenderers = new List<RendererBase>();

        private RendererBase theRenderer;

        public RendererManager()
        {
            RendererOpenGl4CSharp tempRendererOpenGl4 = new RendererOpenGl4CSharp();
            theRenderers.Add(tempRendererOpenGl4);
        }

        public List<RendererBase> TheRenderers
        {
            get { return theRenderers; }
        }

        public RendererBase TheRenderer
        {
            get { return theRenderer; }
        }

        public void SetRenderer(int aIndex, GameStatus aGameStatus, UserInputPlayer aUserInputPlayer)
        {
            if (theRenderer != null)
            {
                theRenderer.Close();
            }
            theRenderer = theRenderers[aIndex];
            theRenderer.TheGameStatus = aGameStatus;
            theRenderer.TheUserInputPlayer = aUserInputPlayer;
//            theRenderer.Start();
        }

        public void Start()
        {
            theRenderer.Start(); 
        }


        public void Close()
        {
            if (theRenderer != null)
            {
                theRenderer.Close();
            }          
        }
    }
}