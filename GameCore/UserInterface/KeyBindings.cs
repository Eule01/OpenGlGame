#region

using System;
using System.Collections.Generic;
using Tao.FreeGlut;

#endregion

namespace GameCore.UserInterface
{
    public class KeyBindings
    {
        private List<KeyBinding> theKeyBindings = new List<KeyBinding>();
        private Dictionary<Ids, int> theKeyLookUp;

        public enum Ids
        {
            PlayerForward,
            PlayerBackward,
            PlayerRight,
            PlayerLeft,

            CameraForward,
            CameraBackward,
            CameraRight,
            CameraLeft,
            CameraUp,
            CameraDown,
            CameraTurnAtPlayer,
            CameraTurnAtField,

            DisplayToggleRenderWireFrame,
            DisplayToggleLighting,
            DisplayToggleLightingRotate,
            DisplayToggleFullFrame,
            DisplayToggleDisplayInfo,

            GameExit
        }

        public KeyBindings()
        {
        }

        public void Initialise()
        {
            CreateLookup();
        }

        private void CreateLookup()
        {
            theKeyLookUp = new Dictionary<Ids, int>();
            foreach (KeyBinding aKeyBinding in theKeyBindings)
            {
                theKeyLookUp.Add(aKeyBinding.Id, aKeyBinding.Key);
            }
        }


        public static KeyBindings GetDefaultKeyBindings()
        {
            KeyBindings tempKeyBindings = new KeyBindings();

            List<KeyBinding> tempBindingList = new List<KeyBinding>
                {
                    new KeyBinding(Ids.GameExit, 27, "Exit game", "Esc"),
                    
                    new KeyBinding(Ids.CameraForward, Glut.GLUT_KEY_UP, "Move camera forward", "Arrow Up"),
                    new KeyBinding(Ids.CameraBackward, Glut.GLUT_KEY_DOWN, "Move camera backward", "Arrow Down"),
                    new KeyBinding(Ids.CameraRight, Glut.GLUT_KEY_RIGHT, "Move camera right", "Arrow Right"),
                    new KeyBinding(Ids.CameraLeft, Glut.GLUT_KEY_LEFT, "Move camera Left", "Arrow Left"),
                    new KeyBinding(Ids.CameraUp, Glut.GLUT_KEY_PAGE_UP, "Move camera up", "Page Up"),
                    new KeyBinding(Ids.CameraDown, Glut.GLUT_KEY_PAGE_DOWN, "Move camera down", "Page Down"),
                    new KeyBinding(Ids.CameraTurnAtPlayer, Glut.GLUT_KEY_HOME, "Turn camera at player", "Home"),
                    new KeyBinding(Ids.CameraTurnAtField, Glut.GLUT_KEY_END, "Turn camera at playing filed", "End"),

                    new KeyBinding(Ids.DisplayToggleRenderWireFrame, (byte) 'q', "Toggle game layer render wire frame",
                                   "q"),
                    new KeyBinding(Ids.DisplayToggleLighting, (byte) 'e', "Toggle game layer lighting",
                                   "e"),
                    new KeyBinding(Ids.DisplayToggleLightingRotate, (byte) 'r', "Toggle game layer lighting to rotate",
                                   "r"),
                    new KeyBinding(Ids.DisplayToggleFullFrame, (byte) 'f', "Toggle full frame", "f"),
                    new KeyBinding(Ids.DisplayToggleDisplayInfo, (byte) 'i', "Toggle display info", "i"),

                    new KeyBinding(Ids.PlayerForward, (byte) 'w', "Move player forward", "w"),
                    new KeyBinding(Ids.PlayerBackward, (byte) 's', "Move player backward", "s"),
                    new KeyBinding(Ids.PlayerLeft, (byte) 'a', "Move player left", "a"),
                    new KeyBinding(Ids.PlayerRight, (byte) 'd', "Move player right", "d")
                };


            tempKeyBindings.theKeyBindings = tempBindingList;
            return tempKeyBindings;
        }

        public List<KeyBinding> TheKeyBindings
        {
            get { return theKeyBindings; }
        }

        public Dictionary<Ids, int> TheKeyLookUp
        {
            get { return theKeyLookUp; }
        }

        public override string ToString()
        {
            string outStr = "";
            foreach (KeyBinding tempKeyBinding in theKeyBindings)
            {
                outStr += tempKeyBinding + System.Environment.NewLine;
            }
            return outStr;
        }
    }
}