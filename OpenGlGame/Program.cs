using GameCore ;

namespace OpenGlGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GameCore.GameCore theGameCore = new GameCore.GameCore();

            theGameCore.Start();
        }
    }
}