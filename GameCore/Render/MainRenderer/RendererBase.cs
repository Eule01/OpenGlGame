using GameCore.Render.RenderMaterial;
using GameCore.UserInterface;

namespace GameCore.Render.MainRenderer
{
    public abstract class RendererBase
    {
        public GameStatus TheGameStatus;

        public UserInputPlayer TheUserInputPlayer;

        protected string name = "RendererBase";
        protected MaterialManager theMaterialManager;

        public RendererBase()
        {
        }

        public abstract void Start();

        public abstract void Close();

        public string Name
        {
            get { return name; }
        }

        public MaterialManager TheMaterialManager
        {
            get { return theMaterialManager; }
        }

        public override string ToString()
        {
            return name;
        }

        public abstract void MapLoaded();
    }
}