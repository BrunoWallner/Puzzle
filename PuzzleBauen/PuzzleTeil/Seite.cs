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
    public class Seite
    {
        public enum SeitenTyp
        {
            unbekannt,
            rand,
            loch,
            knubbel
        }
        public static SeitenTyp reverse(SeitenTyp t)
        {
            if (t == SeitenTyp.loch) return SeitenTyp.knubbel;
            if (t == SeitenTyp.knubbel) return SeitenTyp.loch;
            return t;
        }
        public List<OpenCvSharp.Point> contour = new List<OpenCvSharp.Point>();
        public int index;
        PuzzleTeil parent;
        public Seite(PuzzleTeil teil, int index)
        {
            parent = teil;
            this.index = index;
            if (index < 3)
            {
                for (int i = teil.getEckenIndex(index); i <= teil.getEckenIndex(index + 1); i++)
                {
                    contour.Add(teil.getContour()[i]);
                }
            }
            else
            {
                for (int i = teil.getEckenIndex(index); i < teil.getContour().Length; i++)
                {
                    contour.Add(teil.getContour()[i]);
                }
                for (int i = 0; i <= teil.getEckenIndex(0); i++)
                {
                    contour.Add(teil.getContour()[i]);
                }
            }
        }
        public SeitenTyp getType()
        {
            OpenCvSharp.Point com = getNormalizedCoM(true);
            if (com.Y > 10) return SeitenTyp.knubbel;
            if (com.Y < -10) return SeitenTyp.loch;
            //if (Math.Abs(com.Y) < 5) 
            return SeitenTyp.rand;
            return SeitenTyp.unbekannt;
        }
        Moments moments = null;
        OpenCvSharp.Point getCenterOfMass()
        {
            if (moments == null) moments = Cv2.Moments(contour.ToArray());
            OpenCvSharp.Point centerOfMass = new OpenCvSharp.Point(moments.M10 / moments.M00, moments.M01 / moments.M00);
            return centerOfMass;
        }
        public double getArea(bool oriented)
        {
            return Cv2.ContourArea(contour, oriented);
        }
        public OpenCvSharp.Point getNormalizedCoM(bool fromStartPoint)
        {
            OpenCvSharp.Point transCenter;
            if (fromStartPoint)
            {
                if (contour.Count == 0)
                {
                    MessageBox.Show("kein " + parent.getEckenIndex(0) + "; " + parent.getEckenIndex(1) + "; " + parent.getEckenIndex(2) + "; " + parent.getEckenIndex(3) + "; ");

                }
                transCenter = getCenterOfMass() - contour.First();
                return Transformer.rotate(transCenter, new OpenCvSharp.Point(0, 0), Transformer.getWinkel(contour.First(), contour.Last()));
            }
            transCenter = getCenterOfMass() - contour.Last();
            return Transformer.rotate(transCenter, new OpenCvSharp.Point(0, 0), Transformer.getWinkel(contour.First(), contour.Last()) + 180.0);
        }
        public double getLength()
        {
            return contour.First().DistanceTo(contour.Last());
        }
    }
}
