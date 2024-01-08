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

using System.Xml.Serialization;
using Microsoft.Win32;
using System.Collections;
using OpenCvSharp.Dnn;
using System.Windows.Media.Media3D;
using Point = OpenCvSharp.Point;
using PuzzleBauen;
using Grid = PuzzleBauen.Grid;

namespace PuzzleBauenClient
{

    public class Auftrag
    {
        OrientiertesTeil teil;
        int x;
        int y;
        public Grid grid;
        DateTime erstellt;
        public int X
        {
            get { return x; }
        }
        public int Y
        {
            get { return y; }
        }
        public OrientiertesTeil Teil
        {
            get { return teil; }
        }


        public Auftrag(int x, int y, OrientiertesTeil teil, Grid grid)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            this.teil = teil;
            erstellt = DateTime.Now;
        }
        public Mat getScanBild()
        {
            PuzzleScanCollection col = PuzzleScanCollection.getInstance();
            PuzzleTeil pt = col.getTeil(teil.TeilNummer);
            Mat ret = new Mat(pt.scan.Image.Rows, pt.scan.Image.Cols, MatType.CV_8UC3, Scalar.White);

            foreach (int i in TeilManager.getInstance().getVerfügbareTeile())
            {
                PuzzleTeil otherTeil = col.getTeil(i);
                if (otherTeil.scan == pt.scan)
                {
                    col.getTeil(i).getImage().CopyTo(ret[col.getTeil(i).boundingRect], col.getTeil(i).mask);
                }
            }
            Point[] cont = pt.getContour();
            Point offset = pt.boundingRect.TopLeft;
            for (int i = 0; i < cont.Length - 1; i++)
            {
                Cv2.Line(ret, cont[i] + offset, cont[i + 1] + offset, Scalar.Red, 15);
            }

            int width = ret.Width;
            int height = ret.Height;
            Mat ret_small = new Mat();
            Cv2.Resize(ret, ret_small, new OpenCvSharp.Size(width / 4, height / 4));

            return ret_small;
        }
        public Mat getPuzzleBild()
        {
            Mat ret = grid.gd.getBild(true);
            grid.gd.markiereZelle(ret, x, y, null, Scalar.Red);
            return ret;
        }
        public override string ToString()
        {
            return "X: " + x.ToString() + "; Y: " + y.ToString() + teil.ToString();
        }
    }
}
