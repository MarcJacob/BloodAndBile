using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace BloodAndBileEngine
{
    public static class Debugger
    {
        static bool Enabled = true;
        static Console ConsoleComponent;
        /**
         * Envoie d'un message vers la console Unity et la console de développeur.
         */
        static public void Log(object[] elements, Color consoleColor)
        {
            if (elements == null || elements.Length == 0)
            {
                Log("ERREUR : Pas d'élements !", UnityEngine.Color.red);
                return;
            }
            if (ConsoleComponent == null) ConsoleComponent = GameObject.FindObjectOfType<Console>();
            StringBuilder msg = new StringBuilder();
            Debug.Log("Ajout de " + elements.Length + " éléments");
            foreach (object element in elements)
            {
                if (element != null)
                {
                    msg.Append(element.ToString());
                }
                else
                {
                    Log("ERREUR : élement null !", UnityEngine.Color.red);
                }
            }
            if (Enabled)
            {
                Debug.Log(msg);
                if (ConsoleComponent != null)
                    ConsoleComponent.WriteLine(msg.ToString(), consoleColor);
                else
                    Debug.Log("ERREUR : Pas de component Console !");
            }
        }

        static public void Log(object[] elements)
        {
            Log(elements, Color.green);
        }

        static public void Log(string msg, Color col)
        {
            Log(new object[] { msg }, col);
        }

        static public void Log(string msg)
        {
            Log(msg, Color.green);
        }

        static void Enable()
        {
            Enabled = true;
        }

        static void Disable()
        {
            Enabled = false;
        }
    }
}