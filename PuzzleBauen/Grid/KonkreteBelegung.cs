using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleBauen
{
    public class KonkreteBelegung
    {
        public KonkreteBelegung(int x, int y, int id, int orientierung)
        {
            this.x = x;
            this.y = y;
            this.id = id;
            this.orientierung = orientierung;

        }
        public OrientiertesTeil getTeil()
        {
            return new OrientiertesTeil(id, orientierung);
        }
        public int x;
        public int y;
        public int id;
        public int orientierung;

    }
}
