﻿#region

using System.Collections.Generic;
using System.Diagnostics;
using GameCore.GameObjects;
using GameCore.UserInterface;
using GameCore.Utils;
using GameCore.Utils.Timers;

#endregion

namespace GameCore.Engine
{
    public class GameEngine : IFlowControl
    {
        private ITickEngine theTickEngine;

        /// <summary>
        ///     The gamer time interval in milliseconds.
        /// </summary>
        private const int timerTickIntervalMs = 10;

        private UserInputPlayer theUserInputPlayer;

        private Stopwatch watch;
        private string saveMapName = string.Empty;

        public GameEngine()
        {
            Init();
        }

        /// <summary>
        ///     Initialise the game engine.
        /// </summary>
        private void Init()
        {
            theUserInputPlayer = new UserInputPlayer();
            ObjectGame.TheUserInputPlayer = theUserInputPlayer;
            theTickEngine = new TickEngineThread();
            theTickEngine.Setup("GameEngine", GameTick, StatusTick, timerTickIntervalMs);
            watch = Stopwatch.StartNew();
            watch.Stop();
        }

        public UserInputPlayer TheUserInputPlayer
        {
            get { return theUserInputPlayer; }
        }

        private void StatusTick(OpStatus opstatus)
        {
            GameCore.TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.StatusGameEngine)
                {
                    TheOpStatus = opstatus
                });
        }

        #region Game Timer

        /// <summary>
        ///     Here all the game action is computed. This is called every timerTickIntervalMs
        /// </summary>
        private void GameTick()
        {
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks / Stopwatch.Frequency;
            List<ObjectGame> theGameObjects = GameCore.TheGameCore.TheGameStatus.GameObjects;
            foreach (ObjectGame aGameObject in theGameObjects)
            {
                aGameObject.Move(deltaTime);
            }
            watch.Restart();
        }

        /// <summary>
        ///     Starts the game.
        /// </summary>
        public void Start()
        {
            watch.Restart();
            theTickEngine.Start();
        }

        /// <summary>
        ///     Shuts down the game.
        /// </summary>
        public void Close()
        {
            watch.Stop();
            theTickEngine.Close();
        }

        /// <summary>
        ///     Pauses the game.
        /// </summary>
        public void Pause()
        {
            watch.Stop();
            theTickEngine.Pause();
        }


        /// <summary>
        ///     Resumes the game.
        /// </summary>
        public void Resume()
        {
            watch.Start();
            theTickEngine.Resume();
        }

        #endregion

        
    }
}