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
using OpenCvSharp;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Xceed.Wpf.Toolkit;


#pragma warning disable
namespace PuzzleBauen
{
    public class EckpunkFindAlg
    {
        public static int getNearestIndex(OpenCvSharp.Point point, OpenCvSharp.Point[] contour)
        {
            int ret = -1;
            double minDist = double.MaxValue;
            for (int i = 0; i < contour.Length; i++)
            {
                if (point.DistanceTo(contour[i]) < minDist)
                {
                    minDist = point.DistanceTo(contour[i]);
                    ret = i;
                }
            }
            return ret;
        }
        static int getMostAway(OpenCvSharp.Point point, OpenCvSharp.Point relTo, double maxDist, OpenCvSharp.Point[] contour)
        {
            int ret = -1;
            double dist = 0;
            for (int i = 0; i < contour.Length; i++)
            {
                if (point.DistanceTo(contour[i]) < maxDist)
                {
                    if (relTo.DistanceTo(contour[i]) > dist)
                    {
                        dist = relTo.DistanceTo(contour[i]);
                        ret = i;
                    }
                }

            }
            return ret;
        }
        public static int[] getEckenIndizes(OpenCvSharp.Point[] contour)
        {
            double minLochDistance = 75;
            double epsilon = 18;
            double noEdgeWinkelMax = 60;
            double noEdgeWinkelMin = 120;
            double winkel1 = 220;
            double winkel2 = 240;
            double winkel3 = 130;

            OpenCvSharp.Point[] polygon = Cv2.ApproxPolyDP(contour, epsilon, true);

            Moments m = Cv2.Moments(contour);
            OpenCvSharp.Point centerOfMass = new OpenCvSharp.Point(m.M10 / m.M00, m.M01 / m.M00);


            ConturList cl = new ConturList(polygon.ToList());
            xPoint knubbelStart = null;
            List<xPoint> pointsToKill = new List<xPoint>();
            foreach (xPoint x in cl.points)
            {
                if (x.point.DistanceTo(centerOfMass) < minLochDistance)
                {
                    pointsToKill.Add(x);
                    x.previos.type = xPointType.loch;
                    xPoint startp = x;
                    while (startp.next.getInnenwinkel() > noEdgeWinkelMin || startp.next.getInnenwinkel() < noEdgeWinkelMax)
                    {
                        startp = startp.next;
                        pointsToKill.Add(startp);
                        startp.previos.type = xPointType.loch;
                    }
                    startp = x;
                    while (startp.previos.getInnenwinkel() > noEdgeWinkelMin || startp.previos.getInnenwinkel() < noEdgeWinkelMax)
                    {
                        startp = startp.previos;
                        pointsToKill.Add(startp);
                        startp.previos.type = xPointType.loch;
                    }
                    knubbelStart = startp.previos;
                }
            }
            //if (pointsToKill.Count == 0) return null;
            foreach (xPoint x in pointsToKill)
            {
                x.kill();
            }
            pointsToKill = new List<xPoint>();
            if (knubbelStart == null)
            {
                return null;
                //knubbelStart = cl.points[0].next.next.next;
            }
            xPoint knubbelRun = knubbelStart.next;
            int count1 = 0;
            while (knubbelRun != knubbelStart)
            {
                count1++;
                if (knubbelRun.getInnenwinkel() > winkel1)
                {
                    bool ret = true;
                    int count2 = 0;
                    while (ret)
                    {
                        count2++;
                        pointsToKill.Add(knubbelRun);
                        knubbelRun.previos.type = xPointType.knubbel;
                        knubbelRun.next.type = xPointType.knubbel;
                        knubbelRun = knubbelRun.next;
                        if (knubbelRun.getInnenwinkel() > winkel2)
                        {
                            ret = false;
                            pointsToKill.Add(knubbelRun);
                            knubbelRun.previos.type = xPointType.knubbel;
                            knubbelRun.next.type = xPointType.knubbel;
                        }
                        if (count2 > 2000) return null;
                    }
                }
                knubbelRun = knubbelRun.next;
                if (count1 > 2000) return null;
            }
            foreach (xPoint x in pointsToKill)
            {
                x.kill();
            }
            pointsToKill = new List<xPoint>();
            foreach (xPoint x in cl.points)
            {
                if (x.getInnenwinkel() > winkel3) pointsToKill.Add(x);
                x.previos.type = x.type;

            }
            foreach (xPoint x in pointsToKill)
            {
                x.kill();
            }
            if (cl.points.Count == 4)
            {
                int[] ret = new int[4];

                List<int> r = new List<int>();
                for (int i = 0; i < 4; i++)
                {

                    r.Add(getMostAway(cl.points[i].point, centerOfMass, 15, contour));
                    //r.Add(getBestIndex(cl.points[i].point, contour));
                }
                r.Sort();
                return r.ToArray();
            }
            return null;
        }





    }
    public enum xPointType
    {
        unbekannt,
        loch,
        knubbel,
        kannte
    }
    public class xPoint
    {
        //public bool toKill = false;
        public ConturList list;
        public xPoint previos;
        public xPoint next;
        public xPointType type = xPointType.unbekannt;
        public OpenCvSharp.Point point;
        public double getInnenwinkel()
        {
            double winkel = previos.getWinkel() - getWinkel();
            winkel = 180 + winkel;
            if (winkel > 360) winkel -= 360;
            if (winkel < 0) winkel += 360;
            return winkel;
        }
        public double getWinkel()
        {
            Vector linVec = new Vector(point.X - next.point.X, next.point.Y - point.Y);
            double wink = Math.Atan2(linVec.Y, linVec.X) * 180.0 / Math.PI;
            wink = wink - 180;
            if (wink <= -180) wink += 360;
            return wink;
        }
        public void kill()
        {
            if (next == null && previos == null) return;
            previos.next = next;
            next.previos = previos;
            previos = null;
            next = null;
            list.points.Remove(this);
        }
        public xPoint(ConturList list)
        {
            this.list = list;
        }


    }
    public class ConturList//:IEnumerator
    {
        public List<xPoint> points = new List<xPoint>();
        public ConturList(List<OpenCvSharp.Point> contur)
        {
            xPoint last = null;
            for (int i = 0; i < contur.Count; i++)
            {
                xPoint p = new xPoint(this);
                p.point = contur[i];
                p.previos = last;
                if (last != null) last.next = p;
                points.Add(p);
                last = p;
            }
            points[0].previos = last;
            last.next = points[0];
        }
    }





}
