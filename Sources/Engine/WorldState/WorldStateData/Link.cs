using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine.WorldState
{
    public class Link
    {
        Cell LinkedCell;
        int Cost;

        public Link(Cell cell, int cost)
        {
            LinkedCell = cell;
            Cost = cost;
        }


    }
}
