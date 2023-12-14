using PuzzleBauen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Window = System.Windows.Window;
using PuzzleBauen;

using System.IO;
using OpenCvSharp;
using Point = OpenCvSharp.Point;
using ClientServerCom;
using System.Diagnostics.Eventing.Reader;
using Environment = PuzzleBauenClient.Environment;
using System.Drawing;
using Grid = System.Windows.Controls.Grid;
using OpenCvSharp.Dnn;

namespace PuzzleBauenClient
{

    internal class ClientStarter : Window
    {
        System.Windows.Controls.Grid settingGrid = new System.Windows.Controls.Grid();
        DockPanel dp = new DockPanel();
        ToolBar tb = new ToolBar();
        StackPanel sp = new StackPanel();
        ListBox logList = new ListBox();
        AutoActionButton conButton;
        public ClientStarter()
        {
            this.Title = "Client Starter";
            this.Content = dp;

            dp.Children.Add(logList);
            DockPanel.SetDock(logList, Dock.Left);
            logList.Items.Add("Log");
            dp.Children.Add(sp);
            sp.Children.Add(settingGrid);
            initGrid();

            conButton = new AutoActionButton("connect", con);
            conButton.FontSize = 40;

            sp.Children.Add(conButton);
            
            this.Width = 600;
            this.Height = 600;
        }

        TextBox serverName = new TextBox();
        TextBox Baumeister = new TextBox();
        void initGrid()
        {
            Label lServer = new Label();
            lServer.FontSize = 30;
            lServer.Content = "ServerName";
            Label lBaumeister = new Label();
            lBaumeister.FontSize = 30;
            lBaumeister.Content = "Bauer";
            serverName.FontSize = 30;
            Baumeister.FontSize = 30;

            settingGrid.Children.Add(serverName);
            settingGrid.Children.Add(lServer);
            settingGrid.Children.Add(Baumeister);
            settingGrid.Children.Add(lBaumeister);

            settingGrid.RowDefinitions.Add(new RowDefinition());
            settingGrid.RowDefinitions.Add(new RowDefinition());
            settingGrid.ColumnDefinitions.Add(new ColumnDefinition());
            settingGrid.ColumnDefinitions.Add(new ColumnDefinition());


            Grid.SetColumn(lServer, 0);
            Grid.SetRow(lServer, 0);
            Grid.SetColumn(lBaumeister, 0);
            Grid.SetRow(lBaumeister, 1);


            Grid.SetColumn(serverName, 1);
            Grid.SetRow(serverName, 0);
            Grid.SetColumn(Baumeister, 1);
            Grid.SetRow(Baumeister, 1);

            try
            {
                StreamReader sr = new StreamReader("localClientServerSetting.txt");
                serverName.Text = sr.ReadLine();
                Baumeister.Text = sr.ReadLine();
                sr.Close();
            }
            catch
            {
                serverName.Text = "127.0.0.1";
                Baumeister.Text = "bob";
            }
        }
        async void con()
        {
            try
            {
                StreamWriter sr = new StreamWriter("localClientServerSetting.txt");
                sr.WriteLine(serverName.Text);
                sr.WriteLine(Baumeister.Text);
                sr.Close();
            }
            catch
            {

            }
            conButton.IsEnabled = false;
            ClientServerCom.ClientCS client;
            logList.Items.Add("Hole Environment ...");
            Task<Environment> r = Environment.vonServerHolen(serverName.Text);
            await r;
            Environment environment = r.Result;
            if (environment != null)
            {
                logList.Items.Add("Environment da");
                client = new ClientCS(Baumeister.Text, serverName.Text, this.Dispatcher);
                logList.Items.Add("Connect ...");
                if (await client.connect())
                {
                    logList.Items.Add("Connected");
                    PuzzleBauen.Grid zuBearbeiten = new PuzzleBauen.Grid(environment.Size.X, environment.Size.Y);
                    PuzzleScanCollection.Instance = environment.ScanCollection;
                    BauClientWin bcw = new BauClientWin(client, zuBearbeiten);
                    bcw.Show();
                    this.Close();
                }
                else
                {
                    logList.Items.Add("Connecting gescheitert");
                }
            }
            else
            {
                logList.Items.Add("Environment holen gescheitert");
            }
            conButton.IsEnabled = true;
        }
    }
}