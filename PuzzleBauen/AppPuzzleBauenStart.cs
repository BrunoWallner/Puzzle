using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PuzzleBauen
{
    class AppPuzzleBauenStart : Application
    {
        [STAThread]
        static void Main()
        {
            ///////////////  new PuzzleApp().Run(new ScanningWindow());
            //   new PuzzleApp().Run(new ScanCollectionCreator());
            // new PuzzleApp().Run(new PuTeColWin());

            //   new PuzzleApp().Run(new SelectionWin2());
            //string hostName = System.Net.Dns.GetHostName();
            //MessageBox.Show(hostName);
            //    new AppPuzzleBauenStart().Run(new GridWin(Grid.getInstance()));
            //new AppPuzzleBauenStart().Run(new test());




            //     new PuzzleApp().Run(new SendWin());


            //  MessageBox.Show("lkj");
            //www.sss();

            //var dialog = new CommonOpenFileDialog();
            //dialog.IsFolderPicker = true;
            //dialog.Title = "Projektordner auswählen";
            //CommonFileDialogResult result = dialog.ShowDialog();
            //if (result == CommonFileDialogResult.Cancel)
            //{
            //    throw new Exception("das geht nicht");
            //    //return;
            //}
            //PathProvider.ProjectFolder = dialog.FileName;

            ////new SegmentationApp().Run(new WebServerWin());
            ////return;
            ////new SegmentationApp().Run(new ColorCompareTest());
            ////return;
            //// new SegmentationApp().Run(new Bauen.PuzzleGridWin());
            ////new SegmentationApp().Run(new PuzzleTeilComparer());
            ////new SegmentationApp().Run(new TeilCreator());

            ////new SegmentationApp().Run(new Bauen.PuzzleGridWin());



            //new SegmentationApp().Run(new PuzzleGridWindow());  //bauen
            ////new SegmentationApp().Run(new PT.PuzzleTeilWindow()); //EckenFinden und so
            ////new SegmentationApp().Run(new SegmentationWindow()); //Scans zu puzzleteilen



        }
    }
}
