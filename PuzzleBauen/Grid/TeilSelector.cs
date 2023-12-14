using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Documents;
using System.Windows;
using OpenCvSharp.Flann;
using System.Windows.Forms;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;

namespace PuzzleBauen
{

    public class SeitenPassung
    {
        public bool knubbelPasst = true;
        public double comDistMax = 0;
        public double comDistMin = 0;
        public double comDistMid = 0;
        public double lengthDist = 0;
        public double areaDist = 0;
        //public double area2 = 0;
        //public SeitenPassung(Seite s1, Seite s2)
        //{

        //    if (s1 == null || s2 == null) return;
        //    //if(s1.getType() == Seite.reverse(s2.getType()))
        //    //{

        //    //}                        
        //    double comDist1 = s1.getNormalizedCoM(true).DistanceTo(s2.getNormalizedCoM(false));
        //    double comDist2 = s2.getNormalizedCoM(true).DistanceTo(s1.getNormalizedCoM(false));
        //    comDistMax = (comDist1>comDist2)?comDist1: comDist2;
        //    comDistMax = (comDist1<comDist2)?comDist1 : comDist2;
        //    comDistMid = (comDistMax + comDistMin / 2.0);

        //    lengthDist = Math.Abs(s1.getLength() - s2.getLength());            
        //}
        public SeitenPassung(NeighbourSide s1, NeighbourSide s2)
        {
            if(s1.isRand && s2.isRand)
            {
                return;
            }
            if(s1.isRand || s2.isRand)
            {
                knubbelPasst = false;
                return;
            }       
            if(s1.seite == null || s2.seite == null)
            {
                return;
            }
            if(s1.seite.getType() != Seite.reverse(s2.seite.getType()))
            {
                knubbelPasst = false;
                return;
            }            
            double comDist1 = s1.seite.getNormalizedCoM(true).DistanceTo(s2.seite.getNormalizedCoM(false));
            double comDist2 = s2.seite.getNormalizedCoM(true).DistanceTo(s1.seite.getNormalizedCoM(false));
            comDistMax = (comDist1 > comDist2) ? comDist1 : comDist2;
            comDistMax = (comDist1 < comDist2) ? comDist1 : comDist2;
            comDistMid = (comDistMax + comDistMin / 2.0);
            areaDist = Math.Abs(s1.seite.getArea(false) - s2.seite.getArea(false));            
            lengthDist = Math.Abs(s1.seite.getLength() - s2.seite.getLength());
        }
        public override string ToString()
        {
            return "Form: " + knubbelPasst.ToString() +
                "\r\nComDistMax: " + comDistMax.ToString("F1") +
                "\r\nComDistMid: " + comDistMid.ToString("F1") +
                "\r\nComDistMin: " + comDistMin.ToString("F1") +
                "\r\nLengthDist: " + lengthDist.ToString("F1") +
                "\r\nAreaDist: " + areaDist.ToString("F1");
                
        }

    }
    public class TeilPassung
    {
        public OrientiertesTeil ot;
        public List<SeitenPassung> seitenPassungen = new List<SeitenPassung>();
        public TeilPassung(OrientiertesTeil ot, List<NeighbourSide> seiten)
        {
            this.ot = ot;
            //NeighbourSides in der Reihenfolge left, bottom, right, top
            List<NeighbourSide> otSides = new List<NeighbourSide>();
            for(int i = 0; i<4; i++)
            {
                if (ot.getSeite(i).getType() == Seite.SeitenTyp.rand)
                {
                    otSides.Add(new NeighbourSide(true, ot.getSeite(i)));
                }
                else
                {
                    otSides.Add(new NeighbourSide(false, ot.getSeite(i)));
                }
            }
            for(int i = 0; i<4; i++)
            {

                seitenPassungen.Add(new SeitenPassung(otSides[i], seiten[i]));


                
                //seitenPassungen.Add(new SeitenPassung(ot.PuzzleTeil.getSeite(i+ot.Orientierung), seiten[i].seite));
            }
        }
        public bool passtSeitenForm()
        {
            for(int i = 0; i<4; i++)
            {
                if (!seitenPassungen[i].knubbelPasst) return false;
            }
            return true;
        }
        public override string ToString()
        {
            return "Left " + seitenPassungen[0].ToString() + "\r\n\r\n" +
                "Bottome " + seitenPassungen[1].ToString() + "\r\n\r\n" +
                "Right " +seitenPassungen[2].ToString() + "\r\n\r\n" +
                "Top " + seitenPassungen[3].ToString();
        }
    }

    public class TeilSelector:StackPanel
    {
        public event EventHandler SelectionChanged;
        EnumCombo<Sortierung.Sortierer> sortCombo = new EnumCombo<Sortierung.Sortierer>("Sortierung");
        Label selectedLabel = new Label();
        //Label descLabel = new Label();
        
        int selectedIndex = -1;

        public TeilSelector()
        {
            this.Children.Add(sortCombo);
            sortCombo.Value = Sortierung.Sortierer.standard;
            sortCombo.ValueChanged += SortCombo_ValueChanged;
            this.Children.Add(selectedLabel);
            //this.Children.Add(descLabel);
            setIndex(-1);
        }

        private void SortCombo_ValueChanged(object? sender, EventArgs e)
        {
            MessageBox.Show("Sortierung geändert");
        }

        public OrientiertesTeil getSelected()
        {
            if(selectedIndex>-1 && selectedIndex < geladeneTeile.Count)
            {
                return geladeneTeile[selectedIndex].ot;
            }
            return null;
        }

        
        public void setIndex(int index)
        {
            selectedIndex = index;
            OrientiertesTeil sel = getSelected();
            if(sel != null)
            {
                selectedLabel.Content = "Teil(" + sel.TeilNummer.ToString() +"," + sel.Orientierung.ToString() + ") " + (index + 1).ToString() + " von " + geladeneTeile.Count; ;
                //descLabel.Content = geladeneTeile[selectedIndex].ToString();
            }
            else
            {
                selectedLabel.Content = "kein Teil";
                //descLabel.Content = "keine passung";
            }
            if (SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
        }

        List<TeilPassung> geladeneTeile= new List<TeilPassung>();

        public void load(List<TeilPassung> pasTeile)
        {
            geladeneTeile.Clear();
            geladeneTeile.AddRange(pasTeile);
            setIndex(0);
        }
        public void load(List<int> alleNochVerfügbareTeilIDs, List<NeighbourSide> neighbourSides)
        {
            geladeneTeile.Clear();
            if (alleNochVerfügbareTeilIDs != null && neighbourSides != null)
            {
                foreach (int teilID in alleNochVerfügbareTeilIDs)
                {
                    for (int orientierung = 0; orientierung < 4; orientierung++)
                    {
                        OrientiertesTeil ot = new OrientiertesTeil(teilID, orientierung);
                        TeilPassung tp = new TeilPassung(ot, neighbourSides);
                        geladeneTeile.Add(tp);
                    }
                }
                geladeneTeile.Sort(Sortierung.getSortierer(sortCombo.Value));//  Sortierung.Sortierer.standard););
            }            
            setIndex(0);
        }
        public void next()
        {
            if (selectedIndex + 1 < geladeneTeile.Count)
            {
                setIndex(selectedIndex + 1);
            }
        }
        public void last()
        {
            if (selectedIndex - 1 >= 0)
            {
                setIndex(selectedIndex - 1);
            }
        }



    }
}
