using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

namespace BloodAndBileEngine
{
    public class TestController : EntityComponent, IEntitySynchroniser
    {
        public TestController()
        {

        }

        public float value = 0;

        public override void Initialise(BloodAndBileEngine.WorldState.WorldState worldState)
        {
            Mover = (EntityMover)LinkedEntity.GetComponent(typeof(EntityMover));
        }

        EntityMover Mover;

        public override void Update(float deltaTime)
        {
            Mover.Move(deltaTime, 0);
            UnityEngine.Quaternion newRot = LinkedEntity.Rotation;
            newRot.eulerAngles += new UnityEngine.Vector3(0, 1f * deltaTime, 0);
            LinkedEntity.Rotation = newRot;

            value += deltaTime;
        }

        public StateUpdateObject[] GetSynchInfo()
        {
            StateUpdateObject valueObject = new StateUpdateObject("Value", value);
            return new StateUpdateObject[] { valueObject };
        }

        public void OnSynch(ComponentSynchronizationDataObject data)
        {
            value = (float)data.GetSynchInfo("Value");
        }
    }
}
