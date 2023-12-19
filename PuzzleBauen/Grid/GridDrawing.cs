using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using Point = OpenCvSharp.Point;

namespace PuzzleBauen
{
    public class GridDrawing
    {

        Grid grid;
        public int xSize = 40;
        public int ySize = 40;
        double scale = 0.2;
        public GridDrawing(Grid grid)
        {
            this.grid = grid;
            grid.BelegungChanged += Grid_BelegungChanged;         
        }

        private void Grid_BelegungChanged(object? sender, EventArgs e)
        {
            completeTeilBild = null;
            partialTeilBild = null;
        }

        public OpenCvSharp.Point getCell(OpenCvSharp.Point p)
        {
            int x = p.X / xSize;
            int y = p.Y / ySize;
            return new OpenCvSharp.Point(x, y);
        }
        public Mat drawNeighbourhood(OrientiertesTeil center, Point zell)
        {
            OrientiertesTeil d = grid.getOrientiertesTeil(zell.X, zell.Y);
            OrientiertesTeil left = grid.getOrientiertesTeil(zell.X-1, zell.Y);
            OrientiertesTeil right = grid.getOrientiertesTeil(zell.X+1, zell.Y);
            OrientiertesTeil top = grid.getOrientiertesTeil(zell.X, zell.Y-1);
            OrientiertesTeil bottom = grid.getOrientiertesTeil(zell.X, zell.Y+1);

            Mat dd = PTDraw.drawWithNeighbours2(center, left, bottom, right, top);
            return dd;
        }
        


