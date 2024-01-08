using Microsoft.WindowsAPICodePack.Dialogs;
using PuzzleBauenServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
//using Microsoft.WindowsAPICodePack.Dialogs;

namespace PuzzleBauen
{
    class AppPuzzleBauernServerStart : System.Windows.Application
    {
        [STAThread]
        static void Main()
        {            
            DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            string localRoot = currentDirectory.Parent.Parent.Parent.Parent.ToString();

            //PathProvider.localPath = localRoot + "\\Daten\\server\\";
            //PathProvider.httpPath = localRoot + "\\http";

            
            
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;            
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                PathProvider.localPath = dialog.FileName + "\\";
            }
            else
            {
                return;
            }
            
            
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.ShowDialog();
            //ofd.Title = "ScanCollection auswählen";
            //if (ofd.ShowDialog() == DialogResult.OK)
            //{

            //    //try
            //    //{
            //    //    FileStream fs = new FileStream(PathProvider.getScanCollectionFile(), FileMode.Open);
            //    //    PuzzleScanCollection.Instance = PuzzleScanCollection.load(fs);
            //    //    fs.Close();
            //    //}

            //}
            //else
            //{
            //    System.Windows.MessageBox.Show("keine ScanCollectionDatei");
            //    return;
            //}


            if (File.Exists(PathProvider.getScanCollectionFile()))
            {
                FileStream fs = new FileStream(PathProvider.getScanCollectionFile(), FileMode.Open);
                PuzzleScanCollection.Instance = PuzzleScanCollection.load(fs);
                fs.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("keine ScanCollectionDatei unter \"" + PathProvider.getScanCollectionFile());
                return;
            }
            Grid g = null;
            if (File.Exists(PathProvider.getGridFile()))
            {
                if (System.Windows.MessageBox.Show("gespeichertes Grid laden?", "laden", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    FileStream fs = new FileStream(PathProvider.getGridFile(), FileMode.Open);
                    g = Grid.loadFromStream(fs);
                    fs.Close();
                }
            }
            if (g == null)
            {
                OpenCvSharp.Point ps = PuzzleSizeWin.Show();
                //OpenCvSharp.Point ps = new OpenCvSharp.Point(5, 6);
                g = new Grid(ps.X+2, ps.Y+2);
            }


            new AppPuzzleBauernServerStart().Run(new ServerWin(g));
        }
    }
}
