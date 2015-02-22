#region

using GameCore.GameObjects;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public class ObjGroupGameObjectTurrel : ObjGroupGameObject
    {
        private ObjectTurret theObjectTurret;
        private IObjObject objTurretBase;
        private IObjObject objTurretTower;


        public ObjGroupGameObjectTurrel(ShaderProgram program) : base(program)
        {

        }

        public ObjGroupGameObjectTurrel(ObjGroup objGroup) : base(objGroup)
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
            Location = new Vector3(theObjectGame.Location.X, 0.0, theObjectGame.Location.Y);
            Orientation =
                Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
                    -theObjectTurret.Orientation.Angle));

            if (modelMatrixOld)
            {
                UpdateModelMatrix();
            }
            defaultProgram["model_matrix"].SetValue(modelMatrix);
            objTurretBase.Draw();
            Matrix4 towermodelMatrix = Matrix4.CreateFromAxisAngle(Vector3.Up, -theObjectTurret.OrientationTower.Angle)*modelMatrix;
            defaultProgram["model_matrix"].SetValue(towermodelMatrix);
            objTurretTower.Draw();
        }
    }
}