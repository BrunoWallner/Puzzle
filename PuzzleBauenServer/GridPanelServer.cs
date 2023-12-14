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

using OpenCvSharp.Dnn;
using PuzzleBauen;
using Grid = PuzzleBauen.Grid;
using System.Data;
using Rect = OpenCvSharp.Rect;
using System.Runtime.CompilerServices;

namespace PuzzleBauenServer
{
    public class GridPanelServer : DockPanel
    {
        public event EventHandler<Grid> NeuerAuftrag;
        void onNeuerAuftrag(Rect selection)
        {
            if (NeuerAuftrag != null) NeuerAuftrag(this, grid);
        }


        Grid grid;
 
        //Point selectedCell = new Point(0, 0);
        //Image togetherImage = new Image();
        Image gridImage = new Image();
        StackPanel sp = new StackPanel();
        CheckBox showAll;
        DockPanel dp;// = new DockPanel();
        

      
        public Grid Grid
        {
            get { return grid; }
        }


        ServerWin serv;
        public GridPanelServer(ServerWin server, Grid grid)
        {
            this.serv = server;

            this.grid = grid;
            DockPanel dp = this;

            //this.Title = "GridWinServer";              
            //Content = dp;            
            sp.Orientation = Orientation.Horizontal;
            dp.Children.Add(sp);
            DockPanel.SetDock(sp, Dock.Top);
            dp.Children.Add(gridImage);

            //  sp.Children.Add(new AutoActionButton("Teilstück beauftragen", export));
            //sp.Children.Add(new AutoActionButton("load", load));

            showAll = new CheckBox();
            showAll.Content = "Bild";
            showAll.IsChecked = true;
            showAll.Checked += ShowAll_Checked;
            showAll.Unchecked += ShowAll_Unchecked;
            sp.Children.Add(showAll);





            gridImage.MouseDown += GridImage_MouseDown;
            gridImage.MouseMove += GridImage_MouseMove;
            gridImage.MouseUp += GridImage_MouseUp;

            draw();
       


            this.Focus();

            grid.BelegungChanged += Grid_Redraw;
            //grid.BildChanged += Grid_Redraw;
            this.Visibility = Visibility.Visible;


         
        }

        private void ShowAll_Unchecked(object sender, RoutedEventArgs e)
        {
            draw();
        }

        private void ShowAll_Checked(object sender, RoutedEventArgs e)
        {
            draw();
        }




        Point? selectionStart = null;
        Point? selectionEnd = null;
        public Rect? selection = null;
 
        Rect getRect(Point p1, Point p2)
        {
            int xl = (p1.X < p2.X) ? p1.X : p2.X;
            int xr = (p1.X > p2.X) ? p1.X : p2.X;
            int yo = (p1.Y < p2.Y) ? p1.Y : p2.Y;
            int yu = (p1.Y > p2.Y) ? p1.Y : p2.Y;

            return new Rect(xl, yo, xr - xl+1, yu - yo+1);
        }
        private void GridImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gridImage.Source != null && e.LeftButton == MouseButtonState.Pressed)
            {                
                winPoint pos = e.GetPosition(gridImage);
                int x = (int)Math.Floor(pos.X * gridImage.Source.Width / gridImage.ActualWidth);
                int y = (int)Math.Floor(pos.Y * gridImage.Source.Height / gridImage.ActualHeight);
                Point selectedCell = grid.gd.getCell(new OpenCvSharp.Point(x, y));
                if (grid.isZelleInGrid(selectedCell))
                {
                    selectionStart = selectedCell;
                    selectionEnd = selectedCell;
                }
                else
                {
                    selectionStart = null;
                    selectionEnd = null;
                }
            }
        }
        private void GridImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (gridImage.Source != null && e.LeftButton == MouseButtonState.Pressed)
            {
                winPoint pos = e.GetPosition(gridImage);
                int x = (int)Math.Floor(pos.X * gridImage.Source.Width / gridImage.ActualWidth);
                int y = (int)Math.Floor(pos.Y * gridImage.Source.Height / gridImage.ActualHeight);
                Point selectedCell = grid.gd.getCell(new OpenCvSharp.Point(x, y));
                if (grid.isZelleInGrid(selectedCell))
                {
                    selectionEnd = selectedCell;
                }
                selection = getRect(selectionStart.Value, selectionEnd.Value);
                draw();
            }


        }
        private void GridImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridImage.Source != null && e.LeftButton == MouseButtonState.Pressed)
            {
                MessageBox.Show("Selected " + selection.ToString());
            }
        }

        private void Grid_Redraw(object? sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() => { draw(); });
        }


        void draw()
        {
            Mat bild = grid.gd.getBild(showAll.IsChecked == true);
            if(selection!=null)
            {
                for(int x = selection.Value.Left; x < selection.Value.Right; x++)
                {
                    for(int y = selection.Value.Top; y<selection.Value.Bottom; y++)
                    {
                        grid.gd.markiereZelle(bild, x, y, Scalar.Red, null);
                    }
                }
            }
            gridImage.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(bild);
        }

    }
}
