#region

using GameCore.GameObjects;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public class ObjGroupGameObject : ObjGroup
    {
        protected ObjectGame theObjectGame;

        public ObjGroupGameObject(ShaderProgram program) : base(program)
        {
        }

        public ObjGroupGameObject(ObjGroup objGroup)
            : base(objGroup.defaultProgram)
        {
            Name = objGroup.Name;
            Location = objGroup.Location;
            Orientation = objGroup.Orientation;
            Scale = objGroup.Scale;
            Objects = objGroup.GetObjects();
        }

        public ObjectGame TheObjectGame
        {
            get { return theObjectGame; }
            set { theObjectGame = value; }
        }


        public override void Draw()
        {
            Location = new Vector3(theObjectGame.Location.X, 0.0, theObjectGame.Location.Y);
            switch (theObjectGame.TheObjectId)
            {
//                case ObjectGame.ObjcetIds.Player:
//                    Orientation =
//                        Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
//                            -((ObjectPlayer) theObjectGame).Orientation.Angle));
//
//                    break;
                case ObjectGame.ObjcetIds.Zork:
                    break;
                case ObjectGame.ObjcetIds.Gustav:
                    break;
                case ObjectGame.ObjcetIds.Turret:
                    Orientation =
                        Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
                            -((ObjectTurret) theObjectGame).Orientation.Angle));
                    break;
            }

            base.Draw();
        }
    }
}