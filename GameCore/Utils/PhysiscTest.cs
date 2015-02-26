using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace GameCore.Utils
{
    public class PhysiscTest
    {
        private Vector3 acceleration;
        private float mass;
        private float time;
        private Vector3 position;
        private Vector3 velocity;
        private Vector3 newAcceleration;

        /// <summary>
        /// http://gamedev.stackexchange.com/questions/15708/how-can-i-implement-gravity
        /// 
        /// http://buildnewgames.com/gamephysics/
        /// </summary>
        /// <param name="timestep"></param>
        private void Update(float timestep)
        {
            acceleration = force(time, position, velocity)/mass;
            time += timestep;
            position += timestep*(velocity + timestep*acceleration/2);
            velocity += timestep*acceleration;
            newAcceleration = force(time, position, velocity)/mass;
            velocity += timestep*(newAcceleration - acceleration)/2;
        }

        private Vector3 force(float aTime, Vector3 aPosition, Vector3 aVelocity)
        {
            
            return new Vector3(0,0,0);
        }
    }
}
