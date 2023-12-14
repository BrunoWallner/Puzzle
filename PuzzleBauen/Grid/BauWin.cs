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
using winPoint = System.Windows.Point;
using Point = OpenCvSharp.Point;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Collections;
using System.Diagnostics;

namespace PuzzleBauen
{
    public class BauWin : Window
    {
        GridPanel parent;
        DockPanel dp = new DockPanel();
        ToolBar tb = new ToolBar();
        Image image = new Image();
        NamedIntUpDown neighbourDistance = new NamedIntUpDown("Abstand");

        public BauWin(GridPanel parent)
        {
            this.parent = parent;
            this.Title = "BauFenster";
            this.Content = dp;
            dp.Children.Add(tb);
            DockPanel.SetDock(tb, Dock.Top);
            dp.Children.Add(image);
            this.Topmost = true;
            this.Focusable = false;
            //            this.KeyDown += BauWin_KeyDown;
            neighbourDistance.val = PTDraw.NeighbourDrawDistance;
            tb.Items.Add(neighbourDistance);
            neighbourDistance.ValueChanged += NeighbourDistance_ValueChanged;

        }

        private void NeighbourDistance_ValueChanged(object? sender, EventArgs e)
        {
            PTDraw.NeighbourDrawDistance = neighbourDistance.val;
            parent.drawTogetherBild();
        }


        //private void BauWin_KeyDown(object sender, KeyEventArgs e)
        //{
        //    parent.keyDown(sender, e);
        //}

        public void setImage(Mat m)
        {
            if(m!=null) image.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(m);
            else image.Source = null;
        }

    }
}
