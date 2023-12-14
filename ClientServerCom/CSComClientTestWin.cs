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

//namespace ClientServerCom
//{
//    public class CSComClientTestWin : Window
//    {
//        DockPanel dp = new DockPanel();
//        StackPanel sp = new StackPanel();
//        ListBox logList = new ListBox();


//        static int kundenNummer = 1;
//        ClientCS client;
//        public CSComClientTestWin(string serverName)
//        {
//            this.Title = "CSComClientTestWin";
//            this.Content = dp;
//            //dp.Children.Add(tb);
//            //DockPanel.SetDock(tb, Dock.Top);
//            dp.Children.Add(sp);
//            DockPanel.SetDock(sp, Dock.Left);
//            dp.Children.Add(logList);


//            sp.Children.Add(new AutoActionButton("Connect", connect));
//            sp.Children.Add(new AutoActionButton("Disconnect", disconnect));
//            //sp.Children.Add(new AutoActionButton("ScanCollectionHolen", scanHolen));
//            //sp.Children.Add(new AutoActionButton("Grid anfordern", gridAnfordern));


//            client = new ClientCS(kundenNummer.ToString(), serverName, this.Dispatcher);
//            kundenNummer++;
//            client.logEvent += Client_logEvent;
//            client.connectionChangedEvent += Client_connectionChangedEvent;

//            //client.connect();

//            this.Closing += CSComClientTestWin_Closing;
//        }    
//        //void scanHolen()
//        //{
//        //    PuzzleScanCollection p = client.scanCollectionHolen();
//        //    MessageBox.Show(p.getTeile().Count.ToString());
//        //}
//        //void gridAnfordern()
//        //{
//        //    client.gridAnfordern();

//        //}
//        void connect()
//        {
//            //client.connect();
//        }
//        void disconnect()
//        {
//            client.disconnect();
//        }
//        private void Client_connectionChangedEvent(object? sender, EventArgs e)
//        {
//            //throw new NotImplementedException();
//        }

//        private void Client_logEvent(object? sender, string e)
//        {
//            logList.Items.Clear();
//            foreach (string s in client.LogList)
//            {
//                logList.Items.Add(s);
//            }
//        }

//        private void CSComClientTestWin_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
//        {
//            client.disconnect();
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
