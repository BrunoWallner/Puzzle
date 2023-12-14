using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleBauen
{
    public class OrientiertesTeil
    {
        public PuzzleTeil PuzzleTeil
        {
            get 
            {
                return PuzzleScanCollection.getInstance().getTeil(TeilNummer);
            }
        }
        public Seite getSeite(int index)
        {
            if (PuzzleTeil == null) return null;
            return PuzzleTeil.getSeite(index + orientierung);
        }
        public int TeilNummer
        {
            get { return teilnr; }
        }
        public int Orientierung
        {
            get { return orientierung; } 
        }
        public bool equals(OrientiertesTeil other)
        {
            if (this.Orientierung == other.Orientierung && this.TeilNummer == other.TeilNummer) return true;
            return false;
        }
        int teilnr;
        int orientierung;
        public OrientiertesTeil(int teilnr, int orientierung)
        {
            this.teilnr = teilnr;
            this.orientierung = orientierung;
        }
        public static OrientiertesTeil loadFromStream(Stream fs)
        {
            Int32 teilnr = Serializer.getInt32(fs);
            Int32 orientierung = Serializer.getInt32(fs);    
            return new OrientiertesTeil(teilnr, orientierung);
        }
        public void writeToStream(Stream fs)
        {
            Serializer.Add((Int32)teilnr, fs);
            Serializer.Add((Int32)orientierung, fs);
        }
        public override string ToString()
        {
            return "Nr: " + teilnr.ToString() + " Ori: " + orientierung.ToString();
        }
    }
}
