using PuzzleBauenClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

using PuzzleBauen;
using Microsoft.VisualBasic.Devices;
using System.DirectoryServices;
using ClientServerCom;

namespace PuzzleBauenClient
{
    public class AppPuzzleBauenClientStart : Application
    {
        [STAThread]
        public static void Main()
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            string localRoot = currentDirectory.Parent.Parent.Parent.Parent.ToString();
            PathProvider.localPath = localRoot + "\\Daten\\client\\";
            PathProvider.httpPath = localRoot + "\\http";

            Directory.SetCurrentDirectory(PathProvider.httpPath);
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "http.exe";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = false;
            psi.Arguments = "";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process httpProc = Process.Start(psi);
            Directory.SetCurrentDirectory(currentDirectory.ToString());



            new AppPuzzleBauenClientStart().Run(new ClientStarter());
            httpProc.Kill(true);

        }
    }
}
