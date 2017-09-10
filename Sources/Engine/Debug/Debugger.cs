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
            if (ConsoleComponent == null) ConsoleComponent = GameObject.FindObjectOfType<Console>();
            StringBuilder msg = new StringBuilder();
            foreach (object element in elements)
            {
                msg.Append(element.ToString());
            }
            if (Enabled)
            {
                Debug.Log(msg);
                ConsoleComponent.WriteLine(msg.ToString(), consoleColor);
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