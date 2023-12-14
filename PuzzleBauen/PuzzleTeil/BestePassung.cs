using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using OpenCvSharp;
using MessageBox = System.Windows.MessageBox;
using Window = System.Windows.Window;
using Point = System.Windows.Point;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Collections;
#pragma warning disable

namespace PuzzleBauen
{

    public class BestePassung
    {
        public static int lengthDist = 8;
        public static int ComDist = 15;
        public static bool checkSeiten = true;

        static double getLenghDistance(Seite ns, Seite seite)
        {
            return 0;
        }
        static double getMaxComDistance(Seite ns, Seite seite)
        { return 0; }
        static double getMiddleComDistance(Seite ns, Seite seite)
        { return 0; }
        static double getMinComDistance(Seite ns, Seite seite)
        { return 0; }
        static double getAreaDistance(Seite ns, Seite seite)
        {
            return 0;

        }



        static bool extraCheck(Seite ns, Seite seite)
        {
            if (ns.getType() == Seite.reverse(seite.getType()))
            {
                if (ns.getNormalizedCoM(true).DistanceTo(seite.getNormalizedCoM(false)) < ComDist)
                {
                    if (Math.Abs(ns.getLength() - seite.getLength()) < lengthDist) return true;
                }
                return false;
            }
            return false;
        }
        static bool istEsPosible(NeighbourSide neighbour, Seite seite)
        {
            if (!checkSeiten) return true;
            if (neighbour.isRand)
            {
                if (seite.getType() == Seite.SeitenTyp.rand) return true;
                return false;
            }
            else
            {
                if (neighbour.seite == null) return true;
                return extraCheck(neighbour.seite, seite);
            }
        }         
        static bool getPosibles2(List<NeighbourSide> neighbours, OrientiertesTeil teil)
        {
            bool add = true;
            for (int i = 0; i < 4; i++)
            {
                if (!istEsPosible(neighbours[i], teil.PuzzleTeil.getSeite(teil.Orientierung + i))) return false;
            }
            return true;
        }
        public static List<OrientiertesTeil> getPosibles2(List<NeighbourSide> neighbours, List<OrientiertesTeil> teile)
        {
            List<OrientiertesTeil> ret = new List<OrientiertesTeil>();
            foreach (OrientiertesTeil teil in teile)
            {
                if (getPosibles2(neighbours, teil)) ret.Add(teil);
            }
            return ret;
        }

    }
}