        void drawOutside(Mat mat, int x, int y, Scalar scal)
        {
            Cv2.Rectangle(mat, new OpenCvSharp.Rect(x * xSize, y * ySize, xSize, ySize), scal, -1);
        }
        void drawTeil(Mat mat, PuzzleTeil teil, int x, int y, int Ecke)
        {
            int nextEck = Ecke + 1;
            OpenCvSharp.Point targetPosition = new OpenCvSharp.Point(x * xSize, y * ySize);
            OpenCvSharp.Point puzPoint = teil.getEcke(Ecke);//  teil.getContour()[teil.Ecken[Ecke]];
            OpenCvSharp.Point nextPuzPoint = teil.getEcke(nextEck);// teil.getContour()[teil.Ecken[nextEck]];
            double puzWinkel = Transformer.getWinkel(puzPoint, nextPuzPoint);
            PTDraw.drawMe(mat, teil, scale, puzPoint, puzWinkel, targetPosition, -90);
        }
        Mat completeTeilBild = null;
        Mat partialTeilBild = null;
        void createcompleteTeilBild()
        {
            int height = grid.Height;// Grid.getInstance().Height;
            int width = grid.Width;// Grid.getInstance().Width;
            completeTeilBild = new Mat(height * ySize, width * xSize, MatType.CV_8UC3, Scalar.White);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridBelegung belegung = grid.getBelegung(x, y);
                    if (belegung.isRand)
                    {
                        drawOutside(completeTeilBild, x, y, Scalar.Black);
                    }
                    else
                    {
                        OrientiertesTeil ot = belegung.OrientiertesTeil;
                        if(ot == null || ot.PuzzleTeil == null)
                        {
                            drawOutside(completeTeilBild, x, y, Scalar.AliceBlue);
                        }
                        else
                        {
                            drawTeil(completeTeilBild, ot.PuzzleTeil, x, y, ot.Orientierung);                        
                        }
                    }
                }
            }
            for (int i = 1; i < width; i++)// xSize; i++)
            {
                Cv2.Line(completeTeilBild, i * xSize, 0, i * xSize, completeTeilBild.Rows, Scalar.Blue);
            }
            for (int i = 1; i < height; i++)// ySize; i++)
            {
                Cv2.Line(completeTeilBild, 0, i * ySize, completeTeilBild.Cols, i * ySize, Scalar.Blue);
            }
        }

        void createPartialTeilBild()
        {
            int height = grid.Height;
            int width = grid.Width;
            partialTeilBild = new Mat(height * ySize, width * xSize, MatType.CV_8UC3, Scalar.White);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridBelegung belegung = grid.getBelegung(x, y);
                    if (belegung.isRand)
                    {
                        drawOutside(partialTeilBild, x, y, Scalar.Black);
                    }
                    else
                    {
                        OrientiertesTeil ot = belegung.OrientiertesTeil;
                        if (ot == null || ot.PuzzleTeil == null)
                        {
                            drawOutside(partialTeilBild, x, y, Scalar.AliceBlue);
                        }
                        else
                        {
                            drawOutside(partialTeilBild, x, y, Scalar.BlueViolet);
                            //drawTeil(completeTeilBild, ot.PuzzleTeil, x, y, ot.Orientierung);
                        }
                    }
                    //OrientiertesTeil ot = grid.getBelegung(x, y).OrientiertesTeil;//grid.Belegung[x, y];
                    //{


                    //    if (ot.TeilNummer == -1)
                    //    {
                    //        drawOutside(partialTeilBild, x, y, Scalar.AliceBlue);
                    //    }
                    //    else
                    //    {
                    //        drawOutside(partialTeilBild, x, y, Scalar.Green);
                    //    }
                    //}

                }
            }
            for (int i = 1; i < width; i++)// xSize; i++)
            {
                Cv2.Line(partialTeilBild, i * xSize, 0, i * xSize, partialTeilBild.Rows, Scalar.Blue);
            }
            for (int i = 1; i < height; i++)// ySize; i++)
            {
                Cv2.Line(partialTeilBild, 0, i * ySize, partialTeilBild.Cols, i * ySize, Scalar.Blue);
            }
        }

        public Mat getBild(bool all)
        {
            //if (all)
            //{
            //    if (completeTeilBild == null) createcompleteTeilBild();
            //    Mat ret = new Mat();
            //    completeTeilBild.CopyTo(ret);
            //    return ret;
            //}
            //else
            {
                if (partialTeilBild == null) createPartialTeilBild();
                Mat ret = new Mat();
                partialTeilBild.CopyTo(ret);
                return ret;
            }
        }



        public void markiereZelle(Mat m, int x, int y, Scalar? randColor, Scalar? fillColor)
        {
            OpenCvSharp.Point p1 = new OpenCvSharp.Point(x * xSize, y * ySize);
            OpenCvSharp.Point p2 = new OpenCvSharp.Point((x + 1) * xSize, y * ySize);
            OpenCvSharp.Point p3 = new OpenCvSharp.Point((x + 1) * xSize, (y + 1) * ySize);
            OpenCvSharp.Point p4 = new OpenCvSharp.Point(x * xSize, (y + 1) * ySize);
            if (fillColor != null)
            {
                Cv2.Rectangle(m, new OpenCvSharp.Rect(x * xSize, y * ySize, xSize, ySize), fillColor.Value, -1);
            }
            if (randColor != null)
            {
                Cv2.Rectangle(m, new OpenCvSharp.Rect(x * xSize, y * ySize, xSize, ySize), randColor.Value, 1);
                //Scalar color = randColor.Value;
                //Cv2.Line(m, p1, p2, color, 2);
                //Cv2.Line(m, p2, p3, color, 2);
                //Cv2.Line(m, p3, p4, color, 2);
                //Cv2.Line(m, p4, p1, color, 2);
            }

        }


        //public Mat draw(bool all, Point gewählteZelle)
        //{
        //    Mat ret = getBild();
        //    //markiereZelle(ret, gridWin.SelectedCell.X, gridWin.SelectedCell.Y, Scalar.Red, null);
        //    markiereZelle(ret, gewählteZelle.X, gewählteZelle.Y, Scalar.Red, null);
        //    foreach (Auftrag a in AuftragsManager.getInstance().Aufträge)
        //    {
        //        markiereZelle(ret, a.X, a.Y, null, Scalar.LightGreen);
        //    }



        //    return ret;
        //}
        public Mat drawGrid(bool all)
        {
            Mat ret = getBild(all);            
            return ret;
        }
    }
}
