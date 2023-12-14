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

using PuzzleBauen;
namespace PuzzleBauenClient
{
    public class AuftragsVerwaltungWin : Window
    {
        public static void Show()
        {
            AuftragsVerwaltungWin awin = new AuftragsVerwaltungWin();
            awin.ShowDialog();
        }
        DockPanel dp = new DockPanel();
        ToolBar tb = new ToolBar();
        ListBox aufträge = new ListBox();
        ListBox log = new ListBox();

        public AuftragsVerwaltungWin()
        {
            this.Title = "AuftragsverweltungWin";
            this.Content = dp;
            dp.Children.Add(tb);
            DockPanel.SetDock(tb, Dock.Top);
            tb.Items.Add(new AutoActionButton("ClearLog", new Action(() => { AuftragsManager.getInstance().logList.Clear(); logEvent(null, null); })));
            dp.Children.Add(aufträge);
            DockPanel.SetDock(aufträge, Dock.Left);
            dp.Children.Add(log);
            DockPanel.SetDock(log, Dock.Right);
            this.MinHeight = 200;
            this.MinWidth = 400;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.Closing += AuftragsVerwaltungWin_Closing;
            AuftragsManager.getInstance().auftragEvent += auftragEvent;
            AuftragsManager.getInstance().logEvent += logEvent;
            auftragEvent(null, null);
            logEvent(null, null);
        }

        private void AuftragsVerwaltungWin_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= AuftragsVerwaltungWin_Closing;
            AuftragsManager.getInstance().auftragEvent -= auftragEvent;
            AuftragsManager.getInstance().logEvent -= logEvent;
        }

        private void auftragEvent(object? sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                aufträge.Items.Clear();
                aufträge.Items.Add("Aufträge");
                foreach (Auftrag a in AuftragsManager.getInstance().aufträge) aufträge.Items.Add(a);
            }));
        }

        private void logEvent(object? sender, string e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                log.Items.Clear();
                log.Items.Add("LogList");
                foreach (String s in AuftragsManager.getInstance().logList) log.Items.Add(s);
            }));
        }
    }
}
