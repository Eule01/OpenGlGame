#region

using GameCore.GameObjects;
using GameCore.Render.OpenGlHelper;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public class ObjGroupGameObjectTurret : ObjGroupGameObject
    {
        private ObjectTurret theObjectTurret;
        private readonly IObjObject objTurretBase;
        private readonly IObjObject objTurretTower;


        public ObjGroupGameObjectTurret(ShaderProgram program) : base(program)
        {

        }

        public ObjGroupGameObjectTurret(ObjGroup objGroup) : base(objGroup)
        {
            objTurretTower = Objects[0];
            objTurretBase = Objects[1];
        }

        public new ObjectGame TheObjectGame
        {
            get { return theObjectGame; }
            set
            {
                theObjectGame = value;
                theObjectTurret = (ObjectTurret) theObjectGame;
            }
        }

        public override void Draw()
        {
            Location = theObjectGame.Location;
//            Orientation = Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
//                    -theObjectTurret.Orientation.Angle));
            Orientation = RotationHelper.ReverseQuaternion(RotationHelper.GetQuaternionFromDiretion(theObjectTurret.Orientation));
//
//            Orientation = Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
//                theObjectTurret.Orientation.CalculateAngle(Vector3.Forward)));

            if (modelMatrixOld)
            {
                UpdateModelMatrix();
            }
            defaultProgram["model_matrix"].SetValue(modelMatrix);
            objTurretBase.Draw();
//            Matrix4 towerModelMatrix = (RotationHelper.ReverseQuaternion(RotationHelper.GetQuaternionFromDiretion(-theObjectTurret.OrientationTower))).Matrix4 * modelMatrix;
//            Matrix4 towerModelMatrix = theObjectTurret.OrientQuaternion.Matrix4 * modelMatrix;
//            Matrix4 towerModelMatrix = Matrix4.CreateFromAxisAngle(Vector3.Up, theObjectTurret.OrientationTower.CalculateAngle(Vector3.Forward)) * modelMatrix;
//            Matrix4 towerModelMatrix = Matrix4.CreateFromAxisAngle(Vector3.Up, theObjectTurret.OrientationTower.CalculateAngle(Vector3.Forward)) * modelMatrix;
//            Matrix4 towerModelMatrix = Matrix4.CreateRotationY(theObjectTurret.OrientationTower) * modelMatrix;
            Matrix4 towerModelMatrix = Matrix4.CreateRotationY(theObjectTurret.OrientationTower) * modelMatrix;
            defaultProgram["model_matrix"].SetValue(towerModelMatrix);
            objTurretTower.Draw();
        }
    }
}