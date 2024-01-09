using System;
using System.Diagnostics;

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
using System.Threading;
using System.Windows.Threading;

using PuzzleBauen;
using Grid = PuzzleBauen.Grid;
using System.Runtime.CompilerServices;

namespace PuzzleBauenClient
{
    public class AuftragsManager
    {
        public delegate void InsertDelegate(int x, int y, OrientiertesTeil ot);
        public event InsertDelegate InsertEvent;
        static AuftragsManager instance;
        public event EventHandler<String> logEvent;
        public event EventHandler auftragEvent;        
        public List<Auftrag> aufträge = new List<Auftrag>();
        public List<(int, int, OrientiertesTeil, string)> TeilQueue = new List<(int, int, OrientiertesTeil, string)>();
        string loggedInUser = "";


        void onInsertEvent(int x, int y, OrientiertesTeil ot)
        {
            if(InsertEvent !=null)
            {
                Application.Current.Dispatcher.Invoke(InsertEvent, x, y, ot);
            }

        }
        public static AuftragsManager getInstance()
        {
            if(instance == null)
            {
                instance = new AuftragsManager();
            }
            return instance;
        }
        WebSocketListener listener;
        public List<string> logList = new List<string>();


        void onLog(String log)
        {
            logList.Add(log);
            logEvent?.Invoke(this, log);
        }
        void onAuftragChanged()
        {
            auftragEvent?.Invoke(null, null);
        }
        private AuftragsManager()
        {
            listener = new WebSocketListener(WsHook);
            listener.Start();
            onLog("Auftragsmanager gestartet");
        }

        //static int coo = 0;
        public void erteileAuftrag()
        {
            if (aufträge.Count>0)
            {

                string uuid = getImageUuid();
                onLog("erteile Auftrag");

                Auftrag auftrag = aufträge[0];
                this.TeilQueue.Add((auftrag.X, auftrag.Y, auftrag.Teil, uuid));
                aufträge.RemoveAt(0);

                auftrag.getScanBild().ImWrite(PathProvider.getWWWPath() + "\\images\\" + uuid + "_0" + ".png");
                auftrag.getPuzzleBild().ImWrite(PathProvider.getWWWPath() + "\\images\\" + uuid + "_1" + ".png");
                //listener.SendAll("send_meta:" + auftrag.X.ToString() + ":" + auftrag.Y.ToString() + ":" + auftrag.Teil.TeilNummer.ToString() + ":" + auftrag.Teil.PuzzleTeil.scan.id);
                listener.SendAll("assign:" + uuid + ":" + auftrag.Teil.PuzzleTeil.scan.id);
            }

        }

        string getImageUuid()
        {
            Guid uuid = Guid.NewGuid();
            return uuid.ToString();
        }


        public void addAuftrag(int x, int y, OrientiertesTeil teil, Grid grid)
        {
            Auftrag neu = new Auftrag(x, y, teil, grid);
            aufträge.Add(neu);

            erteileAuftrag();
            onAuftragChanged();
        }

        void successCallback()
        {
            if (this.TeilQueue.Count == 0) { return; }
            (int, int, OrientiertesTeil, string) queue = this.TeilQueue.First();
            this.TeilQueue.RemoveAt(0);
            onInsertEvent(queue.Item1, queue.Item2, queue.Item3);

            List<string> files = new List<string>();
            files.Add(PathProvider.getWWWPath() + "\\images\\" + queue.Item4 + "_0" + ".png");
            files.Add(PathProvider.getWWWPath() + "\\images\\" + queue.Item4 + "_1" + ".png");
            foreach (string path in files)
            {
                File.Delete(path);
            }
        }

        void failCallback()
        {
            if (this.TeilQueue.Count == 0) { return; }
            (int, int, OrientiertesTeil, string) queue = this.TeilQueue.First();
            this.TeilQueue.RemoveAt(0);

            List<string> files = new List<string>();
            files.Add(PathProvider.getWWWPath() + "\\images\\" + queue.Item4 + "_0" + ".png");
            files.Add(PathProvider.getWWWPath() + "\\images\\" + queue.Item4 + "_1" + ".png");
            foreach (string path in files)
            {
                File.Delete(path);
            }
        }

        string WsHook(string msg)
        {
            string[] tokens = msg.Split(':');
            if (tokens.Length < 2)
            {
                onLog("wsHook error:InvalidInput " + msg);
                return "error:InvalidInput";
            }
            string uid = tokens[0];
            bool valid_user = uid == this.loggedInUser;
            string request = tokens[1];
            string output = "";

            // check if user is still reachable
            if (!valid_user)
            {
                if (!listener.stillConnected(this.loggedInUser))
                {
                    this.loggedInUser = "";
                    this.aufträge.Clear();
                    this.TeilQueue.Clear();
                }
            }

            switch (request)
            {
                case "logoff":
                    if (valid_user)
                    {
                        this.loggedInUser = "";
                    }
                    break;
                case "login":
                    if (this.loggedInUser == "")
                    {
                        onLog("wsHook-login, sockets " + listener.sockets.Count.ToString());
                        this.loggedInUser = uid;
                    } else
                    {
                        output += "invalid_user";
                    }                
                    break;
                case "success":
                    if (!valid_user)
                    {
                        output += "invalid_user";
                        break;
                    }
                    onLog("wsHook-success");
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            successCallback();
                        }));
                    }
                    break;
                case "fail":
                    if (!valid_user)
                    {
                        output += "invalid_user";
                        break;
                    }
                    onLog("wsHook-fail");
                    {
                        failCallback();
                    }
                    break;

                default:
                    onLog("wsHook-unbekannt: " + msg);
                    break;
            }


            return output;
        }
    } 
}
