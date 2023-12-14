using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleBauen
{
    public class PathProvider
    {
        //das muss anders werden
        public static string localPath = "c:\\Users\\heigl\\Desktop\\paris2\\";        
        public static string getScanCollectionFile()
        {
            return localPath + "scan.col";           
        }
        public static string getGridFile()
        {
            return localPath + "grsid.grid";
        }


        public static string httpPath = "";
        public static string getWWWPath()
        {
            return httpPath + "\\web";
        }
    }
}
