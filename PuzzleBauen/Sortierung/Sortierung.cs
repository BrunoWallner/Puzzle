using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleBauen
{
    public class Sortierung
    {
        public enum Sortierer
        {
            standard,
        }
        public static IComparer<TeilPassung> getSortierer(Sortierer sortierer)
        {
            switch (sortierer)
            {
                case Sortierer.standard:
                    return new ComparePassung();
                    break;
                default:
                    return null;
            }
        }
    }
    public class ComparePassung : IComparer<TeilPassung>
    {

        /// <summary>
        /// Gibt 0 zurück, wenn kein unterschied besteht
        /// Gibt -1 zurück, wenn Passung x besser ist
        /// Gibt +1 zurück, wenn Passung y besser ist
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int IComparer<TeilPassung>.Compare(TeilPassung? x, TeilPassung? y)
        {            
            if (!x.passtSeitenForm() && !y.passtSeitenForm()) return 0; //Es passt bei keinem die Form
            if (x.passtSeitenForm() && !y.passtSeitenForm()) return -1; // bei x passt die Form, bei y nicht
            if (!x.passtSeitenForm() && y.passtSeitenForm()) return 1; //bei y passt die Form, bei x nicht
            //if (x.passtSeitenForm() && y.passtSeitenForm()) return 0; 
            //Es passt bei beiden die Form
            double lengthDifSumX = 0;
            double lengthDifSumY = 0;                
            double comDifSumX = 0;
            double comDifSumY = 0;
            double areaDifSumX = 0;
            double areaDifSumY = 0;
            for(int i = 0; i<4; i++)
            {
                lengthDifSumX += x.seitenPassungen[i].lengthDist;
                lengthDifSumY += y.seitenPassungen[i].lengthDist;
                comDifSumX += x.seitenPassungen[i].comDistMid;
                comDifSumY += y.seitenPassungen[i].comDistMid;
                areaDifSumX += x.seitenPassungen[i].areaDist;
                areaDifSumY += y.seitenPassungen[i].areaDist;
            }
            if(comDifSumX + lengthDifSumX> comDifSumY + lengthDifSumY)
            {
                return 1;
            }
            if (comDifSumX + lengthDifSumX < comDifSumY + lengthDifSumY)
            {
                return -1;
            }
            return (areaDifSumX < areaDifSumY) ? -1 : 1;

        }
    }
}
