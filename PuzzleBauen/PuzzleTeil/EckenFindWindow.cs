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

    public class EckenFindWindow : Window
    {
        DockPanel dp = new DockPanel();
        ListBox teileListBox = new ListBox();
        Image image = new Image();
        StackPanel tool = new StackPanel();
        List<PuzzleTeil> teile;
        public EckenFindWindow(List<PuzzleTeil> teile)
        {
            this.Title = "EckenFinden per Hand";
            this.teile = teile;
            Content = dp;
            tool.Orientation = Orientation.Horizontal;
            dp.Children.Add(tool);
            DockPanel.SetDock(tool, Dock.Top);
            dp.Children.Add(teileListBox);
            DockPanel.SetDock(teileListBox, Dock.Left);
            dp.Children.Add(image);
            foreach (PuzzleTeil teil in teile)
            {
                teileListBox.Items.Add(teil);
            }
            teileListBox.SelectionChanged += TeileListBox_SelectionChanged;
            image.MouseDown += Image_MouseDown;
            AutoActionButton setButton = new AutoActionButton("Set the Points", set);
            setButton.FontSize = 20;

            AutoActionButton clearButton = new AutoActionButton("Clear", clear);
            clearButton.FontSize = 20;
            tool.Children.Add(clearButton);
            tool.Children.Add(setButton);
        }
        void set()
        {
            if (selectedTeil == null)
            {
                MessageBox.Show("kein Teil gewählt");
                return;
            }
            if (points.Count != 4)
            {
                MessageBox.Show("passt nicht von der PunkteZahl: " + points.Count.ToString());
                return;
            }
            List<int> pointIndex = new List<int>();
            foreach (OpenCvSharp.Point p in points)
            {
                pointIndex.Add(EckpunkFindAlg.getNearestIndex(p, selectedTeil.getContour()));
            }

            selectedTeil.setEcken(pointIndex.ToArray());
            clear();
        }
        void clear()
        {
            bild = new Mat();
            points.Clear();
            if (selectedTeil != null) bild = selectedTeil.draw();
            image.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(bild);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (image.Source != null)
            {
                Point pos = e.GetPosition(image);
                var x = Math.Floor(pos.X * image.Source.Width / image.ActualWidth);
                var y = Math.Floor(pos.Y * image.Source.Height / image.ActualHeight);
                OpenCvSharp.Point p = new OpenCvSharp.Point(x, y);
                points.Add(p);


                Cv2.Circle(bild, p, 5, Scalar.Blue, 2);
                image.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(bild);

            }
        }
        Mat bild = null;
        PuzzleTeil selectedTeil = null;
        List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();
        private void TeileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTeil = (PuzzleTeil)teileListBox.SelectedItem;
            if (selectedTeil == null) return;
            bild = selectedTeil.draw();
            points.Clear();
            //List<OpenCvSharp.Point> points = FindEck.polyApp(selectedTeil);
            //foreach (OpenCvSharp.Point point in points)
            //{
            //    Cv2.Circle(bild, point, 3, Scalar.Blue);
            //}
            image.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(bild);
        }
    }
}
