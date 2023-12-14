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

using PuzzleBauen;

namespace PuzzleBauen
{
    public class PuTeColWin : Window
    {
        public static PuzzleScan Show(BlackWhite blackWhite)
        {
            PuTeColWin put = new PuTeColWin(blackWhite);

            if (put.ShowDialog() == true)
            {
                return put.psc;
            }
            else return null;

        }
        enum color
        {
            no,
            white,
            green,
            red,
            blue,
            black
        }
        ListBox box = new ListBox();
        DockPanel dp = new DockPanel();
        StackPanel sp = new StackPanel();
        Image image = new Image();

        NamedIntUpDown minSize = new NamedIntUpDown("minArea");
        NamedIntUpDown maxSize = new NamedIntUpDown("maxArea");
        NamedIntUpDown threshold = new NamedIntUpDown("Threshold");
        NamedIntUpDown sizePlus = new NamedIntUpDown("SizePlus");
        NamedIntUpDown erode = new NamedIntUpDown("ErodeCount");
        NamedIntUpDown diletate = new NamedIntUpDown("DiletateCount");
        EnumCombo<color> overlayColor = new EnumCombo<color>("Overlay");
        NamedIntUpDown contourDicke = new NamedIntUpDown("ContourDicke");
        EnumCombo<color> contourColor = new EnumCombo<color>("Contour");
        AutoActionButton neuCalc;
        Label result = new Label();


        BlackWhite blackWhite;
        Mat imageSource;
        PuzzleScan psc = new PuzzleScan();
        public PuTeColWin(BlackWhite blackWhite)
        {
            this.blackWhite = blackWhite;
            this.Title = "Scan-Bearbeitung";

            this.Content = dp;
            dp.Children.Add(sp);
            DockPanel.SetDock(sp, Dock.Left);

            overlayColor.Value = color.green;
            contourColor.Value = color.blue;
            
            minSize.val = 10000;
            maxSize.val = 80000;
            sizePlus.val = 20;
            threshold.val = 180;
            erode.val = 0;
            diletate.val = 0;
            contourDicke.val = 15;

            neuCalc = new AutoActionButton("Neu berechnen", Calc);
            sp.Children.Add(neuCalc);
            sp.Children.Add(minSize);
            sp.Children.Add(maxSize);
            sp.Children.Add(sizePlus);
            sp.Children.Add(threshold);
            sp.Children.Add(erode);
            sp.Children.Add(diletate);
            sp.Children.Add(result);
            sp.Children.Add(overlayColor);
            sp.Children.Add(contourDicke);
            sp.Children.Add(contourColor);                        
            sp.Children.Add(new AutoActionButton("Abbrechen", notOk));
            sp.Children.Add(new AutoActionButton("OK", ok));

            minSize.ValueChanged += ValueChanged;
            maxSize.ValueChanged += ValueChanged;
            sizePlus.ValueChanged += ValueChanged;
            threshold.ValueChanged += ValueChanged;
            erode.ValueChanged += ValueChanged;
            diletate.ValueChanged += ValueChanged;
            overlayColor.ValueChanged += draw;
            contourColor.ValueChanged += draw;
            contourDicke.ValueChanged += draw;
            dp.Children.Add(image);
            image.MouseDown += Image_MouseDown;
            image.MouseUp += Image_MouseUp;

            this.Closing += PuTeColWin_Closing;
            Calc();
        }
        bool userClose = false;
        private void PuTeColWin_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if(this.DialogResult == true)
            {

            }
            else
            {
                if (MessageBox.Show("Wirklich abbrechen?", "Abbrechen?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    if (e != null) e.Cancel = true;
                    return;
                }
            }
            //if(userClose) return;
            //if (MessageBox.Show("Wirklich abbrechen?", "Abbrechen?", MessageBoxButton.YesNo) == MessageBoxResult.No)
            //{
            //    if(e!=null) e.Cancel = true;
            //    return;
            //}
            //try
            //{
            //    this.DialogResult = false;
            //}
            //catch (Exception ex) { }
        }

        void ok()
        {
            if(MessageBox.Show("Wirklich in Ordnding?", "OK?",MessageBoxButton.YesNo) == MessageBoxResult.No) return;            
            //try
            //{
                this.DialogResult = true;
            //}
            //catch (Exception ex) { }
            //userClose = true;
            //MessageBox.Show("userclosetrue");
           // this.Close();            
        }
        void notOk()
        {
            this.DialogResult = false;
            //PuTeColWin_Closing(null, null);
            //if (MessageBox.Show("Wirklich abbrechen?", "Abbrechen?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            //try
            //{
            //    this.DialogResult = false;
            //}
            //catch (Exception ex) { }
            //this.Close();
        }
      

