using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine
{
    public class TestController : EntityComponent
    {
        public TestController()
        {

        }

        public override void Initialise()
        {
            
        }

        public override void Update(float deltaTime)
        {
            LinkedEntity.Position += UnityEngine.Vector3.forward * deltaTime;
            UnityEngine.Quaternion newRot = LinkedEntity.Rotation;
            newRot.eulerAngles += new UnityEngine.Vector3(0, 1f * deltaTime, 0);
            LinkedEntity.Rotation = newRot;
        }
    }
}
