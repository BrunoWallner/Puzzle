using PuzzleBauen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Point = OpenCvSharp.Point;
using Rec = OpenCvSharp.Rect;

namespace PuzzleBauen
{   
    public class Grid
    {
        public event EventHandler BelegungChanged;
        public GridDrawing gd;
        Point positionInParent = new Point(0,0);
        GridBelegung[,] belegung;
        
        public List<KonkreteBelegung> getAlleBelegungen()
        {
            List<KonkreteBelegung> ret = new List<KonkreteBelegung>();
            for (int x = 0; x<Width; x++)
            {
                for(int y = 0; y<Height; y++)
                {
                    if (belegung[x,y].OrientiertesTeil!=null && belegung[x,y].OrientiertesTeil.TeilNummer>-1)
                    {
                        ret.Add(new KonkreteBelegung(x, y, belegung[x, y].OrientiertesTeil.TeilNummer, belegung[x, y].OrientiertesTeil.Orientierung));
                    }
                }
            }
            return ret;
        }
        public void onBelegungChanged()
        {
            if(BelegungChanged!=null) BelegungChanged(this, EventArgs.Empty);
        }
        public bool isZelleInGrid(Point p)
        {
            if (p.X < 0 || p.X >= Width || p.Y < 0 || p.Y >= Height) return false;
            return true;
        }
        public Grid getSubGrid(Point positionInParent, int width, int height)
        {
            Grid ret = new Grid(width, height);
            ret.positionInParent = positionInParent;
            for(int x = 0; x<width; x++)
            {
                for(int y = 0; y<height; y++)
                {
                    ret.belegung[x, y] = getBelegung(x + positionInParent.X, y + positionInParent.Y);
                }
            }
            return ret;
        }
        public Grid()
        {
            MessageBox.Show("Breite Auswählen!");
            createGrid(12, 23);
        }
  
        public Grid(int width, int height)
        {
            createGrid(width, height);
        }
        
        void createGrid(int width, int height)
        {           
            belegung = new GridBelegung[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if(i == 0 || j==0 || i == (width-1) || j == (height-1))
                    {
                        belegung[i, j] = new GridBelegung(true, null);
                    }
                    else
                    {
                        belegung[i, j] = new GridBelegung(false, null);// new OrientiertesTeil(-1, -1));
                    }

                }
            }     
            gd = new GridDrawing(this);
        }

