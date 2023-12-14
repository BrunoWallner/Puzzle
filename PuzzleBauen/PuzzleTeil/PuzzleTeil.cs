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
    public class PuzzleTeil
    {
        public Mat mask;
        public OpenCvSharp.Rect boundingRect;
        public PuzzleScan scan;
        int[] Ecken = { -1, -1, -1, -1 };
        public int getEckenIndex(int ecke)
        {
            return Ecken[ecke % 4];
        }
        /// <summary>
        /// Gibt true zurück, wenn alle Ecken gesetzt sind
        /// </summary>
        /// <returns></returns>
        public bool isEckenOK()
        {
            for (int i = 0; i < 4; i++)
            {
                if (Ecken[i] < 0) return false;
            }
            return true;
        }
        /// <summary>
        /// Die Ecken sind im Gegenuhrzeigersinn durchnummeriert (von 0 bis 3). 
        /// Die Ecke 0 ist zufällig (zumindest weiß ich nicht, nach welchen Kriterien das entschieden wird)
        /// Die Funktion gibt die Koordinaten der entsprechenden Ecke zurück. Der Koordinatensystemursprung
        /// ist dabei die mask (glaube ich). Jede Ecke ist Element von getContour()
        /// </summary>
        /// <param name="ecke"></param>
        /// <returns></returns>
        public OpenCvSharp.Point getEcke(int ecke)
        {
            return getContour()[Ecken[ecke % 4]];
        }
        public void setEcken(int[] ecken)
        {
            if (ecken == null) return;
            if (ecken.Length != 4) return;
            List<int> sorted = new List<int>();
            sorted.AddRange(ecken);
            sorted.Sort();
            Ecken = sorted.ToArray();
        }
        public PuzzleTeil(PuzzleScan scan, Mat mask, OpenCvSharp.Rect boundingRect)
        {
            this.scan = scan;
            this.mask = mask;
            this.boundingRect = boundingRect;
        }

        Seite[] seiten = null;
        /// <summary>
        /// Die Seiten sind durchnummeriert. Seite 0 ist die Seite von Ecke 0 zu Ecke 1, ...
        /// (glaube ich). Jede Seite ist eine Teilmenge von getContour()
        /// </summary>
        /// <param name="Dindex"></param>
        /// <returns></returns>
        public Seite getSeite(int Dindex)
        {
            int index = Dindex % 4;
            if (seiten == null)
            {
                if (!isEckenOK()) return null;
                seiten = new Seite[4];
                for (int i = 0; i < 4; i++)
                {
                    seiten[i] = new Seite(this, i);
                }
            }
            return seiten[index];
        }
        public List<int> hasSeiten(List<Seite.SeitenTyp> reqS)
        {
            List<int> ret = new List<int>();
            for (int teilIndex = 0; teilIndex < 4; teilIndex++)
            {
                bool add = true;
                for (int i = 0; i < 4; i++)
                {
                    if (reqS[i] != Seite.SeitenTyp.unbekannt && reqS[i] != getSeite(teilIndex + i).getType())
                    {
                        add = false;
                    }
                }
                if (add) ret.Add(teilIndex);
            }
            return ret;
        }
        OpenCvSharp.Point[] contour = null;
        /// <summary>
        /// Die Contour ist eine Polygonaproximation?? aller Punkte, die das Teil umschließen.
        /// </summary>
        /// <returns></returns>
        public OpenCvSharp.Point[] getContour()
        {
            if (contour == null)
            {
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchyIndices;
                Cv2.FindContours(mask, out contours, out hierarchyIndices, RetrievalModes.CComp, ContourApproximationModes.ApproxNone);
                contour = contours[0];
            }
            return contour;
            //return contours[0];
        }
        public int getArea()
        {
            OpenCvSharp.Point[][] conturs;
            HierarchyIndex[] hierarchyIndices;
            Cv2.FindContours(mask, out conturs, out hierarchyIndices, RetrievalModes.CComp, ContourApproximationModes.ApproxNone);
            return (int)Cv2.ContourArea(conturs[0]);
        }
        public OpenCvSharp.Point getCoM()
        {
            OpenCvSharp.Point[][] conturs;
            HierarchyIndex[] hierarchyIndices;
            Cv2.FindContours(mask, out conturs, out hierarchyIndices, RetrievalModes.CComp, ContourApproximationModes.ApproxNone);
            Moments m = Cv2.Moments(conturs[0]);
            OpenCvSharp.Point centerOfMass = new OpenCvSharp.Point(m.M10 / m.M00, m.M01 / m.M00);
            return centerOfMass;
        }
        public Mat getImage()
        {
            Mat ret = new Mat();
            scan.image[boundingRect].CopyTo(ret, mask);
            if (Ecken != null)
            {
                Cv2.Circle(ret, getContour()[Ecken[0]], 5, Scalar.White, 3);
                Cv2.Circle(ret, getContour()[Ecken[1]], 5, Scalar.White, 3);
                Cv2.Circle(ret, getContour()[Ecken[2]], 5, Scalar.White, 3);
                Cv2.Circle(ret, getContour()[Ecken[3]], 5, Scalar.White, 3);
            }

            return ret;
        }
        public Mat draw()
        {
            Mat ret = new Mat();
            scan.image[boundingRect].CopyTo(ret, mask);
            if (Ecken == null)
            {
                Ecken = EckpunkFindAlg.getEckenIndizes(getContour());
            }
            if (isEckenOK())
            {
                for (int i = 0; i < 4; i++)
                {
                    Cv2.Circle(ret, getContour()[Ecken[i]], 5, Scalar.Blue, 1);
                }
            }
            return ret;
        }
        public Mat drawTo(OpenCvSharp.Size rec)
        {
            Mat ret = new Mat();
            scan.image[boundingRect].CopyTo(ret, mask);
            Cv2.Resize(ret, ret, rec);

            return ret;
        }


        public static PuzzleTeil load(Stream fs, PuzzleScan scan)
        {
            Mat mask = Serializer.getMat(fs);
            var boundingRect = Serializer.getRect(fs);
            PuzzleTeil ret = new PuzzleTeil(scan, mask, boundingRect);
            for (int i = 0; i < 4; i++)
            {
                ret.Ecken[i] = Serializer.getInt32(fs);// .dd((Int32)Ecken[i], fs);
            }
            return ret;
        }
        public void writeToStream(Stream fs)
        {
            Serializer.Add(mask, fs);
            Serializer.Add(boundingRect, fs);
            for (int i = 0; i < 4; i++)
            {
                Serializer.Add(Ecken[i], fs);
            }
        }
    }
}
