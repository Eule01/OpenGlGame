#region

using GameCore.GameObjects;
using GameCore.Render.OpenGlHelper;
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
            Location = theObjectGame.Location;
//            Orientation = RotationHelper.GetQuaternionFromDiretion(theObjectPlayer.Orientation);
            Orientation = RotationHelper.ReverseQuaternion(RotationHelper.GetQuaternionFromDiretion(theObjectPlayer.Orientation));
//            Orientation = Quaternion.FromAxis(theObjectPlayer.Orientation, Vector3.Zero, Vector3.Zero);
//            Orientation = Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
//                theObjectPlayer.Orientation.CalculateAngle(Vector3.Forward)));

            base.Draw();
        }
    }
}