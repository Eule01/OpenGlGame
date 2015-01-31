using GameCore.UserInterface;

namespace GameCore.MainRenderer
{
    public abstract class RendererBase
    {
        public GameStatus TheGameStatus;

        public UserInput TheUserInput;

        protected string name = "RendererBase";

        public RendererBase()
        {
        }

        public abstract void Start();

        public abstract void Close();

        public string Name
        {
            get { return name; }
        }

        public override string ToString()
        {
            return name;
        }

        public abstract void MapLoaded();
    }
}