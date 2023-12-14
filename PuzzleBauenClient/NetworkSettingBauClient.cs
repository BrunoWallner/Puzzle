//using PuzzleBauen;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using System.Windows;
//using Grid = System.Windows.Controls.Grid;
//using System.Net.Sockets;
//using System.Net;
//using System.Windows.Forms;
//using TextBox = System.Windows.Controls.TextBox;
//using Label = System.Windows.Controls.Label;
//using MessageBox = System.Windows.MessageBox;

//namespace PuzzleBauenClient
//{
//    internal class NetworkSettingBauClient
//    {
//        //public static bool Show2()
//        //{
//        //    NetworkSettingWindow n = new NetworkSettingWindow();           
//        //    if(n.ShowDialog() == true)
//        //    {
//        //        return true;
//        //    }
//        //    return false;
//        //}
//        //public static void Show()
//        //{
//        //    NetworkSettingWindow n = new NetworkSettingWindow();
//        //    n.ShowDialog();         
//        //}
//        //public static void ShowIPs()
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
//        //    MessageBox.Show(ips);
//        //}
//        //public static string ServerName = "127.0.0.1";
//        //public static int ServerPort = 21000;
//        //public static int FilePort = 22000;
//        //public static string BaumeisterName = "hans";

//        //class NetworkSettingWindow : Window
//        //{

//        //    Grid g = new Grid();
//        //    TextBox serverName = new TextBox();
//        //    TextBox ServerPortNumber = new TextBox();
//        //    TextBox FilePortNumber = new TextBox();
//        //    TextBox Baumeister = new TextBox();

//        //    public NetworkSettingWindow()
//        //    {
//        //        this.Title = "NetzwerkEinstellung BauClient";
//        //        this.Content = g;
//        //        Label lServer = new Label();
//        //        lServer.Content = "ServerName";
//        //        Label lServerPort = new Label();
//        //        lServerPort.Content = "ServerPort";
//        //        Label lFilePort = new Label();
//        //        lFilePort.Content = "ServerPort";
//        //        Label lBaumeister = new Label();
//        //        lBaumeister.Content = "Baumeister";


//        //        g.Children.Add(serverName);
//        //        g.Children.Add(ServerPortNumber);
//        //        g.Children.Add(lServer);
//        //        g.Children.Add(lServerPort);
//        //        g.Children.Add(lFilePort);
//        //        g.Children.Add(FilePortNumber);
//        //        g.Children.Add(Baumeister);
//        //        g.Children.Add(lBaumeister);


//        //        g.RowDefinitions.Add(new RowDefinition());
//        //        g.RowDefinitions.Add(new RowDefinition());
//        //        g.RowDefinitions.Add(new RowDefinition());
//        //        g.RowDefinitions.Add(new RowDefinition());
//        //        g.RowDefinitions.Add(new RowDefinition());
//        //        g.ColumnDefinitions.Add(new ColumnDefinition());
//        //        g.ColumnDefinitions.Add(new ColumnDefinition());


//        //        Grid.SetColumn(lServer, 0);
//        //        Grid.SetRow(lServer, 0);
//        //        Grid.SetColumn(lServerPort, 0);
//        //        Grid.SetRow(lServerPort, 1);
//        //        Grid.SetColumn(lFilePort, 0);
//        //        Grid.SetRow(lFilePort, 2);
//        //        Grid.SetColumn(lBaumeister, 0);
//        //        Grid.SetRow(lBaumeister, 3);


//        //        Grid.SetColumn(serverName, 1);
//        //        Grid.SetRow(serverName, 0);
//        //        Grid.SetColumn(ServerPortNumber, 1);
//        //        Grid.SetRow(ServerPortNumber, 1);
//        //        Grid.SetColumn(FilePortNumber, 1);
//        //        Grid.SetRow(FilePortNumber, 2);
//        //        Grid.SetColumn(Baumeister, 1);
//        //        Grid.SetRow(Baumeister, 3);



//        //        AutoActionButton okB = new AutoActionButton("OK", ok);                
//        //        g.Children.Add(okB);

//        //        Grid.SetColumn(okB, 0);
//        //        Grid.SetRow(okB, 4);
//        //        Grid.SetColumnSpan(okB, 2);

//        //        serverName.Text = NetworkSettingBauClient.ServerName;
//        //        ServerPortNumber.Text = NetworkSettingBauClient.ServerPort.ToString();
//        //        FilePortNumber.Text = NetworkSettingBauClient.FilePort.ToString();
//        //        Baumeister.Text = NetworkSettingBauClient.BaumeisterName;
//        //        this.SizeToContent = SizeToContent.Height;
//        //        this.Width = 400;


//        //    }
//        //    void ok()
//        //    {
//        //        int serverPort;
//        //        int filePort;
//        //        if (int.TryParse(ServerPortNumber.Text, out serverPort) && int.TryParse(FilePortNumber.Text, out filePort))
//        //        {
//        //            NetworkSettingBauClient.ServerPort = serverPort;
//        //            NetworkSettingBauClient.FilePort = filePort;
//        //            NetworkSettingBauClient.ServerName = serverName.Text;
//        //            NetworkSettingBauClient.BaumeisterName = Baumeister.Text;
//        //        }
//        //        this.DialogResult = true;
//        //        this.Close();
//        //    }



//        //}
//    }
//}
