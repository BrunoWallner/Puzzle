//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Xml.Linq;

//using System.Windows.Controls;
//using PuzzleBauen;
//using Image = System.Windows.Controls.Image;
//using System.Windows.Threading;

//namespace ScannenServer
//{
//    internal class ScanServer
//    {
//        public event EventHandler<BlackWhite> neuerScan;
//        public event EventHandler<String> log;

//        static ScanServer instance = null;
//        public static ScanServer getInstance()
//        {
//            if (instance == null)
//            {
//                instance = new ScanServer();
//            }
//            return instance;
//        }


        
//        void onLog(String msg)
//        {
//            log?.Invoke(this, msg);
//        }
        

//        TcpListener server = new TcpListener(IPAddress.Any, 21100);

//        //byte[] buffer = new byte[1000000 * 10];
//        ScanServer()
//        {
//            //server.Start();
//            //onLog("ScanServer start");
//            //accept_connection();
//        }
//        public void start()
//        {
//            try
//            {
//                server.Start();
//                onLog("ScanServer start");
                
//                accept_connection();
//            }
//            catch (Exception ex)
//            {
//                onLog("probleme beim serverstart");
//            }
//        }
//        private void accept_connection()
//        {
//            onLog("Warte auf Client");
//            //this is called asynchronously and will run in a different thread
//            server.BeginAcceptTcpClient(handle_connection, server);            
//        }
//        private async void handle_connection(IAsyncResult result)  //the parameter is a delegate, used to communicate between threads
//        {
//            onLog("neue verbindung");
//            try
//            {
//                TcpClient client = server.EndAcceptTcpClient(result);
//                NetworkStream stream = client.GetStream();
//                MemoryStream ms = new MemoryStream();
//                stream.CopyTo(ms);
//                onLog("Scan erhalten");
//                ms.Position = 0;                
//                BlackWhite res = BlackWhite.loadFromStream(ms);
//                neuerScan?.Invoke(null, res);                
//            }
//            catch (Exception ex)
//            {
//                onLog("Catch: " + ex.Message);
//            }
//            accept_connection();
//        }
//    }
//}
