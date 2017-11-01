using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine
{
    public class TestController : EntityComponent
    {
        public TestController(Entity linked) : base(linked)
        {

        }

        public override void Initialise()
        {
            
        }

        public override void Update(float deltaTime)
        {
            LinkedEntity.Position += UnityEngine.Vector3.forward * deltaTime;
        }

        public override uint GetComponentID()
        {
            return 1;
        }
    }
}
