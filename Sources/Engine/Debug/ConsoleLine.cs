using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloodAndBileEngine
{
    public struct ConsoleLine
    {
        public string Text;
        public Color TextColor;

        public ConsoleLine(string txt, Color col)
        {
            Text = txt;
            TextColor = col;
        }
    }
}
