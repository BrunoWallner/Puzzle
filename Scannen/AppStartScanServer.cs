using ScannenServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using PuzzleBauen;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ScannenServer
{
    class AppStartScanServer : Application
    {
        [STAThread]
        static void Main()
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            string localRoot = currentDirectory.Parent.Parent.Parent.Parent.ToString();

            PathProvider.localPath = localRoot + "\\Daten\\server\\";
            PathProvider.httpPath = localRoot + "\\http";

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

            new AppStartScanServer().Run(new ScanCollectionCreator(dialog.FileName + "\\scan.col"));         
        }
    }
}
