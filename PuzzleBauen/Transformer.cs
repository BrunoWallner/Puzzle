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

namespace PuzzleBauen
{
    public static class Transformer
    {
        public static OpenCvSharp.Point rotate(OpenCvSharp.Point p, OpenCvSharp.Point center, double gradAngle)
        {
            double angle = gradAngle * Math.PI / 180.0;
            double xNeu = (p.X - center.X) * Math.Cos(angle) - (p.Y - center.Y) * Math.Sin(angle) + center.X;
            double yNeu = (p.X - center.X) * Math.Sin(angle) + (p.Y - center.Y) * Math.Cos(angle) + center.Y;
            return new OpenCvSharp.Point(xNeu, yNeu);
        }
        public static double getWinkel(OpenCvSharp.Point first, OpenCvSharp.Point second)
        {
            Vector linVec = new Vector(first.X - second.X, second.Y - first.Y);
            double wink = Math.Atan2(linVec.Y, linVec.X) * 180.0 / Math.PI;
            wink = wink - 180;
            if (wink <= -180) wink += 360;
            return wink;
        }

        public static Mat getTransmat(float tx, float ty)
        {
            List<float> transVal = new List<float>();
            transVal.Add(1);
            transVal.Add(0);
            transVal.Add(tx);
            transVal.Add(0);
            transVal.Add(1);
            transVal.Add(ty);
            return new Mat(2, 3, MatType.CV_32F, transVal.ToArray());
        }
    }
}
