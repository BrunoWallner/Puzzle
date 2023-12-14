using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientServerCom
{
    class AppClientServerCom : Application
    {
        [STAThread]
        static void Main()
        {
            //new AppClientServerCom().Run(new CSComServerTestWin());
            //DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            //string localRoot = currentDirectory.Parent.Parent.Parent.Parent.ToString();

            //PathProvider.localPath = localRoot + "\\Daten\\server\\";
            //PathProvider.httpPath = localRoot + "\\http";
            //new AppClientServerCom().Run(new ServerWin());

        }
    }
}
