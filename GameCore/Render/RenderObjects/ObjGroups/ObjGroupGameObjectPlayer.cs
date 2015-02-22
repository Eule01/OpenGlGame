#region

using GameCore.GameObjects;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public class ObjGroupGameObjectPlayer : ObjGroupGameObject
    {
        private ObjectPlayer theObjectPlayer;

        public ObjGroupGameObjectPlayer(ShaderProgram program) : base(program)
        {
        }

        public ObjGroupGameObjectPlayer(ObjGroup objGroup) : base(objGroup)
        {
        }

        public new ObjectGame TheObjectGame
        {
            get { return theObjectGame; }
            set
            {
                theObjectGame = value;
                theObjectPlayer = (ObjectPlayer) theObjectGame;
            }
        }

        public override void Draw()
        {
            Location = new Vector3(theObjectGame.Location.X, 0.0, theObjectGame.Location.Y);
            Orientation =
                Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
                    -theObjectPlayer.Orientation.Angle));

            base.Draw();
        }
    }
}