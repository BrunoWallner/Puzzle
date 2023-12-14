using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleBauen
{
    public class NeighbourSide
    {
        public bool isRand;
        public Seite seite;
        public NeighbourSide(bool isRand, Seite seite)
        {
            this.isRand = isRand;
            this.seite = seite;
        }
    }
}
