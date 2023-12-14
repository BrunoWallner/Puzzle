using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Point = OpenCvSharp.Point;
using System.Runtime.CompilerServices;
using Window = System.Windows.Window;
using Xceed.Wpf.AvalonDock.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace PuzzleBauen
{
    //public class test : Window
    //{
    //    GridPanel gp;
    //    DockPanel dp = new DockPanel();
    //    public test()
    //    {
    //        this.Title = "test";
            
    //        this.Content = dp;
    //        StackPanel sp = new StackPanel();
    //        sp.Children.Add(new AutoActionButton("save", save));
    //        sp.Children.Add(new AutoActionButton("load", load));
    //        dp.Children.Add(sp);
    //        DockPanel.SetDock(sp, Dock.Left);

    //        gp = new GridPanel(null);// Grid.getInstance());
    //        gp.InsertRequestEvent += Gp_InsertRequestEvent;
    //        dp.Children.Add(gp);

    //    }

    //    void save()
    //    {
    //        FileStream fs = new FileStream("c:\\Users\\heigl\\Desktop\\paris2\\x.x", FileMode.Create);
    //        gp.Grid.writeToStream(fs);
    //        fs.Close();
    //    }
    //    void load()
    //    {
    //        FileStream fs = new FileStream("c:\\Users\\heigl\\Desktop\\paris2\\x.x", FileMode.Open);
    //        gp.InsertRequestEvent -= Gp_InsertRequestEvent;
    //        dp.Children.Remove(gp);
    //        throw new Exception();
    //        //gp = new GridPanel(Grid.loadFromStream(fs));
    //        gp.InsertRequestEvent += Gp_InsertRequestEvent;
    //        dp.Children.Add(gp);
    //        fs.Close();

    //    }
    //    private void Gp_InsertRequestEvent(int x, int y, OrientiertesTeil ot)
    //    {
    //        gp.Grid.insert(x, y, ot);
    //    }
    //}

    public class GridPanel:DockPanel
    {
        public delegate void InsertRequestDelegate(int x, int y, OrientiertesTeil ot);
        public delegate void GridImageDrawDelegate(Mat m);
        public event InsertRequestDelegate InsertRequestEvent;
        public event GridImageDrawDelegate GridImageDrawEvent;
        

        Grid grid = null;
        Point? selectedCell = null;
        Point? SelectedCell
        {
            get { return selectedCell; }
            set
            {
                if (value != null)
                {
                    GridBelegung bel = grid.getBelegung(value.Value.X, value.Value.Y);
                    if (bel != null && !bel.isRand)
                    {
                        selectedCell = value;
                    }
                    else
                    {
                        //selectedCell = null;
                    }
                }
                //else selectedCell = null;                 
                fillTeilSelector();
                drawGridImage();
                drawTogetherBild();
            }
        }
        void fillTeilSelector()
        {
            if(SelectedCell!=null)
            {
                List<NeighbourSide> neighbourSides = grid.getNeighbourSides(SelectedCell.Value.X, SelectedCell.Value.Y);
                teilSelector.load(TeilManager.getInstance().getVerfügbareTeile(), neighbourSides);
            }
            else
            {
                teilSelector.load(null, null);
            }
        }

        Image gridImage = new Image();        
        TeilSelector teilSelector = new TeilSelector();
        StackPanel sp = new StackPanel();
        BauWin bauFenster = null;
        Image togetherImage = new Image();
        CheckBox showAll;
        //NamedIntUpDown neighbourDistance = new NamedIntUpDown("Abstand");
        public GridPanel(Grid grid)
        {

            this.Children.Add(sp);
            DockPanel.SetDock(sp, Dock.Left);
            this.Children.Add(gridImage);
            //AutoActionButton baub = new AutoActionButton("Show Bauwin", bau);
            //baub.Focusable = false;
            
            sp.Children.Add(teilSelector);
            sp.Children.Add(togetherImage);
            //sp.Children.Add(neighbourDistance);
            //neighbourDistance.val = PTDraw.NeighbourDrawDistance;
            //neighbourDistance.Focusable = false;
            //neighbourDistance.IsEnabled = false;
            //neighbourDistance.ValueChanged += NeighbourDistance_ValueChanged;
            sp.Children.Add(new AutoActionButton("Show Bauwin", zeigeBauFenster));
            togetherImage.MaxWidth = 400;
            teilSelector.SelectionChanged += TeilSelector_SelectionChanged;
            gridImage.MouseDown += GridImage_MouseDown;

            showAll = new CheckBox();
            showAll.Content = "Bild";
            showAll.IsChecked = true;
            showAll.Checked += ShowAll_Checked;
            showAll.Unchecked += ShowAll_Unchecked;
            sp.Children.Add(showAll);
            //this.KeyDown += keyDown;
            setGrid(grid);
            SelectedCell = new Point(1, 1);





        }
        private void ShowAll_Checked(object sender, RoutedEventArgs e)
        {
            if (grid != null)
            {
                drawGridImage();
                return;
            }
           // drawGridImage();
            //throw new NotImplementedException();
        }
        private void ShowAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (grid != null)
            {
                drawGridImage();
                return;
            }
            //throw new NotImplementedException();
        }
        //private void NeighbourDistance_ValueChanged(object? sender, EventArgs e)
        //{
        //    PTDraw.NeighbourDrawDistance = neighbourDistance.val;

        //        drawTogetherBild();
        //}

        //private void GridPanel_KeyDown(object sender, KeyEventArgs e)
        //{
        //    keyDown()
        //}

        public void setGrid(Grid neuesGrid)
        {
            if(this.grid != null)
            {             
                grid.BelegungChanged -= Grid_BelegungChanged;
            }            
            this.grid = neuesGrid;
            if (neuesGrid == null)
            {
                drawGridImage();
                return;
            }
            grid.BelegungChanged += Grid_BelegungChanged;
            drawGridImage();
        }

        private void Grid_BelegungChanged(object? sender, EventArgs e)
        {
            drawGridImage();
        }

        private void Grid_BildChanged(object? sender, EventArgs e)
        {
            drawGridImage();
        }

        private void TeilSelector_SelectionChanged(object? sender, EventArgs e)
        {
            drawTogetherBild();
        }

        void zeigeBauFenster()
        {
            if (bauFenster == null)
            {
                bauFenster = new BauWin(this);
                bauFenster.Closing += BauFenster_Closing;
                drawTogetherBild();
                bauFenster.Show();
                // bauFenster.SizeToContent = SizeToContent.WidthAndHeight;
                bauFenster.Width = 400;
                bauFenster.Height = 400;
            }
        }
        private void BauFenster_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            bauFenster.Closing -= BauFenster_Closing;
            bauFenster = null;
        }

        private void GridImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gridImage.Source != null && grid!=null)
            {
                System.Windows.Point pos = e.GetPosition(gridImage);             
                int x = (int)Math.Floor(pos.X * gridImage.Source.Width / gridImage.ActualWidth);
                int y = (int)Math.Floor(pos.Y * gridImage.Source.Height / gridImage.ActualHeight);
                SelectedCell = grid.gd.getCell(new OpenCvSharp.Point(x, y));
            }
        }

        public Grid Grid
        {
            get { return grid; }
        }
        void drawGridImage()
        {
            if(grid == null)
            {
                Mat empty = new Mat(1000, 1000, MatType.CV_8UC3, Scalar.RoyalBlue);
                gridImage.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(empty);
                return;
            }
            //Mat bild = grid.gd.drawGrid(true);
            Mat bild = grid.gd.drawGrid(showAll.IsChecked == true);
            if (GridImageDrawEvent != null) { GridImageDrawEvent(bild); }
            if(SelectedCell!=null)
            {
                grid.gd.markiereZelle(bild, SelectedCell.Value.X, SelectedCell.Value.Y, Scalar.Red, null);
            }            
            gridImage.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(bild); 
        }
        public void drawTogetherBild()
        {
            OrientiertesTeil ot = teilSelector.getSelected();
            Mat togetherbild = null;
            if (ot != null && SelectedCell != null)
            {
                togetherbild = grid.gd.drawNeighbourhood(ot, SelectedCell.Value);                
            }

            if (togetherbild!=null)
            {
                togetherImage.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(togetherbild);
            }
            else
            {
                togetherImage.Source = null;
            }

            if(bauFenster != null)
            {
                bauFenster.setImage(togetherbild);
            }
        }

        public void keyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)

            {
                case Key.Left:
                    SelectedCell = SelectedCell + new Point(-1, 0);
                    break;
                case Key.Right:
                    SelectedCell = SelectedCell + new Point(1, 0);
                    break;
                case Key.Up:
                    SelectedCell = SelectedCell + new Point(0, -1);
                    break;
                case Key.Down:
                    SelectedCell = SelectedCell + new Point(0, 1);
                    break;
                case Key.A:
                    teilSelector.last();
                    break;
                case Key.Y:
                    teilSelector.next();
                    break;
                case Key.Space:
                    if (SelectedCell!=null)
                    {
                        GridBelegung belegung = grid.getBelegung(SelectedCell.Value.X, SelectedCell.Value.Y);
                        OrientiertesTeil sel = teilSelector.getSelected();
                        if (belegung!=null && !belegung.isRand && belegung.OrientiertesTeil == null && sel !=null)
                        {
                            if (InsertRequestEvent != null) InsertRequestEvent(SelectedCell.Value.X, SelectedCell.Value.Y, sel);
                        }
                    }
                    break;
            }
        }
    }
}
