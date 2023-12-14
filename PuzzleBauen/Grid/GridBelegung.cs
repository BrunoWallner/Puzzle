using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleBauen
{
    public class GridBelegung
    {
        public bool isRand
        {
            get { return isrand; }
        }
        public OrientiertesTeil OrientiertesTeil
        {
            get { return orientiertesTeil; }
        }
        public GridBelegung(bool isrand, OrientiertesTeil orientiertesTeil)
        {
            this.isrand = isrand;
            this.orientiertesTeil = orientiertesTeil;
        }
        OrientiertesTeil orientiertesTeil;
        bool isrand;
        public void writeToStream(Stream stream)
        {
            Serializer.Add(isrand ? 1 : 0, stream);
            if (orientiertesTeil == null)
            {
                Serializer.Add(0, stream);
            }
            else
            {
                Serializer.Add(1, stream);
                orientiertesTeil.writeToStream(stream);
            }
        }
        public static GridBelegung loadFromStream(Stream stream)
        {
            int isran = Serializer.getInt32(stream);
            int isOr = Serializer.getInt32(stream);
            OrientiertesTeil ot = null;
            if (isOr == 1)
            {
                ot = OrientiertesTeil.loadFromStream(stream);
            }
            return new GridBelegung(isran == 1, ot);
        }

    }
}
