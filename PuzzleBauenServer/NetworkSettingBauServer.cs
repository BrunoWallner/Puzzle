//using PuzzleBauen;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using Xceed.Wpf.AvalonDock.Layout;
//using Grid = System.Windows.Controls.Grid;

//namespace PuzzleBauenServer
//{
    //internal class NetworkSettingBauServer
    //{

        //public static int ServerPort = 21000;
        //public static int FilePort = 22000;
        //public static void ShowIPs()
        //{
        //    String ips = "IP Addressen\r\n";
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (var ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        {
        //            ips += ip.ToString() + "\r\n";
        //        }
        //    }
        //    ips += "\r\n ServerPort: " + NetworkSettingBauServer.ServerPort;
        //    ips += "\r\n FilePort: " + NetworkSettingBauServer.FilePort;
        //    MessageBox.Show(ips);
        //}


        //class NetworkSettingWindow : Window
        //{
        //    //public static void Show()
        //    //{
        //    //    NetworkSettingWindow n = new NetworkSettingWindow();
        //    //    n.ShowDialog();
        //    //}

        //    Grid g = new Grid();
        //    TextBox serverName = new TextBox();
        //    TextBox portNumber = new TextBox();

        //    public NetworkSettingWindow()
        //    {
        //        this.Title = "NetzwerkEinstellung";
        //        this.Content = g;
        //        Label lServer = new Label();
        //        lServer.Content = "ServerName";
        //        Label lPort = new Label();
        //        lPort.Content = "Port";

        //        g.Children.Add(serverName);
        //        g.Children.Add(portNumber);
        //        g.Children.Add(lServer);
        //        g.Children.Add(lPort);
        //        g.RowDefinitions.Add(new RowDefinition());
        //        g.RowDefinitions.Add(new RowDefinition());
        //        g.RowDefinitions.Add(new RowDefinition());
        //        g.ColumnDefinitions.Add(new ColumnDefinition());
        //        g.ColumnDefinitions.Add(new ColumnDefinition());
        //        //g.ColumnDefinitions.Add(new ColumnDefinition());

        //        Grid.SetColumn(lServer, 0);
        //        Grid.SetRow(lServer, 0);

        //        Grid.SetColumn(lPort, 0);
        //        Grid.SetRow(lPort, 1);

        //        Grid.SetColumn(serverName, 1);
        //        Grid.SetRow(serverName, 0);

        //        Grid.SetColumn(portNumber, 1);
        //        Grid.SetRow(portNumber, 1);



        //        AutoActionButton okB = new AutoActionButton("OK", ok);
        //        //AutoActionButton nokB = new AutoActionButton("Nein", notOk);
        //        g.Children.Add(okB);
        //        //g.Children.Add(nokB);

        //        Grid.SetColumn(okB, 0);
        //        Grid.SetRow(okB, 2);
        //        Grid.SetColumnSpan(okB, 2);

        //        //serverName.Text = NetworkSettingBauServer.ServerName;
        //        //portNumber.Text = NetworkSettingBauServer.Port.ToString();
        //        this.SizeToContent = SizeToContent.Height;
        //        this.Width = 400;


        //    }
        //    void ok()
        //    {
        //        int port;
        //        if (int.TryParse(portNumber.Text, out port))
        //        {
        //            //NetworkSettingBauServer.Port = port;
        //            //NetworkSettingBauServer.ServerName = serverName.Text;
        //        }
        //        this.DialogResult = true;
        //        this.Close();
        //    }
        //}
//    }
   
//}
