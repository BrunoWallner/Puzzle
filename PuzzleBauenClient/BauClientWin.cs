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
using Grid = PuzzleBauen.Grid;
using System.IO;
using OpenCvSharp;
using Point = OpenCvSharp.Point;
using ClientServerCom;

namespace PuzzleBauenClient
{
    internal class BauClientWin : Window
    {
        DockPanel dp = new DockPanel();
        ToolBar tb = new ToolBar();
        StackPanel sp = new StackPanel();
        ListBox logList = new ListBox();

        //PuzzleBauClient pc;
        GridPanel gp;// = new GridPanel(null);
        Label TeileLabel = new Label();
        ClientServerCom.ClientCS client;
        public BauClientWin(ClientCS client, Grid g)
        {
            this.client = client;
            gp = new GridPanel(g);
            initClient();

            

            this.Title = "PuzzlebauClient";

            this.Content = dp;
            dp.Children.Add(tb);
            DockPanel.SetDock(tb, Dock.Top);
            dp.Children.Add(sp);
            DockPanel.SetDock(sp, Dock.Left);
            //dp.Children.Add(logList);
            //DockPanel.SetDock(logList, Dock.Left);
            dp.Children.Add(gp);

            tb.Items.Add(new AutoActionButton("IPs", Ports.ShowIPs));
            tb.Items.Add(new AutoActionButton("Aktualisieren", akt));
            tb.Items.Add(TeileLabel);
            TeileLabel.Content = "Noch " + TeilManager.getInstance().getVerfügbareTeile().Count.ToString() + " Teile, Verbaut: " + TeilManager.getInstance().getVerbauteTeile().Count.ToString();
            TeilManager.getInstance().teileGeändertEvent += BauClientWin_teileGeändertEvent;

            tb.Items.Add(new AutoActionButton("AuftragsManager", AuftragsVerwaltungWin.Show));
            gp.InsertRequestEvent += Gp_InsertRequestEvent;
            gp.GridImageDrawEvent += Gp_GridImageDrawEvent;
            AuftragsManager.getInstance().InsertEvent += BauClientWin_InsertEvent;
            
            //CheckBox showAll;
            //showAll = new CheckBox();
            //showAll.Content = "Bild";
            //showAll.IsChecked = true;
            //showAll.Checked += ShowAll_Checked;
            //showAll.Unchecked += ShowAll_Unchecked;
            //sp.Children.Add(showAll);

            this.KeyDown += BauClientWin_KeyDown;

            this.Closing += BauClientWin_Closing;
        }

        //private void ShowAll_Checked(object sender, RoutedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}
        //private void ShowAll_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        private void BauClientWin_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if(MessageBox.Show("Wirklich beenden?", "Beenden?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void BauClientWin_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            gp.keyDown(sender, e);
            //throw new NotImplementedException();
        }

        private void BauClientWin_teileGeändertEvent(object? sender, EventArgs e)
        {
            TeileLabel.Content = "Noch " + TeilManager.getInstance().getVerfügbareTeile().Count.ToString() + " Teile, Verbaut: " + TeilManager.getInstance().getVerbauteTeile().Count.ToString();
        }

        void akt()
        {
            client.allePlaziertenTeileAnfordern();
        }
        private void BauClientWin_InsertEvent(int x, int y, OrientiertesTeil ot)
        {
            client.teilPlazieren(x, y, ot);
            //MessageBox.Show("BauClientInsertEvetn");
        }

        void initClient()
        {
            client.logEvent += Client_logEvent;
            client.restartEvent += Client_restartEvent;
            client.plaziereClientTeilEvent += Client_placedTeilEvent;                
            client.sendName();
            client.allePlaziertenTeileAnfordern();
        }

        private void Client_restartEvent(object? sender, EventArgs e)
        {
            gp.Grid.restartGrid();
            TeilManager.getInstance().reload();
            client.allePlaziertenTeileAnfordern();
            //gp.Grid = new Grid();
            //MessageBox.Show("restart");
        }

        private void Client_placedTeilEvent(List<KonkreteBelegung> bel)
        {
            if (gp.Grid != null)
            {
                foreach(KonkreteBelegung kb  in bel)
                {
                    gp.Grid.insert(kb.x, kb.y, kb.getTeil());
                }                
            }
        }

        //private void Client_placedTeilEvent(int x, int y, OrientiertesTeil ot)
        //{
        //    if (gp.Grid != null)
        //    {
        //        gp.Grid.insert(x, y, ot);
        //    }

        //}

        //private void Client_GridErhaltenEvent(Grid g)
        //{
        //    gp.setGrid(g);
        //}

        private void Client_logEvent(object? sender, string e)
        {
            logList.Items.Clear();
            foreach(String s in client.LogList)
            {
                logList.Items.Add(s);
            }            
        }

        private void Gp_GridImageDrawEvent(OpenCvSharp.Mat m)
        {
            if (gp.Grid == null) return;
            foreach(Auftrag a in AuftragsManager.getInstance().aufträge)
            {
                gp.Grid.gd.markiereZelle(m, a.X, a.Y, Scalar.Green, Scalar.Green);
            }

        }

        private void Gp_InsertRequestEvent(int x, int y, OrientiertesTeil ot)
        {
            if(ot==null || gp.Grid == null)
            {
                MessageBox.Show("Null bei InsertRequest");
                return;
            }            
            AuftragsManager.getInstance().addAuftrag(x, y, ot, gp.Grid);
            gp.Grid.onBelegungChanged();
        }
        //private void Pc_NeueTeileVerbaut(object? sender, List<int> e)
        //{
        //    TeilManager.getInstance().teileVerbaut(e);
        //}
        //void loadScan()
        //{
        //    if(FileTransferClient.getFile(NetworkSettingBauClient.ServerName, NetworkSettingBauClient.FilePort, PathProvider.getScanCollectionFile()))
        //    {
        //        MessageBox.Show("ScanCollection geholt");
        //    }
        //    else
        //    {
        //        MessageBox.Show("ScanCollection Nicht geholt");
        //    }            
        //}

        //void update()
        //{
        //    if(gp.Grid == null)
        //    {
        //        MessageBox.Show("Nix Grid");
        //        return;
        //    }
        //    pc.sendGridUpdate(gp.Grid);
        //}
        private void Pc_NeuerSubGridAuftrag(object? sender, PuzzleBauen.Grid e)
        {
            Dispatcher.BeginInvoke(new Action(()=>         
            {
                gp.setGrid(e);            
            }));
        }

        private void Pc_ClientConnected(object? sender, bool e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (!e) this.Title = "PuzzlebauClient - Nicht Verbunden";
                else this.Title = "PuzzlebauClient - Verbunden";
            }));

        }

        //void connect()
        //{
        //    pc.connect();
        //    this.Title = "connecting";
        //}
        //void disconnect()
        //{
        //    this.Title = "disconnecting";
        //    pc.disconnect();
        //}

        //private void Pc_log(object? sender, string e)
        //{
        //    this.Dispatcher.Invoke(new Action(() => { logList.Items.Add(e); }));
        //}
    }
}


