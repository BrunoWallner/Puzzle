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
using Point = System.Windows.Point;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Collections;

#pragma warning disable
using PuzzleBauen;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Threading;

namespace ScannenServer
{
    public class ScanCollectionCreator:Window
    {       
        DockPanel dp = new DockPanel();
        StackPanel sp = new StackPanel();
        NamedListBox scans = new NamedListBox("Scans");
        NamedListBox teile = new NamedListBox("Teile");
        Image image = new Image();
        public PuzzleScanCollection target;
        AutoActionButton scanButton;
        string scanFile;
        public ScanCollectionCreator(String scanFile)
        {
            this.scanFile = scanFile;
            this.Content = dp;
            this.Title = "ScanCollectionCreator";//, Servername: \"" + System.Net.Dns.GetHostName() + "\"";

            dp.Children.Add(sp);
            DockPanel.SetDock(sp, Dock.Left);
            //dp.Children.Add(logList);
            //DockPanel.SetDock(logList, Dock.Right);
            //logList.Items.Add("LogList");
            scanButton = new AutoActionButton("Scannen", startScan);
            scanButton.FontSize = 20;
            sp.Children.Add(scanButton);
            //sp.Children.Add(new AutoActionButton("getIP", ip));
            AutoActionButton saveButton = new AutoActionButton("Save", save);
            saveButton.FontSize = 20;
            sp.Children.Add(saveButton);
            AutoActionButton loadButton = new AutoActionButton("load", load);
            loadButton.FontSize = 20;
            sp.Children.Add(loadButton);
            //sp.Children.Add(new AutoActionButton("Neu", neu));


            AutoActionButton del = new AutoActionButton("Delete", dewl);
            del.FontSize = 20;
            sp.Children.Add(del);
            AutoActionButton findEckButton = new AutoActionButton("FindEck", findEck);
            findEckButton.FontSize = 20;
            sp.Children.Add(findEckButton);
            dp.Children.Add(scans);
            DockPanel.SetDock(scans, Dock.Left);
            dp.Children.Add(teile);
            DockPanel.SetDock(teile, Dock.Left);
            dp.Children.Add(image);

            target = new PuzzleScanCollection();
            scans.list.SelectionChanged += List_SelectionChanged;
            teile.list.SelectionChanged += List_SelectionChanged1;

            //server.log += Server_log;
            //server.neuerScan += Server_neuerScan;
            //server.start();
            this.Closing += ScanCollectionCreator_Closing;
        }
        void dewl()
        {
            PuzzleScan pus = (scans.list.SelectedItem as PuzzleScan);
            if(pus == null)
            {
                MessageBox.Show("nix gewählt");
                return;
            }
            if(MessageBox.Show("Wirklich", "wirklich", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                target.removeScan(pus);
                refresh();
            }

        }
        void startScan()
        {
            Thread th = new Thread(scanThreadStart);
            scanButton.IsEnabled = false;
            this.Title = "ScanCollectionCreator - Scannt gerade";
            th.Start(this);
        }
        void ScanFertig(BlackWhite e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                scanButton.IsEnabled = true;
                this.Title = "ScanCollectionCreator";
                if (e != null)
                {
                    PuzzleScan ps = PuTeColWin.Show(e);
                    if (ps != null)
                    {
                        int scanId = target.addScan(ps);
                        MessageBox.Show("Scan " + scanId.ToString() + " hinzugefügt");                        
                        refresh();
                    }
                }
            }));        
        }
        //int index = 1;
 
        void scanThreadStart(object s)
        {
            Dispatcher dis = (s as ScanCollectionCreator).Dispatcher;
            BlackWhite bw = Scannen.scan(dis);
            //index++;
            ScanFertig(bw);
        }
        private void ScanCollectionCreator_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Sind alle Ecken zugewiesen?\r\nIst gespeichert?\r\nWirklich beenden?", "Wirklich", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }


        void findEck()
        {
            List<PuzzleTeil> teileOhneEck = new List<PuzzleTeil>();
            foreach(PuzzleTeil t in target.getTeile())
            {
                if (!t.isEckenOK())
                {
                    t.setEcken(EckpunkFindAlg.getEckenIndizes(t.getContour()));
                    if(!t.isEckenOK()) teileOhneEck.Add(t);
                }
            }
            //this.Dispatcher.Invoke(new Action(() => { new EckenFindWindow(teileOhneEck).ShowDialog(); })); 
            new EckenFindWindow(teileOhneEck).ShowDialog();
        }
        //void neu()
        //{
        //    if (MessageBox.Show("Neue Scancollection anlegen?", "Wirklich", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //    {
        //        target = new PuzzleScanCollection();
        //        refresh();
        //    }
        //}
        void load()
        {
            if (File.Exists(PathProvider.getScanCollectionFile()))
            {
                if (MessageBox.Show("Scancollection laden?", "Wirklich", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //string filename = PathProvider.getScanCollectionFile();
                    FileStream fs = new FileStream(scanFile, FileMode.Open);
                    target = PuzzleScanCollection.load(fs);
                    fs.Close();
                    refresh();
                }
            }
            else
            {
                MessageBox.Show("gibt keine Datei");
            }
        }
        void save()
        {
            if (MessageBox.Show("Scancollection speichern?", "Wirklich", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string filename = scanFile;
                    //PathProvider.getScanCollectionFile();
                FileStream fs = new FileStream(filename, FileMode.Create);
                target.writeToStream(fs);
                fs.Close();
            }
        }
        private void List_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            PuzzleTeil teil = teile.list.SelectedItem as PuzzleTeil;
            if(teil != null)
            {
                image.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(teil.draw());
            }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PuzzleScan selectedScan = scans.list.SelectedItem as PuzzleScan;
            teile.list.Items.Clear();
            if(selectedScan != null)
            {
                this.Title = "Scan Number " + selectedScan.id.ToString();
                foreach (PuzzleTeil p in selectedScan.teile)
                {
                    teile.list.Items.Add(p);
                    //if(!p.isEckenOK()) p.setEcken(EckpunkFindAlg.getEckenIndizes(p.getContour()));
                    //if(!p.isEckenOK()) teile.list.Items.Add(p);
                }
            }

        }

        void refresh()
        {
            //object obj = scans.list.SelectedValue;
            scans.list.Items.Clear();
            teile.list.Items.Clear();
            foreach (PuzzleScan scan in target.getScans())
            {
                scans.list.Items.Add(scan);
            }
        }
    }
}
