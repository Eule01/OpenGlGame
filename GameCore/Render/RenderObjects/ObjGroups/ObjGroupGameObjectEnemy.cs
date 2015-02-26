#region

using GameCore.GameObjects;
using GameCore.Render.OpenGlHelper;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public class ObjGroupGameObjectEnemy : ObjGroupGameObject
    {
        private ObjectEnemy theObjectEnemy;

        public ObjGroupGameObjectEnemy(ShaderProgram program) : base(program)
        {
        }

        public ObjGroupGameObjectEnemy(ObjGroup objGroup)
            : base(objGroup)
        {
        }

        public new ObjectGame TheObjectGame
        {
            get { return theObjectGame; }
            set
            {
                theObjectGame = value;
                theObjectEnemy = (ObjectEnemy)theObjectGame;
            }
        }

        public override void Draw()
        {
            Location = theObjectGame.Location;
//            Orientation = RotationHelper.GetQuaternionFromDiretion(theObjectEnemy.Orientation);
            Orientation = RotationHelper.ReverseQuaternion(RotationHelper.GetQuaternionFromDiretion(theObjectEnemy.Orientation));
//            Orientation = Quaternion.FromAxis(theObjectEnemy.Orientation, Vector3.Zero, Vector3.Zero);
//            Orientation = Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
//                theObjectEnemy.Orientation.CalculateAngle(Vector3.Forward)));

            base.Draw();
        }
    }
}