using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine.WorldState
{
    public class Link
    {
        public int LinkedCellID { get; private set; }
        public int Cost { get; private set; }

        public Link(int cellID, int cost)
        {
            LinkedCellID = cellID;
            Cost = cost;
        }

    }
}