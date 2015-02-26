#region

using System;
using System.Collections.Generic;
using System.Drawing;
using GameCore.UserInterface;
using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.GameObjects
{
    public class ObjectGame
    {
        public bool Changed = true;

        public static GameStatus TheGameStatus;
        public static UserInputPlayer TheUserInputPlayer;

        public enum ObjcetIds
        {
            Player,
            Zork,
            Gustav,
            Turret,
            Enemy
        }

        public float Diameter = 0.4f;

        private Vector3 location = new Vector3(0, 0,0);

        public Vector3 Location
        {
            get { return location; }
            set
            {
                location = value;
                Changed = true;
            }
        }

        private ObjcetIds theObjectId = ObjcetIds.Player;

        public ObjectGame(ObjcetIds aObjectId)
        {
            theObjectId = aObjectId;
        }

        public ObjcetIds TheObjectId
        {
            get { return theObjectId; }
        }

        public virtual void Move(float deltaTime)
        {
            
        }

        public static Dictionary<ObjcetIds, GameObjectType> GetObjTypes()
        {
            Dictionary<ObjcetIds, GameObjectType> tempList = new Dictionary<ObjcetIds, GameObjectType>();

            foreach (ObjcetIds objcetIds in (ObjcetIds[]) Enum.GetValues(typeof (ObjcetIds)))
            {
                GameObjectType tempType = new GameObjectType(objcetIds.ToString());
                switch (objcetIds)
                {
                    case ObjcetIds.Player:
                        tempType.Color = Color.DarkSeaGreen;
                        break;
                    case ObjcetIds.Gustav:
                        tempType.Color = Color.Red;
                        break;
                    case ObjcetIds.Zork:
                        tempType.Color = Color.Blue;
                        break;
                    default:
                        tempType.Color = Color.Gray;
                        break;
                }
                tempList.Add(objcetIds, tempType);
            }

            return tempList;
        }
    }

    public class GameObjectType
    {
        private static readonly Color defaultColor = Color.Gray;

        private string name;

        public Color Color = defaultColor;

        public GameObjectType(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }
}