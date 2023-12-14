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

        static int coo = 0;
        public void erteileAuftrag()
        {
            if (aufträge.Count>0)
            {
                onLog("Erteile richtigen Auftrag");
                Auftrag a = aufträge[0];
                a.getScanBild().ImWrite(PathProvider.getWWWPath() + "\\bild1" + coo.ToString() + ".png");
                a.getPuzzleBild().ImWrite(PathProvider.getWWWPath() + "\\bild2" + coo.ToString() + ".png");
                listener.SendAll("load_image:1:/bild1" + coo.ToString() + ".png");
                listener.SendAll("load_image:2:/bild2" + coo.ToString() + ".png");
                listener.SendAll("send_meta:" + a.X.ToString() + ":" + a.Y.ToString() + ":" + a.Teil.TeilNummer.ToString() + ":" + a.Teil.PuzzleTeil.scan.id);
            }
            else
            {
                onLog("Erteile \"Nichts zu tun\" Auftrag");
                Mat m = new Mat(100, 100, MatType.CV_8UC3, Scalar.Black);
                m.PutText("nix", new OpenCvSharp.Point(0, 50), HersheyFonts.Italic, 2, Scalar.Red);
                m.ImWrite(PathProvider.getWWWPath() + "\\bild1" + coo.ToString() + ".png");
                m.ImWrite(PathProvider.getWWWPath() + "\\bild2" + coo.ToString() + ".png");
                listener.SendAll("load_image:1:/bild1" + coo.ToString() + ".png");
                listener.SendAll("load_image:2:/bild2" + coo.ToString() + ".png");
                listener.SendAll("send_meta: -1:-1:-1:-1");
            }
            coo++;
        }
        Auftrag getErstenPassendenAuftrag(int x, int y, int TeilId)
        {
            Auftrag ret = null;
            foreach (Auftrag a in aufträge)
            {
                if ((a.X == x && a.Y == y) || TeilId == a.Teil.TeilNummer)
                {
                    ret = a;
                    break;
                }
            }
            return ret;
        }
        public void addAuftrag(int x, int y, OrientiertesTeil teil, Grid grid)
        {
            bool autoInsert = false;
            Auftrag neu = new Auftrag(x, y, teil, grid);
            aufträge.Add(neu);
            if (autoInsert)
            {
                successCallback(x, y, teil.TeilNummer);

            }
            erteileAuftrag();
            onAuftragChanged();
        }
        void loginCallback()
        {
            erteileAuftrag();
        }
        void successKill(Auftrag auftrag)
        {
            //Das Teil hat an die Position gepasst. Also können alle Aufträge an die Position
            //und auch alle Aufträge mit diesem Teil gelöscht werden.
            List<Auftrag> killList = new List<Auftrag>();
            foreach (Auftrag a in aufträge)
            {
                if ((a.X == auftrag.X && a.Y == auftrag.Y) || auftrag.Teil.TeilNummer == a.Teil.TeilNummer)
                {
                    killList.Add(a);
                }
            }
            foreach (Auftrag a in killList)
            {
                aufträge.Remove(a);
            }
        }
        void failedKill(Auftrag auftrag)
        {
            //Das Teil hat nicht an die Position gepasst. Also können alle Identischen Aufträge gelöscht werden.
            List<Auftrag> killList = new List<Auftrag>();
            foreach (Auftrag a in aufträge)
            {
                if (a.X == auftrag.X && a.Y == auftrag.Y && auftrag.Teil.TeilNummer == a.Teil.TeilNummer)
                {
                    killList.Add(a);
                }
            }
            foreach (Auftrag a in killList)
            {
                aufträge.Remove(a);
            }
        }
        void successCallback(int x, int y, int TeilID)
        {
            Auftrag a = getErstenPassendenAuftrag(x,y,TeilID);         
            if(a!=null)
            {
                //a.grid.insert(a.X, a.Y, a.Teil);                             
                onInsertEvent(x, y, a.Teil);
                successKill(a);
                //onLog("Teil eingefügt");
            }
            else
            {
                onLog("SuccessCallback: Auftrag gibts nicht");
            }
            erteileAuftrag();
            onAuftragChanged();            
        }
        void failCallback(int x, int y, int TeilID)
        {
            Auftrag a = getErstenPassendenAuftrag(x, y, TeilID);
            if(a!=null)
            {
                failedKill(a);
            }
            else
            {
                onLog("FailCallback: Auftrag gibts nicht");
            }
            erteileAuftrag();
            onAuftragChanged();
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
            string request = tokens[1];
            string output = "";
            switch (request)
            {
                case "login":
                    onLog("wsHook-login, sockets " + listener.sockets.Count.ToString());
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        loginCallback(); 
                    }));                    
                    break;
                case "request_image":
                    onLog("wsHook-imageRequest");
                    output += "load_image:";
                    string file_name = "/example.png";
                    output += file_name;
                    break;
                case "success":
                    onLog("wsHook-success");
                    {
                        int x = int.Parse(tokens[2]);
                        int y = int.Parse(tokens[3]);
                        int teilID = int.Parse(tokens[4]);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            successCallback(x, y, teilID);
                        }));
                    }
                    break;
                case "fail":
                    onLog("wsHook-fail");
                    {
                        int x = int.Parse(tokens[2]);
                        int y = int.Parse(tokens[3]);
                        int teilID = int.Parse(tokens[4]);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            failCallback(x, y, teilID);
                        }));                        
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
