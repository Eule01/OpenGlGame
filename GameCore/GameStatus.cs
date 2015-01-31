#region

using System.Collections.Generic;
using System.IO;
using GameCore.GameObjects;
using GameCore.Utils;

#endregion

namespace GameCore
{
    public class GameStatus
    {
        /// <summary>
        ///     The tile map of this status.
        /// </summary>
        internal Map.Map TheMap;

        /// <summary>
        ///     This contains all the game objects.
        /// </summary>
        internal List<GameObject> GameObjects;


        internal ObjectPlayer ThePlayer;

        /// <summary>
        ///     The millisecond run from start for this status.
        /// </summary>
        private ulong theMilliSeconds = 0;

        public GameStatus()
        {
            Init();
        }

        private void Init()
        {
            GameObjects = new List<GameObject>();
            TheMap = Map.Map.GetTestMap();
            theMilliSeconds = 0;
        }

        internal static GameStatus CreatTestGame()
        {
            GameStatus tempGameStatus = new GameStatus {TheMap = Map.Map.GetTestMap()};
            ObjectPlayer tempPlayer = new ObjectPlayer(GameObject.ObjcetIds.Player) {Location = new Vector(10.3, 5.6)};
            tempGameStatus.ThePlayer = tempPlayer;
            tempGameStatus.GameObjects.Add(tempPlayer);
            return tempGameStatus;
        }


        /// <summary>
        ///     Loads a GameStatus from file
        /// </summary>
        /// <param name="aGamePath"></param>
        /// <returns></returns>
        public static GameStatus Load(string aGamePath)
        {
            return null;
        }

        /// <summary>
        ///     Saves a GameStatus to file
        /// </summary>
        /// <param name="aGamePath"></param>
        public static void Save(string aGamePath)
        {
        }


        /// <summary>
        ///     Closes this object and disposes everything.
        /// </summary>
        public void Close()
        {
            TheMap.Close();
        }

        public void SaveMap(string aFilePath)
        {
            Map.Map.SaveMap(aFilePath, TheMap);
        }

        public void LoadMap(string aFilePath)
        {
            TheMap = Map.Map.LoadMap(aFilePath);
        }

        private static string FileNameToMapFileName(string aFilePath)
        {
            string tempPath = Path.GetDirectoryName(aFilePath);
            string aFileName = Path.GetFileNameWithoutExtension(aFilePath);
            aFileName = "Map_" + aFileName + ".xml";
            string tempFilePath = Path.Combine(tempPath, aFileName);
            return tempFilePath;
        }
    }
}