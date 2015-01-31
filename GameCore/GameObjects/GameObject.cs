#region

using System;
using System.Collections.Generic;
using System.Drawing;
using GameCore.Utils;

#endregion

namespace GameCore.GameObjects
{
    public class GameObject
    {
        public bool changed = true;

        public enum ObjcetIds
        {
            Player,
            Zork,
            Gustav,
        }

        public float Diameter = 0.4f;

        private Vector location = new Vector(0, 0);

        public Vector Location
        {
            get { return location; }
            set
            {
                location = value;
                changed = true;
            }
        }

        private ObjcetIds theObjectId = ObjcetIds.Player;

        public GameObject(ObjcetIds aObjectId)
        {
            theObjectId = aObjectId;
        }

        public ObjcetIds TheObjectId
        {
            get { return theObjectId; }
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