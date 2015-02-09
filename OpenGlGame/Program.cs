#region

using System;
using System.Threading;
using GameCore;
using GameCore.Utils;

#endregion

namespace OpenGlGame
{
    internal class Program
    {
        private static GameCore.GameCore theGameCore;

        private static ManualResetEvent waitForClose = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            FormPositioner.PlaceConsoleOnSecondScreenIfPossible();

            theGameCore = new GameCore.GameCore();
            theGameCore.TheGameEventHandler += theGameCore_TheGameEventHandler;
            theGameCore.Start();
            waitForClose.WaitOne();
//            Console.ReadLine();
            theGameCore.TheGameEventHandler -= theGameCore_TheGameEventHandler;
        }

        private static void theGameCore_TheGameEventHandler(object sender, GameEventArgs args)
        {
            Console.WriteLine(args);
            switch (args.TheType)
            {
                case GameEventArgs.Types.Status:
                    break;
                case GameEventArgs.Types.Message:
                    break;
                case GameEventArgs.Types.MapLoaded:
                    break;
                case GameEventArgs.Types.MapSaved:
                    break;
                case GameEventArgs.Types.RendererExited:
                    theGameCore.Close();
                    waitForClose.Set();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


        }
    }
}