        OpenCvSharp.Point sizeStart;
        bool size = false;
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!size) return;
            if (image.Source != null)
            {
                try
                {
                    Point pos = e.GetPosition(image);
                    var x = Math.Floor(pos.X * image.Source.Width / image.ActualWidth);
                    var y = Math.Floor(pos.Y * image.Source.Height / image.ActualHeight);
                    size = false;
                    OpenCvSharp.Size sized = new OpenCvSharp.Size(x - sizeStart.X, y - sizeStart.Y);
                    OpenCvSharp.Rect rect = new OpenCvSharp.Rect(sizeStart, sized);
                    Mat sub = imageSource[rect];
                    Image im = new Image();
                    im.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(sub);
                    Window w = new Window();
                    w.Content = im;
                    w.ShowDialog();
                }
                catch (Exception ex) { }

            }
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (image.Source != null)
            {
                Point pos = e.GetPosition(image);
                var x = Math.Floor(pos.X * image.Source.Width / image.ActualWidth);
                var y = Math.Floor(pos.Y * image.Source.Height / image.ActualHeight);                
                sizeStart = new OpenCvSharp.Point(x, y);
                size = true;
            }
        }

        void draw(object? sender, EventArgs e)
        {
            imageSource = new Mat();
            blackWhite.whiteImage.CopyTo(imageSource);
            if (overlayColor.Value != color.no)
            {
                Scalar scal = fromColor(overlayColor.Value);
                foreach (PuzzleTeil teil in psc.teile)
                {
                    Mat realgreen = new Mat(new OpenCvSharp.Size(teil.boundingRect.Width, teil.boundingRect.Height), MatType.CV_8UC3, scal);
                    realgreen.CopyTo(imageSource[teil.boundingRect], teil.mask);
                }
            }
            if (contourColor.Value != color.no)
            {
                Scalar scal = fromColor(contourColor.Value);
                foreach (PuzzleTeil teil in psc.teile)
                {
                    int thick = contourDicke.val;
                    if (thick < 1) return;
                    OpenCvSharp.Point trans = new OpenCvSharp.Point(teil.boundingRect.X, teil.boundingRect.Y);
                    OpenCvSharp.Point[][] conturs;
                    HierarchyIndex[] hierarchyIndices;
                    Cv2.FindContours(teil.mask, out conturs, out hierarchyIndices, RetrievalModes.CComp, ContourApproximationModes.ApproxNone);
                    for (int i = 0; i < conturs[0].Length - 1; i++)
                    {
                        Cv2.Line(imageSource, conturs[0][i] + trans, conturs[0][i + 1] + trans, scal, thick);
                    }
                    Cv2.Line(imageSource, conturs[0][conturs[0].Length - 1] + trans, conturs[0][0] + trans, scal, thick);

                }
            }
            image.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(imageSource);
        }
        Scalar fromColor(color c)
        {
            Scalar scal = Scalar.Black;
            switch(c)
            {
                case color.white:
                    scal = Scalar.White;
                    break;
                case color.blue:
                    scal = Scalar.Blue;
                    break;
                case color.green:
                    scal = Scalar.Green;
                    break;
                case color.red:
                    scal = Scalar.Red;
                    break;
                case color.black:
                    scal = Scalar.Black;
                    break;
            }
            return scal;
        }


        private void ValueChanged(object? sender, EventArgs e)
        {
            neuCalc.IsEnabled = true;
        }

        public void Calc()
        {
                    
            result.Content = "Nix";
            Mat thresholdImage = new Mat();
            Cv2.Threshold(blackWhite.diffSource(), thresholdImage, threshold.val, 255, ThresholdTypes.BinaryInv);
            

            Mat strukturElement = new Mat();
            Cv2.Erode(thresholdImage, thresholdImage, strukturElement, null, erode.val);

            ConnectedComponents cc = Cv2.ConnectedComponentsEx(thresholdImage, PixelConnectivity.Connectivity8);

            psc = new PuzzleScan();
            psc.image = new Mat();
            blackWhite.whiteImage.CopyTo(psc.image);
            Mat realWhite = new Mat(blackWhite.whiteImage.Size(), MatType.CV_8UC1, Scalar.White);
            foreach (ConnectedComponents.Blob b in cc.Blobs)
            {
                if (b.Area > minSize.val && b.Area < maxSize.val)
                {
                    OpenCvSharp.Rect boundingRect = new OpenCvSharp.Rect(b.Rect.Left - sizePlus.val, b.Rect.Top - sizePlus.val, b.Width + 2 * sizePlus.val, b.Height + 2 * sizePlus.val);
                    if (boundingRect.X < 0)
                    {                        
                        boundingRect.Width += boundingRect.X;
                        boundingRect.X = 0;
                    }
                    if (boundingRect.Y < 0)
                    {
                        boundingRect.Height += boundingRect.Y;
                        boundingRect.Y = 0;
                    }
                    if (boundingRect.Right > blackWhite.whiteImage.Width) boundingRect.Width = blackWhite.whiteImage.Width - boundingRect.Left;
                    if (boundingRect.Bottom > blackWhite.whiteImage.Height) boundingRect.Height = blackWhite.whiteImage.Height - boundingRect.Top;
                    Mat mask = new Mat();
                    cc.FilterByBlob(realWhite, mask, b);
                    mask[boundingRect].CopyTo(mask);

                    Cv2.Dilate(mask, mask, strukturElement, null, diletate.val);

                    PuzzleTeil neuesTeil = new PuzzleTeil(psc, mask, boundingRect);
                    //neuesTeil.scan = psc;
                    //neuesTeil.mask = mask;
                    //neuesTeil.targetRect = boundingRect;
                    psc.teile.Add(neuesTeil);
                }
            }
            result.Content = psc.ToString();
            neuCalc.IsEnabled = false;
            draw(null, null);
        }
    }
}
