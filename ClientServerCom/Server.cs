using Microsoft.VisualBasic.Logging;
using PuzzleBauen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Drawing.Imaging;

namespace ClientServerCom
{
    public class Server
    {
        public delegate void TeilGefundenDelegate(ClientSS kunde, int x, int y, OrientiertesTeil ot);
        /// <summary>
        /// Wird Ausgelöst, wenn ein Kunde ein Teil gefunden hat
        /// </summary>
        public event TeilGefundenDelegate TeilGefundenEvent;
        void onTeilGefunden(ClientSS kunde, int x, int y, OrientiertesTeil ot)
        {
            if (TeilGefundenEvent != null)
            {
                dispatcher.Invoke(new Action(() =>
                {
                    TeilGefundenEvent(kunde, x, y, ot);
                }));
                //dispatcher.Invoke(TeilGefundenEvent, kunde, x, y, ot);

            }
        }

        
        public event EventHandler<String> logEvent;
        List<string> logList = new List<string>(); 
        public List<string> LogList { get { return logList; } }
        
        Dispatcher dispatcher;
        public Dispatcher Dispatcher { get {  return dispatcher; } }
        TcpListener bauServerListener = new TcpListener(IPAddress.Any, Ports.serverPort);
        TcpListener scanFileServerListener = new TcpListener(IPAddress.Any, Ports.environmentPort);
        public List<ClientSS> css = new List<ClientSS>();


        public void onLog(string message)
        {
            dispatcher.Invoke(new Action(() =>
            {
                logList.Add(message);
                if (logEvent != null) logEvent(null, message);
            }));                        
        }
        public void onNeuerArbeiter()
        {

        }
        //public void restart()
        //{
        //    List<byte> data = new List<byte>();
        //    data.Add(5); //Code
        //    client.GetStream().Write(data.ToArray(), 0, data.Count);
        //    onLog("Restart gesendet");
        //    return;
        //}
        public Grid grid;
        public PuzzleScanCollection scanCollection;
        public Server(Dispatcher dispatcher, Grid grid, PuzzleScanCollection scanCollection)
        {
            this.grid = grid;
            this.scanCollection = scanCollection;

            this.dispatcher = dispatcher;
            //this is called asynchronously and will run in a different thread
            bauServerListener.Start();
            bauServerListener.BeginAcceptTcpClient(handle_BauServerConnection, bauServerListener);
            scanFileServerListener.Start();
            scanFileServerListener.BeginAcceptTcpClient(handle_scanFileServerConnection, scanFileServerListener);
            onLog("Server started");
        }
        void handle_scanFileServerConnection(IAsyncResult result)
        {
            onLog("scanFile wurde angefordert");
            scanFileServerListener.BeginAcceptTcpClient(handle_scanFileServerConnection, scanFileServerListener);
            TcpClient kunde = scanFileServerListener.EndAcceptTcpClient(result);

            kunde.GetStream().Write(BitConverter.GetBytes(grid.Width));
            kunde.GetStream().Write(BitConverter.GetBytes(grid.Height));
            scanCollection.writeToStream(kunde.GetStream());            

            kunde.Close();
            onLog("FileTransfer Fertig");
        }
        void handle_BauServerConnection(IAsyncResult result)
        {
            bauServerListener.BeginAcceptTcpClient(handle_BauServerConnection, bauServerListener);
            ClientSS neuerKunde = new ClientSS(bauServerListener.EndAcceptTcpClient(result), this);
            initNeuenKunden(neuerKunde);
            onLog("neuer Client");
        } 
        void initNeuenKunden(ClientSS neuerKunde)
        {
            neuerKunde.logEvent += kundenLog;
            neuerKunde.connectionLostEvent += kundenConnectionLost;
            neuerKunde.teilGefundenEvent += onTeilGefunden;
            css.Add(neuerKunde);
        }
        private void kundenConnectionLost(object? sender, EventArgs e)
        {
            ClientSS client = (ClientSS)sender;
            client.logEvent -= kundenLog;
            client.connectionLostEvent -= kundenConnectionLost;
            client.teilGefundenEvent -= onTeilGefunden;


            css.Remove(client);
            onLog("Client " + client.Name + " getrennt");
        }

        private void kundenLog(object? sender, string e)
        {
            ClientSS client = (ClientSS)sender;
            onLog(client.Name + ": " + e);
        }
    }
}
