#region

using System.Drawing;
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