        public void restartGrid()
        {
            int width = belegung.GetLength(0);
            int height = belegung.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || j == 0 || i == (width - 1) || j == (height - 1))
                    {
                        belegung[i, j] = new GridBelegung(true, null);
                    }
                    else
                    {
                        belegung[i, j] = new GridBelegung(false, null);// new OrientiertesTeil(-1, -1));
                    }

                }
            }
            onBelegungChanged();
        }
        public List<NeighbourSide> getNeighbourSides(int x, int y)
        {
            List<NeighbourSide> ret = new List<NeighbourSide>();
            ret.Add(getNeighbourSide(x, y, Neighbours.left));
            ret.Add(getNeighbourSide(x, y, Neighbours.bottom));
            ret.Add(getNeighbourSide(x, y, Neighbours.right));
            ret.Add(getNeighbourSide(x, y, Neighbours.top));
            return ret;
        }
        public NeighbourSide getNeighbourSide(int x, int y, Neighbours who)
        {
            int neighbourSideIndex = 0;
            GridBelegung neighbour = getNeighbour(x, y, who);
            switch (who)
            {
                case Neighbours.left:
                    neighbourSideIndex = 2;
                    break;
                case Neighbours.top:
                    neighbourSideIndex = 1;
                    break;
                case Neighbours.right:
                    neighbourSideIndex = 0;
                    break;
                case Neighbours.bottom:
                    neighbourSideIndex = 3;
                    break;
            }
            if (neighbour != null)
            {
                if (neighbour.isRand) return new NeighbourSide(true, null);
                if (neighbour.OrientiertesTeil == null) return new NeighbourSide(false, null);
                return new NeighbourSide(false, neighbour.OrientiertesTeil.getSeite(neighbourSideIndex));
            }
            else return new NeighbourSide(false, null);
        }
        public GridBelegung getNeighbour(int x, int y, Neighbours who)
        {         
            switch (who)
            {
                case Neighbours.left:
                    return getBelegung(x - 1, y);
                    break;
                case Neighbours.right:
                    return getBelegung(x + 1, y);
                    break;
                case Neighbours.bottom:
                    return getBelegung(x, y + 1);
                    break;
                case Neighbours.top:
                    return getBelegung(x, y - 1);
                    break;
            }
            return null;
        }

        public void remove(int x, int y)
        {
            GridBelegung bel = getBelegung(x, y);
            if (bel != null)
            {
                if (bel.isRand)
                {
                    MessageBox.Show("Rand kann nicht gelöscht werden");
                    return;
                }
                if (bel.OrientiertesTeil != null)
                {
                    int nr = bel.OrientiertesTeil.TeilNummer;
                    if (nr > -1)
                    {
                        TeilManager.getInstance().insert(nr);
                    }
                }
                belegung[x, y] = new GridBelegung(false, null);
                onBelegungChanged();
            }
        }

        public void insert(int X, int Y, OrientiertesTeil teil)
        {
            if (teil != null)
            {
                GridBelegung bel = getBelegung(X, Y);
                if (bel != null)
                {
                    if(bel.isRand)
                    {
                        //MessageBox.Show("Rand kann nicht bebaut werden");
                        //onBelegungChanged();
                        return;
                    }
                    else
                    {
                        if(bel.OrientiertesTeil != null)
                        {
                            //MessageBox.Show("Da ist schon ein Teil");
                            //onBelegungChanged();
                            return;
                        }
                        else
                        {
                            belegung[X, Y] = new GridBelegung(false, teil);
                            TeilManager.getInstance().teilVerbaut(teil.TeilNummer);
                            onBelegungChanged();
                        }

                    }
                }                
                else
                {
                    //MessageBox.Show("Insert gescheitert: außerhalb des Gitters (" + X.ToString() + "; " + Y.ToString() + ")");
                    //onBelegungChanged();
                }
            }
            else
            {
                //MessageBox.Show("Insert gescheitert: Einzufügendes teil ist null");
                //onBelegungChanged();
            }          

        }
        //public void insertGrid(Grid g)
        //{
        //    for (int x = 0; x < g.Width; x++)
        //    {
        //        for (int y = 0; y < g.Height; y++)
        //        {
        //            belegung[x + g.positionInParent.X, y + g.positionInParent.Y] = g.belegung[x, y];
        //            GridBelegung bel = g.belegung[x, y];
        //            if (bel!=null)
        //            {   
        //                if(bel.OrientiertesTeil != null)
        //                {
        //                    TeilManager.getInstance().teilVerbaut(bel.OrientiertesTeil.TeilNummer);
        //                }                        
                    
        //            }
        //        }
        //    }
        //    onBelegungChanged();
        //}     

        public OrientiertesTeil getOrientiertesTeil(int x, int y)
        {
            GridBelegung b = getBelegung(x, y);
            if(b!=null)
            {
                return b.OrientiertesTeil;
            }
            return null;
        }
        public GridBelegung getBelegung(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return belegung[x, y];
        }
        public int Width
        {
            get { return belegung.GetLength(0);}
        }
        public int Height
        {
            get { return belegung.GetLength(1);}
        }
        public byte[] toByteArray()
        {
            MemoryStream ms = new MemoryStream();
            writeToStream(ms);
            byte[] ret = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(ret, 0, ret.Length);
            return ret;            
        }

        public static Grid fromByteArray(byte[] arr)
        {
            MemoryStream ms = new MemoryStream(arr);
            return loadFromStream(ms);
        }
        public static Grid loadFromStream(Stream fs)
        {
            Int32 posx = Serializer.getInt32(fs);
            Int32 posy = Serializer.getInt32(fs);
            Int32 width = Serializer.getInt32(fs);                   
            Int32 height = Serializer.getInt32(fs);            
            Grid ret = new Grid(width, height);
            ret.positionInParent.X = posx;
            ret.positionInParent.Y = posy;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridBelegung be = GridBelegung.loadFromStream(fs);
                    ret.belegung[x, y] = be;
                    if (be == null) MessageBox.Show("null");
                    else
                    {
                        if (be.OrientiertesTeil != null)
                        {
                            TeilManager.getInstance().teilVerbaut(be.OrientiertesTeil.TeilNummer);// be.OrientiertesTeil.TeilNummer);                 
                        }
                    }
                }
            }
            return ret;
        }
        public void writeToStream(Stream fs)
        {
            Serializer.Add((Int32)positionInParent.X, fs);
            Serializer.Add((Int32)positionInParent.Y, fs);
            Serializer.Add((Int32)Width, fs);
            Serializer.Add((Int32)Height, fs);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    belegung[x, y].writeToStream(fs);
                }
            }
        }       

    }
}
