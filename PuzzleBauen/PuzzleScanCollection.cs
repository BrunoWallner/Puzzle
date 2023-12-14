using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PuzzleBauen
{
    public class PuzzleScanCollection
    {
        static PuzzleScanCollection instance = null;
        public static PuzzleScanCollection Instance 
        { 
            get 
            {
                if (instance == null)
                {
                    MessageBox.Show("psc-inszan null");
                    FileStream fs = new FileStream(PathProvider.getScanCollectionFile(), FileMode.Open);
                    instance = PuzzleScanCollection.load(fs);
                    fs.Close();
                }
                return instance; 
            }
            set
            {
                instance = value;
            }
        }

        
        public static PuzzleScanCollection getInstance()
        {
            if (instance == null)
            {
                FileStream fs = new FileStream(PathProvider.getScanCollectionFile(), FileMode.Open);
                instance = PuzzleScanCollection.load(fs);
                fs.Close();
                instance.nextScanIndex = instance.scans.Count + 1;
            }
            return instance;
        }
        List<PuzzleScan> scans = new List<PuzzleScan>();
        public List<PuzzleScan> getScans() { return scans; }
        int nextScanIndex = 1;
        public int addScan(PuzzleScan scan)
        {
            alleTeile = null;
            scan.id = nextScanIndex;// scans.Count + 1;
            nextScanIndex++;
            scans.Add(scan);
            return scan.id;
        }

        public void removeScan(PuzzleScan scan)
        {
            alleTeile = null;
            //scan.id = counter;// scans.Count + 1;
            //counter++;
            //scans.Add(scan);
            //return scan.id;
            scans.Remove(scan);

            
        }
        public PuzzleScanCollection()
        {
            nextScanIndex = 1;
        }
        List<PuzzleTeil> alleTeile = null;
        public List<int> getAllIDs()
        {
            List<int> ret = new List<int>();
            for(int i = 0; i< getTeile().Count; i++)
            {
                ret.Add(i);
            }
            return ret;
        }
        public PuzzleTeil getTeil(int id)
        {
            if (id < 0 || id >= getTeile().Count) return null;
            return getTeile()[id];
        }
        public List<PuzzleTeil> getTeile()
        {
            if (alleTeile == null)
            {
                alleTeile = new List<PuzzleTeil>();
                foreach (PuzzleScan scan in scans) alleTeile.AddRange(scan.teile);
            }
            return alleTeile;
        }
        public static PuzzleScanCollection load(Stream fs)
        {
            PuzzleScanCollection col = new PuzzleScanCollection();
            Int32 count = Serializer.getInt32(fs);
            for (int i = 0; i < count; i++)
            {
                //col.scans.Add(PuzzleScan.load(fs));
                col.addScan(PuzzleScan.load(fs));
            }
            return col;
        }
        public void writeToStream(Stream fs)
        {
            Serializer.Add((Int32)scans.Count, fs);
            for (int i = 0; i < scans.Count; i++)
            {
                scans[i].writeToStream(fs);
            }
        }

    }
}
