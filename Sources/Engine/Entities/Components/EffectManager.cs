using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodAndBileEngine
{
    public class EffectManager : EntityComponent
    {
        private List<Effect> EntityEffects;


        public EffectManager()
        {
            EntityEffects = new List<Effect>();
        }


        public override void Initialise(BloodAndBileEngine.WorldState.WorldState worldState) {}

        public override void Update(float deltaTime)
        {
            foreach (Effect e in EntityEffects)
            {
                e.Update((int)LinkedEntity.ID, deltaTime);
                if (e.Time <= 0f)
                    RemoveEffect(e);
            }
        }

        public void AddEffect(Effect e)
        {
            EntityEffects.Add(e);
            e.OnBirth((int)LinkedEntity.ID);
            if (e.Time <= 0f)
                RemoveEffect(e);
        }

        public void RemoveEffect(Effect e)
        {
            e.OnDeath((int)LinkedEntity.ID);
            EntityEffects.Remove(e);
        }
    }
}
