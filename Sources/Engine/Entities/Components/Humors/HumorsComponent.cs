using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.WorldState;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Définit les humeurs d'une entité, provoque la destruction de l'entité sous certaines conditions.
/// </summary>
namespace BloodAndBileEngine
{
    public class HumorsComponent : EntityComponent, IEntitySynchroniser
    {
        int Blood;
        int Phlegm;
        int YellowBile;
        int BlackBile;

        int LOP;

        bool DieOfUnbalance = true;

        public override void Initialise(WorldState.WorldState worldState)
        {
            
        }

        public void SetHumors(int b, int p, int y, int bb)
        {
            Blood = b;
            Phlegm = p;
            YellowBile = y;
            BlackBile = bb;

            RecalculateLOP();
        }

        public void SetDieOfUnbalance(bool dieOfUnbalance)
        {
            DieOfUnbalance = dieOfUnbalance;
        }

        public override void Update(float deltaTime)
        {
            
        }

        public int GetLOP()
        {
            return LOP;
        }

        void RecalculateLOP()
        {
            LOP = Blood + Phlegm + YellowBile + BlackBile;

            if (DieOfUnbalance && LOP > 0)
            {
                float bloodRatio = Blood / LOP;
                float phlegmRatio = Phlegm / LOP;
                float yellowBileRatio = YellowBile / LOP;
                float blackBileRatio = BlackBile / LOP;
                if (bloodRatio > 0.6f || phlegmRatio > 0.6f || yellowBileRatio > 0.6f || blackBileRatio > 0.6f)
                {
                    LinkedEntity.Destroy();
                }
                else
                {
                    // On compte le nombre de ratios au dessus de 0.4f.
                    int overUnbalanceRatio = new[] { bloodRatio > 0.4f, phlegmRatio > 0.4f, yellowBileRatio > 0.4f, blackBileRatio > 0.4f }.Count(x => x);
                    if (overUnbalanceRatio >= 2)
                    {
                        LinkedEntity.Destroy();
                    }
                }
            }
            else if (LOP == 0)
            {
                LinkedEntity.Destroy();
            }
        }

        public int GetBlood()
        {
            return Blood;
        }

        public void ChangeBlood(int amount)
        {
            Blood += amount;
            RecalculateLOP();
        }

        public int GetPhlegm()
        {
            return Phlegm;
        }

        public void ChangePhlegm(int amount)
        {
            Phlegm += amount;
        }

        public int GetYellowBile()
        {
            return YellowBile;
        }

        public void ChangeYellowBile(int amount)
        {
            YellowBile += amount;
        }

        public int GetBlackBile()
        {
            return BlackBile;
        }

        public void ChangeBlackBile(int amount)
        {
            BlackBile += amount;
        }

        public void RemoveHumors(float[] amounts)
        {
            ChangeBlood(-(int)amounts[0]);
            ChangePhlegm(-(int)amounts[1]);
            ChangeYellowBile(-(int)amounts[2]);
            ChangeBlackBile(-(int)amounts[3]);
        }

        public StateUpdateObject[] GetSynchInfo()
        {
            StateUpdateObject humors = new StateUpdateObject("Humors", new int[] { Blood, Phlegm, YellowBile, BlackBile });
            return new StateUpdateObject[] { humors };
        }

        public void OnSynch(ComponentSynchronizationDataObject data)
        {
            int[] humors = (int[])data.GetSynchInfo("Humors");
            Blood = humors[0];
            Phlegm = humors[1];
            YellowBile = humors[2];
            BlackBile = humors[3];
        }


    }
}
