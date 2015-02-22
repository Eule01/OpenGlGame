#region

using GameCore.GameObjects;
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
            Location = new Vector3(theObjectGame.Location.X, 0.0, theObjectGame.Location.Y);
            Orientation = Quaternion.FromRotationMatrix(Matrix4.CreateFromAxisAngle(Vector3.Up,
                    -theObjectTurret.Orientation.Angle));

            if (modelMatrixOld)
            {
                UpdateModelMatrix();
            }
            defaultProgram["model_matrix"].SetValue(modelMatrix);
            objTurretBase.Draw();
            Matrix4 towerModelMatrix = Matrix4.CreateFromAxisAngle(Vector3.Up, -theObjectTurret.OrientationTower.Angle)*modelMatrix;
            defaultProgram["model_matrix"].SetValue(towerModelMatrix);
            objTurretTower.Draw();
        }
    }
}