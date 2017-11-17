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
            foreach (Effect e in EntityEffects)
            {
                e.OnBirth(LinkedEntity.ID);
                if (e.Time <= 0f)
                    EntityEffects.Remove(e);
            }
        }

        public override void Update(float deltaTime)
        {
            List<Effect> DeadEffects = new List<Effect>();
            foreach (Effect e in EntityEffects)
            {
                e.Update(LinkedEntity.ID, deltaTime);
                if (e.Time <= 0f)
                    DeadEffects.Add(e);
            }
            foreach(Effect e in DeadEffects)
            {
                e.OnDeath(LinkedEntity.ID);
                EntityEffects.Remove(e);
            }
        }
    }
}
