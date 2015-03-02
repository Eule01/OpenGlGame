﻿#region

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CodeToast;
using GameCore.Engine;
using GameCore.GameObjects;
using GameCore.GuiHelpers;
using GameCore.Render.MainRenderer;
using GameCore.UserInterface;
using GameCore.Utils;
using GameCore.Utils.Timers;

#endregion

namespace GameCore
{
    /// <summary>
    ///     The core of the game. This is the main class that creates the game.
    /// </summary>
    public class GameCore : IFlowControl
    {
        private GameStatus theGameStatus;

        private GameEngine theGameEngine;

        private RendererBase theRenderer;

        private RendererManager theRendererManager;

        private KeyboardBindingsForm theKeyboardBindingsForm;

        private MenuForm theMenuForm;

        /// <summary>
        ///     This is holding the game core so it can be seen from all other classes. This is not best practice I guess.
        /// </summary>
        internal static GameCore TheGameCore;

        public GameCore()
        {
            Init();
        }

        private void Init()
        {
            TheGameEventHandler += GameCore_TheGameEventHandler;
            TheGameCore = this;
        }

        public GameStatus TheGameStatus
        {
            get { return theGameStatus; }
        }

        public RendererManager TheRendererManager
        {
            get { return theRendererManager; }
        }

        public RendererBase TheRenderer
        {
            get { return theRenderer; }
            set { theRenderer = value; }
        }

        public UserInputPlayer TheUserInputPlayer
        {
            get { return theGameEngine.TheUserInputPlayer; }
        }

        private void GameCore_TheGameEventHandler(object sender, GameEventArgs args)
        {
            switch (args.TheType)
            {
                case GameEventArgs.Types.StatusGameEngine:
                    break;
                case GameEventArgs.Types.Message:
                    break;
                case GameEventArgs.Types.MapLoaded:
                    break;
                case GameEventArgs.Types.MapSaved:
                    break;
                case GameEventArgs.Types.RendererExited:
                    break;
            }
        }

        /// <summary>
        ///     Raise a message.
        /// </summary>
        /// <param name="aMessage"></param>
        internal void RaiseMessage(string aMessage)
        {
            OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.Message){Message = aMessage});
        }

        #region Game flow

        public void Start()
        {
            theGameEngine = new GameEngine();
            theRendererManager = new RendererManager();

            //            TheGameStatus = new GameStatus();
            theGameStatus = GameStatus.CreatTestGame();
            SaveMap("test1");
//
//            LoadMap("test1");
//
            ObjectGame.TheGameStatus = theGameStatus;


            theGameEngine.Start();
            ChangeRenderer(0);
            ShowKeyboardBindingForm();
            ShowMenuForm();

        }

        public void ChangeRenderer(int aRendererIndex)
        {
            theGameEngine.Pause();
            theRendererManager.SetRenderer(aRendererIndex, theGameStatus, TheUserInputPlayer);
            theRenderer = theRendererManager.TheRenderer;
            theGameEngine.Resume();
            theRendererManager.Start();
        }


        public void Pause()
        {
            theGameEngine.Pause();
        }

        public void Resume()
        {
            theGameEngine.Resume();
        }

        public void Close()
        {
            if (theKeyboardBindingsForm != null)
            {
                Async.UI(delegate { theKeyboardBindingsForm.Close(); }, theKeyboardBindingsForm, false);
            }
            if (theMenuForm != null)
            {
                Async.UI(delegate { theMenuForm.Close(); }, theMenuForm, false);
            }
            theGameEngine.Close();
            theRendererManager.Close();
//            theRenderer.Close();
            theGameStatus.Close();
        }

        #endregion

        #region Commands

        public void SaveMap(string aFilePath)
        {
            string tempFilePath = FileNameToMapFileName(aFilePath);
            theGameStatus.SaveMap(tempFilePath);
            TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.MapSaved)
                {
                    Message = tempFilePath
                });
        }

        public void LoadMap(string aFilePath)
        {
            Pause();
            string tempFilePath = FileNameToMapFileName(aFilePath);
            theGameStatus.LoadMap(tempFilePath);
            theRenderer.MapLoaded();
            TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.MapLoaded)
                {
                    Message = tempFilePath
                });
            Resume();
        }

        public void MapToBitmap()
        {
            Map.Map.TestFromMapObject(theGameStatus.TheMap);
        }

        private static string FileNameToMapFileName(string aFilePath)
        {
            string tempPath = Path.GetDirectoryName(aFilePath);
            string aFileName = Path.GetFileNameWithoutExtension(aFilePath);
            aFileName = "Map_" + aFileName + ".xml";
            string tempFilePath = Path.Combine(tempPath, aFileName);
            return tempFilePath;
        }

        #endregion


        private void ShowMenuForm()
        {
            theMenuForm = new MenuForm(this);
            Async.Do(delegate
            {
                Application.Run(theMenuForm);
            });

            theMenuForm.Shown += delegate
            {
                Thread.Sleep(100);
                FormPositioner.PlaceNextToForm(theMenuForm, theKeyboardBindingsForm,FormPositioner.Locations.Left);
            };
        }


        private void ShowKeyboardBindingForm()
        {
            theKeyboardBindingsForm = new KeyboardBindingsForm(theRenderer.TheKeyBindings);
            Async.Do(delegate
            {
                Application.Run(theKeyboardBindingsForm); 
            });

            theKeyboardBindingsForm.Shown += delegate
            {
                FormPositioner.PlaceOnSecondScreenIfPossible(theKeyboardBindingsForm, FormPositioner.Locations.TopRight);

            };

            // Wait for the form to start up.
//            Thread.Sleep(200);
//            FormPositioner.PlaceOnSecondScreenIfPossible(theKeyboardBindingsForm, FormPositioner.Locations.TopRight);
        }

        #region Game Events

        public delegate void GameEventHandlerDel(object sender, GameEventArgs args);


        private GameEventHandlerDel gameEventHandlerDel;

        private readonly object eventLock = new object();

        public event GameEventHandlerDel TheGameEventHandler
        {
            add
            {
                lock (eventLock)
                {
                    // First try to remove the handler, then re-add it
                    gameEventHandlerDel -= value;
                    gameEventHandlerDel += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    gameEventHandlerDel -= value;
                }
            }
        }


        internal void OnGameEventHandler(GameEventArgs args)
        {
            GameEventHandlerDel handler = gameEventHandlerDel;
            if (handler != null) handler(this, args);
        }

        #endregion
    }
}