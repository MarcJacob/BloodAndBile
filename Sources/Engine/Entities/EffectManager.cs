using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodAndBileEngine
{
    public class EffectManager : EntityComponent
    {
        private List<Effect> EntityEffects;


        public EffectManager(Entity linked) : base(linked)
        {
            EntityEffects = new List<Effect>();
        }

        public EffectManager(Entity linked, List<Effect> effects) : base(linked)
        {
            EntityEffects = new List<Effect>();
            foreach (Effect e in effects)
                EntityEffects.Add(e);
        }


        public override void Initialise()
        {
        }

        public override void Update(float deltaTime)
        {
            foreach (Effect e in EntityEffects)
            {
                e.EffectAction(LinkedEntity.ID);
                e.AddTime(-deltaTime);
                if (e.Time <= 0f)
                    EntityEffects.Remove(e);
            }
        }
    }
}
