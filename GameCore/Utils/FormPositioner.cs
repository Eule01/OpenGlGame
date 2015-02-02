#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CodeToast;

#endregion

namespace GameCore.Utils
{
    public static class FormPositioner
    {
        public enum Locations
        {
            Top,
            Bottom,
            TopLeft,
            TopRight,
            Right,
            Left
        }

        public static Rectangle GetPrimaryScreeenBound()
        {
            Screen firstPrimaryScreen = GetFirstPrimaryScreen();
            Rectangle bounds = firstPrimaryScreen.WorkingArea;
            return bounds;
        }


        public static void PlaceOnSecondScreenIfPossible(Form aForm, Locations aLocations, bool fillWidth = false)
        {
            Screen firstNonPrimaryScreen = GetFirstNonPrimaryScreen();
            PlaceOnScreen(aForm, aLocations, fillWidth, firstNonPrimaryScreen);
        }



        const int SWP_NOSIZE = 0x0001;


        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static IntPtr MyConsole = GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

//        http://stackoverflow.com/questions/1548838/setting-position-of-a-console-window-opened-in-a-winforms-app/1548881#1548881
//        http://blog.csharplearners.com/2012/01/14/working-console-window-c/
        /// <summary>
        /// Kind of works:
        ///        http://stackoverflow.com/questions/1548838/setting-position-of-a-console-window-opened-in-a-winforms-app/1548881#1548881
        ///        http://blog.csharplearners.com/2012/01/14/working-console-window-c/
        /// </summary>
        /// <param name="maximise"></param>
        public static void PlaceConsoleOnSecondScreenIfPossible(bool maximise = false)
        {
            Screen firstNonPrimaryScreen = GetFirstNonPrimaryScreen();
            Rectangle bounds = firstNonPrimaryScreen.WorkingArea;
            SetWindowPos(MyConsole, 0, bounds.Left, bounds.Top, 0, 0, SWP_NOSIZE);

//           Console.SetWindowPosition(0,0);
//           Console.WindowHeight = bounds.Height;
        }


        public static void PlaceOnPrimaryScreen(Form aForm, Locations aLocations, bool fillWidth = false)
        {
            Screen firstPrimaryScreen = GetFirstPrimaryScreen();

            PlaceOnScreen(aForm, aLocations, fillWidth, firstPrimaryScreen);
        }

        private static Screen GetFirstPrimaryScreen()
        {
            Screen[] screens = Screen.AllScreens;
            Screen firstPrimaryScreen = screens[0];
            if (screens.Length > 1) // If there is more than one get the first primary screen.
            {
                foreach (Screen aScreen in screens)
                {
                    if (aScreen.Primary)
                    {
                        firstPrimaryScreen = aScreen;
                        break;
                    }
                }
            }
            return firstPrimaryScreen;
        }

        private static Screen GetFirstNonPrimaryScreen()
        {
            Screen[] screens = Screen.AllScreens;
            Screen firstNonPrimaryScreen = screens[0];
            if (screens.Length > 1) // If there is more than one get the first non primary screen.
            {
                for (int i = 0; i < screens.Length; i++)
                {
                    Screen aScreen = screens[i];
                    if (!aScreen.Primary)
                    {
                        firstNonPrimaryScreen = aScreen;
                        break;
                    }
                }
            }
            return firstNonPrimaryScreen;
        }

        private static void PlaceOnScreen(Form aForm, Locations aLocations, bool fillWidth, Screen aScreen)
        {
            Rectangle bounds = aScreen.WorkingArea;
            int destWidth = fillWidth ? bounds.Width : aForm.Width;
            switch (aLocations)
            {
                case Locations.Top:
                    Async.UI(delegate
                        {
                            aForm.DesktopBounds = new Rectangle(bounds.Left,
                                                                0,
                                                                destWidth, aForm.Height);
                        }, aForm, true);

                    break;
                case Locations.Bottom:
                    Async.UI(delegate
                        {
                            aForm.DesktopBounds = new Rectangle(bounds.Left,
                                                                bounds.Height - aForm.Height,
                                                                destWidth, aForm.Height);
                        }, aForm, true);

                    break;
                case Locations.TopLeft:
                    Async.UI(delegate
                        {
                            aForm.DesktopBounds = new Rectangle(bounds.Left,
                                                                0,
                                                                destWidth, aForm.Height);
                        }, aForm, true);

                    break;
                default:
                    break;
            }
        }

        public static void PlaceNextToForm(Form aFormToPlace, Form aForm, Locations aLocations)
        {
            //            Rectangle bounds = aScreen.WorkingArea;
            //            int destWidth = fillWidth ? bounds.Width : aFormToPlace.Width;
            switch (aLocations)
            {
                case Locations.Top:
                    Async.UI(delegate
                    {
                        aFormToPlace.DesktopBounds = new Rectangle(aForm.Left,
                                                            aForm.Top - aFormToPlace.Height,
                                                            aFormToPlace.Width, aFormToPlace.Height);
                    }, aFormToPlace, true);

                    break;
                case Locations.Bottom:
                    Async.UI(delegate
                    {
                        aFormToPlace.DesktopBounds = new Rectangle(aForm.Left,
                                                            aForm.Bottom,
                                                            aFormToPlace.Width, aFormToPlace.Height);
                    }, aFormToPlace, true);

                    break;
                case Locations.Left:
                    Async.UI(delegate
                    {
                        aFormToPlace.DesktopBounds = new Rectangle(aForm.Left - aFormToPlace.Width,
                                                            aForm.Top,
                                                           aFormToPlace.Width, aFormToPlace.Height);
                    }, aFormToPlace, true);

                    break;
                case Locations.Right:
                    Async.UI(delegate
                    {
                        aFormToPlace.DesktopBounds = new Rectangle(aForm.Right,
                                                             aForm.Top,
                                                           aFormToPlace.Width, aFormToPlace.Height);
                    }, aFormToPlace, true);

                    break;
                default:
                    break;
            }
        }
    }
}