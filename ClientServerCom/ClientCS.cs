using PuzzleBauen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ClientServerCom
{
    public class ClientCS
    {
        public delegate void plaziereClientTeilDelegate(List<KonkreteBelegung> bel);
        /// <summary>
        /// Wird ausgelöst, wenn der Server Teile im Client plazieren will. 
        /// (zum Beispiel weil einer der Clients ein Teil gefunden hat und diese dem Server mitgeteilt hat)
        /// </summary>
        public event plaziereClientTeilDelegate plaziereClientTeilEvent;
        public event EventHandler restartEvent;
        public event EventHandler connectionChangedEvent;
        public event EventHandler<String> logEvent;
        public void onLog(string message)
        {
            dispatcher.Invoke(new Action(() =>
            {
                logList.Add(message);
                if (logEvent != null) logEvent(null, message);
            }));
        }
        void onRestart()
        {
            if (restartEvent != null)
            {
                onLog("RestartDelegate");
                dispatcher.Invoke(new Action(() =>
                {
                    restartEvent(null, null);
                    //plaziereClientTeilEvent(bel);
                }));
            }
        }
        void onPlaziereClientTeil(List<KonkreteBelegung> bel)
        {
            onLog("placve");
            if (plaziereClientTeilEvent != null)
            {
                onLog("placeDelegate");
                dispatcher.Invoke(new Action(() =>
                {
                    plaziereClientTeilEvent(bel);
                }));
            }

        }
        /// <summary>
        /// Es wird dem Server mitgeteilt, dass an die Position jenes Teil passt
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ot"></param>
        public void teilPlazieren(int x, int y, OrientiertesTeil ot)
        {
            List<byte> sendlist = new List<byte>();
            sendlist.Add(3);
            sendlist.AddRange(BitConverter.GetBytes(x));
            sendlist.AddRange(BitConverter.GetBytes(y));
            sendlist.AddRange(BitConverter.GetBytes(ot.TeilNummer));
            sendlist.AddRange(BitConverter.GetBytes(ot.Orientierung));
            client.GetStream().Write(sendlist.ToArray(), 0, sendlist.Count);
        }

        /// <summary>
        /// Fordert den Server auf, die bekannten Teilplazierungen zu senden.
        /// Das ist nur beim Start nötig, da es sonst automtisch geschen sollte.
        /// </summary>
        public void allePlaziertenTeileAnfordern()
        {
            List<byte> sendlist = new List<byte>();
            sendlist.Add(2);
            client.GetStream().Write(sendlist.ToArray(), 0, sendlist.Count);
        }

        void decodeReadBuffer(byte[] buffer, int count)
        {
            switch (buffer[0])
            {
                case 2: //Teil plaziert
                    onLog("Server plaziert Teile");
                    int zahl = BitConverter.ToInt32(buffer, 1);
                    List<KonkreteBelegung> kob = new List<KonkreteBelegung>();
                    int pos = 5;
                    for (int i = 0; i < zahl; i++)
                    {
                        int x = BitConverter.ToInt32(buffer, pos);
                        pos += 4;
                        int y = BitConverter.ToInt32(buffer, pos);
                        pos += 4;
                        int teilID = BitConverter.ToInt32(buffer, pos);
                        pos += 4;
                        int orient = BitConverter.ToInt32(buffer, pos);
                        pos += 4;
                        kob.Add(new KonkreteBelegung(x, y, teilID, orient));
                    }
                    onPlaziereClientTeil(kob);
                    break;
                case 5:
                    onLog("restart");
                    onRestart();
                    break;

                default:
                    onLog("unbekannter Code");
                    break;
            }
        }

        void onConnectionChanged()
        {
            dispatcher.Invoke(new Action(() =>
            {
                if (connectionChangedEvent != null) connectionChangedEvent(null, null);
            }));
        }


        List<string> logList = new List<string>();

        public List<string> LogList { get { return logList; } }
        public bool isConnected
        {
            get
            {
                if (client != null && client.Connected) return true;
                return false; ;
            }
        }

        TcpClient client;
        Dispatcher dispatcher;
        string name;
        string serverName = "127.0.0.0";
        public ClientCS(string name, string serverName, Dispatcher dispatcher)
        {
            this.name = name;
            this.dispatcher = dispatcher;
            this.serverName = serverName;
        }



        public async Task<bool> connect()
        {
            if (client != null)
            {
                onLog("Bereits verbunden");
                return true;
            }
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverName, Ports.serverPort);
                read();
                onLog("Verbunden");
                onConnectionChanged();
                return true;
            }
            catch (Exception e)
            {
                disconnect();
                onLog("Error beim verbinden");
            }
            return false;
        }
        public void disconnect()
        {
            if (client != null) client.Close();
            client = null;
            onLog("Disconnected");
            onConnectionChanged();
        }

        public void sendName()
        {
            List<byte> sendlist = new List<byte>();
            sendlist.Add(1);
            sendlist.AddRange(System.Text.Encoding.ASCII.GetBytes(name));
            client.GetStream().Write(sendlist.ToArray(), 0, sendlist.Count);
            return;
        }

        public async void read()
        {
            byte[] buffer = new byte[1000000 * 10];
            try
            {
                while (true)
                {
                    Task<int> x = client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    await x;
                    int bytes = x.Result;
                    onLog("Empfangene Bytes " + bytes.ToString());
                    if (bytes > 0)
                    {
                        decodeReadBuffer(buffer, bytes);
                    }
                    else
                    {
                        onLog("Null bytes Read");
                    }
                }
            }
            catch (Exception e)
            {
                onLog("ReadException");
                disconnect();
            }

        }
    }
}
