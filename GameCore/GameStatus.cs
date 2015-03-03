#region

using System.Collections.Generic;
using System.IO;
using GameCore.GameObjects;
using GameCore.Render.Cameras;
using GameCore.Render.RenderMaterial;
using OpenGL;

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
        internal List<ObjectGame> GameObjects;

        /// <summary>
        ///     The player object.
        /// </summary>
        internal ObjectPlayer ThePlayer;

        internal Environment TheEnvironment;


        /// <summary>
        ///     The millisecond run from start for this status.
        /// </summary>
        private ulong theMilliSeconds = 0;

        public GameStatus()
        {
            Init();
        }

        public Camera TheCamera;

        private void Init()
        {
            GameObjects = new List<ObjectGame>();
//            TheMap = Map.Map.CreateTestMap();
            theMilliSeconds = 0;
        }

        internal static GameStatus CreatTestGame()
        {
            GameStatus tempGameStatus = new GameStatus {TheMap = LoadMapObjectFromFile("test1")};
            //            GameStatus tempGameStatus = new GameStatus {TheMap = Map.Map.CreateTestMap()};
            ObjectPlayer tempPlayer = new ObjectPlayer(ObjectGame.ObjcetIds.Player)
            {
                Location = new Vector3(10.3f, 0.0f, 5.6f)
            };
            tempGameStatus.ThePlayer = tempPlayer;
            tempGameStatus.GameObjects.Add(tempPlayer);

            ObjectEnemy tempEnemy = new ObjectEnemy(ObjectGame.ObjcetIds.Enemy) {Location = new Vector3(15.0, 0.0, 20.0)};
            tempGameStatus.GameObjects.Add(tempEnemy);


            ObjectTurret tempTurret = new ObjectTurret(ObjectGame.ObjcetIds.Turret)
            {
                Location = new Vector3(20.0f, 0.0f, 20.0f),
                Orientation = new Vector3(0.0, 0.0f, 1.0),
                OrientationTower = 0.5f
//                OrientationTower = new Vector3(0.5, 0.0f, 0.5)
            };
            tempGameStatus.GameObjects.Add(tempTurret);


            tempTurret = new ObjectTurret(ObjectGame.ObjcetIds.Turret)
            {
                Location = new Vector3(30.0f, 0.0f, 25.0f),
                Orientation = new Vector3(0.0f, 0.0f, 1.0f),
                OrientationTower = 0.1f
//                OrientationTower = new Vector3(0.5f, 0.0f, 0.5f)
            };
            tempGameStatus.GameObjects.Add(tempTurret);

            tempTurret = new ObjectTurret(ObjectGame.ObjcetIds.Turret)
            {
                Location = new Vector3(31, 0.0f, 22),
                Orientation = new Vector3(0.0, 0.0f, 1.0),
                OrientationTower = 1.0f
//                OrientationTower = new Vector3(0.5, 0.0f, 0.5)
            };
            tempGameStatus.GameObjects.Add(tempTurret);

            tempTurret = new ObjectTurret(ObjectGame.ObjcetIds.Turret)
            {
                Location = new Vector3(27, 0.0f, 18),
                Orientation = new Vector3(0.0, 0.0f, 1.0),
                OrientationTower = 2.7f
//                OrientationTower = new Vector3(0.5, 0.0f, 0.5)
            };
            tempGameStatus.GameObjects.Add(tempTurret);

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

       public void LoadMapObject(string aFilePath)
        {
            TheMap = LoadMapObjectFromFile(aFilePath);
        }

        private static Map.Map LoadMapObjectFromFile(string aFilePath)
        {
            string filePath = ResourceManager.GetMapPath(aFilePath);

            return Map.Map.LoadFromMapObjectFiles(filePath);
        }
    }
}