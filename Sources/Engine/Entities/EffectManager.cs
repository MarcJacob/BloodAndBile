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
                AddEffect(e);
        }


        public override void Initialise() {}

        public override void Update(float deltaTime)
        {
            foreach (Effect e in EntityEffects)
            {
                e.Update(LinkedEntity.ID, deltaTime);
                if (e.Time <= 0f)
                    RemoveEffect(e);
            }
        }

        public void AddEffect(Effect e)
        {
            EntityEffects.Add(e);
            e.OnBirth(LinkedEntity.ID);
            if (e.Time <= 0f)
                RemoveEffect(e);
        }

        public void RemoveEffect(Effect e)
        {
            e.OnDeath(LinkedEntity.ID);
            EntityEffects.Remove(e);
        }
    }
}
