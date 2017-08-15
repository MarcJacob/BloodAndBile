using System;
using System.Collections.Generic;
using UnityEngine;

public static class Debugger
{
    static bool Enabled = true;
    static public void Log(string msg)
    {
        if (Enabled)
            Debug.Log(msg);
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