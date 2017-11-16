using UnityEngine;
using UnityEditor;
using System;

namespace BloodAndBileEngine
{
    public class Effect
    {
        public float Time { get; private set; }
        public Action<int> EffectAction { get; private set; }

        public Effect(float time, Action<int> action) 
        {
            Time = time;
            EffectAction = action;
        }


        /// <summary>
        /// Permet d'ajouter du "temps d'existence" à l'effet. Retourne alors la nouvelle valeur.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public float AddTime(float deltaTime)
        {
            Time += deltaTime;
            return Time;
        }
    }
}