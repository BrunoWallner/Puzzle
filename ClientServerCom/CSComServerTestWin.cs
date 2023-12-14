//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using Xceed.Wpf.Toolkit;
//using System.Reflection;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;
//using OpenCvSharp;
//using MessageBox = System.Windows.MessageBox;
//using Window = System.Windows.Window;
//using winPoint = System.Windows.Point;
//using Point = OpenCvSharp.Point;
//using System.Xml.Serialization;
//using Microsoft.Win32;
//using System.Collections;
//using System.Diagnostics;
//using System.Net.Sockets;
//using System.Net;
//using System.Windows.Forms;
//using ListBox = System.Windows.Controls.ListBox;
//using PuzzleBauen;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using System.Windows.Markup;
//using MS.WindowsAPICodePack.Internal;
//using System.Windows.Threading;
//using Grid = PuzzleBauen.Grid;
//using ToolBar = System.Windows.Controls.ToolBar;
//using Label = System.Windows.Controls.Label;
//using System.Threading;

//namespace ClientServerCom
//{
//    public class CSComServerTestWin : Window
//    {
//        DockPanel dp = new DockPanel();
//        //ToolBar tb = new ToolBar();
//        StackPanel sp = new StackPanel();
//        ListBox logList = new ListBox();

//        Server server;

//        public CSComServerTestWin()
//        {
//            //this.Title = "CSComServerTestWin";
//            //this.Content = dp;
//            ////dp.Children.Add(tb);
//            ////DockPanel.SetDock(tb, Dock.Top);
//            //dp.Children.Add(sp);
//            //DockPanel.SetDock(sp, Dock.Left);
//            //dp.Children.Add(logList);




//            //sp.Children.Add(new AutoActionButton("CreateClient", create));


//            ////tb.Items.Add(new AutoActionButton("zeigealle", zeig));
//            ////bauServer.log += BauServer_log;
//            ////bauServer.gridUpdatey += BauServer_gridUpdate;
//            //server = new Server(this.Dispatcher);
//            //server.logEvent += Server_logEvent;
//            //Server_logEvent(null, null);
//        }
//        void create()
//        {
//            Thread th = new Thread(threaddd);
//            th.SetApartmentState(ApartmentState.STA);
//            th.Start();
//        }

//        void threaddd()
//        {
//            CSComClientTestWin cw = new CSComClientTestWin("127.0.0.1");
//            cw.Show();
//            System.Windows.Threading.Dispatcher.Run();
//        }
//        private void Server_logEvent(object? sender, string e)
//        {
//            logList.Items.Clear();
//            foreach(string s in server.LogList)
//            {
//                logList.Items.Add(s);
//            }
//        }


//        //void ip()
//        //{
//        //    String ips = "IP Addressen\r\n";
//        //    var host = Dns.GetHostEntry(Dns.GetHostName());
//        //    foreach (var ip in host.AddressList)
//        //    {
//        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
//        //        {
//        //            ips += ip.ToString() + "\r\n";
//        //        }
//        //    }
//        //    ips += "\r\n ServerPort: " + NetworkSettingBauServer.ServerPort;
//        //    ips += "\r\n FilePort: " + NetworkSettingBauServer.FilePort;
//        //    MessageBox.Show(ips);
//        //}
//        //void aktVerb()
//        //{
//        //    if (gws == null)
//        //    {
//        //        return;
//        //    }
//        //    List<int> verb = TeilManager.getInstance().getVerbauteTeile();
//        //    foreach (PuzzleBauClient client in bauServer.kunden)
//        //    {
//        //        client.neueVerbauteTeile(verb);
//        //    }
//        //    //PuzzleBauClient sel = ClientSelection.selectClient(bauServer.kunden);
//        //    //if (sel != null) sel.neueVerbauteTeile(verb);
//        //}

//        //private void BauServer_gridUpdate(object? sender, Grid e)
//        //{
//        //    gws.Grid.insertGrid(e);

//        //}

//        //private void BauServer_log(object? sender, string e)
//        //{
//        //    this.Dispatcher.Invoke(new Action(() => { logList.Items.Add(e); }));
//        //}



//        //public void export(Grid g)
//        //{
//        //    PuzzleBauClient sel = ClientSelection.selectClient(bauServer.kunden);
//        //    if (sel != null) sel.neuerAuftrag(g);
//        //}

//    }

//}
