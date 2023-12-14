using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using OpenCvSharp;
using MessageBox = System.Windows.MessageBox;
using Window = System.Windows.Window;
using winPoint = System.Windows.Point;
using Point = OpenCvSharp.Point;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Collections;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using ListBox = System.Windows.Controls.ListBox;
using PuzzleBauen;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Markup;
using MS.WindowsAPICodePack.Internal;
using System.Windows.Threading;
using Grid = PuzzleBauen.Grid;
using ToolBar = System.Windows.Controls.ToolBar;
using Label = System.Windows.Controls.Label;
using ClientServerCom;

namespace PuzzleBauenServer
{
    public class ServerWin : Window
    {
        DockPanel dp = new DockPanel();
        ToolBar tb = new ToolBar();
        StackPanel sp = new StackPanel();
        Label nochVorhandenLabel = new Label();

        ListBox logList = new ListBox();

        ClientServerCom.Server server;
        GridPanelServer gws;
        Grid grid;
        public ServerWin(Grid grid)
        {
            this.grid = grid;
            this.Title = "PuzzleBauSever, Servername: \"" + System.Net.Dns.GetHostName() + "\"";
            this.Content = dp;
            dp.Children.Add(tb);
            DockPanel.SetDock(tb, Dock.Top);
            dp.Children.Add(sp);
            DockPanel.SetDock(sp, Dock.Left);


            dp.Children.Add(logList);

            gws = new GridPanelServer(this, grid);
            dp.Children.Add(gws);

            tb.Items.Add(new AutoActionButton("Kill", kill));
            tb.Items.Add(new AutoActionButton("Save", save));
            tb.Items.Add(new AutoActionButton("getIP", ip));
            ServerWin_teileGeändertEvent(null, null);
            //nochVorhandenLabel.Content = "Noch vorhandene Teile:" + TeilManager.getInstance().getVerfügbareTeile().Count.ToString();
            tb.Items.Add(nochVorhandenLabel);


            server = new ClientServerCom.Server(this.Dispatcher, grid, PuzzleScanCollection.getInstance());

            TeilManager.getInstance().teileGeändertEvent += ServerWin_teileGeändertEvent;

            server.logEvent += Server_logEvent;
            server.TeilGefundenEvent += Server_PlaziereEvent;

            Server_logEvent(null, null);
            this.Closing += ServerWin_Closing;

        }

        private void ServerWin_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Wirklich beenden?", "Beenden?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }
        void kill()
        {
            if(gws.selection!=null)
            {
                if(gws.selection.Value.Width != 1 && gws.selection.Value.Height != 1)
                {
                    MessageBox.Show("Nur eine Zelle kann man löschen");
                }
                else
                {
                    OpenCvSharp.Point p = new Point(gws.selection.Value.X, gws.selection.Value.Y);
                    if(MessageBox.Show("lösche " + p.ToString(), "Löschen", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        grid.remove(p.X, p.Y);
                        foreach (ClientSS clientSS in server.css)
                        {
                            clientSS.restart();
                            
                        }
                    }
                }
            }
            //PuzzleBauClient sel = ClientSelection.selectClient(bauServer.kunden);
        }
        void save()
        {
            FileStream fs = new FileStream(PathProvider.getGridFile(), FileMode.Create);
            grid.writeToStream(fs);
            fs.Close();
        }
        private void Server_PlaziereEvent(ClientServerCom.ClientSS kunde, int x, int y, OrientiertesTeil ot)
        {
            grid.insert(x, y, ot);
            List<KonkreteBelegung> plaz = new List<KonkreteBelegung>();
            plaz.Add(new KonkreteBelegung(x, y, ot.TeilNummer, ot.Orientierung));
            foreach(ClientSS clientSS in server.css)
            {
                clientSS.sendPlatzierung(plaz);
                //clientSS.plaziere(x, y, ot);
            }
        }

        //private void Server_GridAnforderungEvent(ClientServerCom.ClientSS kunde)
        //{
        //    kunde.sendGrid(grid);
        //}

        private void Server_logEvent(object? sender, string e)
        {
            logList.Items.Clear();
            foreach(String s in server.LogList)
            {
                logList.Items.Add(s);
            }
        }

        private void ServerWin_teileGeändertEvent(object? sender, EventArgs e)
        {
            nochVorhandenLabel.Content = "Noch " + TeilManager.getInstance().getVerfügbareTeile().Count.ToString() + " Teile, verbaut: " + TeilManager.getInstance().getVerbauteTeile().Count.ToString();
        }

        void ip()
        {
            String ips = "IP Addressen\r\n";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ips += ip.ToString() + "\r\n";
                }
            }
            //ips += "\r\n ServerPort: " + NetworkSettingBauServer.ServerPort;
            //ips += "\r\n FilePort: " + NetworkSettingBauServer.FilePort;
            MessageBox.Show(ips);
        }
        void aktVerb()
        {
            //if (gws == null)
            //{
            //    return;
            //}
            //List<int> verb = TeilManager.getInstance().getVerbauteTeile();
            //foreach (PuzzleBauClient client in bauServer.kunden)
            //{
            //    client.neueVerbauteTeile(verb);
            //}
            ////PuzzleBauClient sel = ClientSelection.selectClient(bauServer.kunden);
            ////if (sel != null) sel.neueVerbauteTeile(verb);
        }

        //private void BauServer_gridUpdate(object? sender, Grid e)
        //{
        //    gws.Grid.insertGrid(e);

        //}

        //private void BauServer_log(object? sender, string e)
        //{
        //    this.Dispatcher.Invoke(new Action(() => { logList.Items.Add(e); }));
        //}



        //public void export(Grid g)
        //{
        //    //PuzzleBauClient sel = ClientSelection.selectClient(bauServer.kunden);
        //    //if (sel != null) sel.neuerAuftrag(g);
        //}

    }

}
