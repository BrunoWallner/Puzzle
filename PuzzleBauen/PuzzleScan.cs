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


namespace PuzzleBauen
{
    public class PuzzleScan
    {
        public int id = 1;
        public string name = "unbekannt";
        public Mat image;
        public Mat Image
        { get { return image; } }
        public List<PuzzleTeil> teile = new List<PuzzleTeil>();
        public int getMaxArea()
        {
            int ret = 0;
            foreach(PuzzleTeil teil in teile)
            {
                if(teil.getArea()>ret) ret = teil.getArea();
            }
            return ret;
        }
        public int getMinArea()
        {
            int ret = int.MaxValue;
            foreach (PuzzleTeil teil in teile)
            {
                if (teil.getArea() < ret) ret = teil.getArea();
            }
            return ret;
        }
        public PuzzleScan()
        {
        }


        public override string ToString()
        {
            String s = "Teile: " + teile.Count.ToString() + Environment.NewLine
               + "MinSize: " + getMinArea().ToString() + Environment.NewLine
               + "MaxSize: " + getMaxArea().ToString();
            return s;
        }


        public static PuzzleScan load(Stream fs)
        {
            PuzzleScan scan = new PuzzleScan();
            scan.image = Serializer.getMat(fs);
            Int32 count = Serializer.getInt32(fs);
            scan.teile = new List<PuzzleTeil>();
            for(int i = 0; i<count; i++)
            {
                scan.teile.Add(PuzzleTeil.load(fs, scan));
            }
            return scan;
        }
        public void writeToStream(Stream fs)
        {
            Serializer.Add(image, fs);
            Serializer.Add((Int32)teile.Count, fs);
            for (int i = 0; i < teile.Count; i++)
            {
                teile[i].writeToStream(fs);
            }
        }
    }
}
