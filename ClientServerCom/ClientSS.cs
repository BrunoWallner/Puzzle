using PuzzleBauen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using Grid = PuzzleBauen.Grid;

namespace ClientServerCom
{
    public class ClientSS
    {
        public delegate void teilGefundenDelegate(ClientSS kunde, int x, int y, OrientiertesTeil ot);
        /// <summary>
        /// Der Client hat ein Teil gefunden und möchte, dass der Server es plaziert.
        /// </summary>
        public event teilGefundenDelegate teilGefundenEvent;
      
        //public void plaziere(int x, int y, OrientiertesTeil ot)
        //{
        //    List<byte> data = new List<byte>();
        //    data.Add(2); //Code
        //    data.AddRange(BitConverter.GetBytes((int)1)); //nur ein teil
        //    data.AddRange(BitConverter.GetBytes(x));
        //    data.AddRange(BitConverter.GetBytes(y));
        //    data.AddRange(BitConverter.GetBytes(ot.TeilNummer));
        //    data.AddRange(BitConverter.GetBytes(ot.Orientierung));
        //    client.GetStream().Write(data.ToArray(), 0, data.Count);
        //    onLog("TeilPlazierung gesendet");
        //    return;
        //}
        void onTeilGefunden(int x, int y, OrientiertesTeil ot)
        {
            if(teilGefundenEvent != null)
            {
                teilGefundenEvent(this, x, y, ot);
            }
        }      
        /// <summary>
        /// Schickt dem Client die Belegungen
        /// </summary>
        /// <param name="kob"></param>
        public void sendPlatzierung(List<KonkreteBelegung> kob)
        {
            List<byte> data = new List<byte>();
            data.Add(2); //Code
            data.AddRange(BitConverter.GetBytes((int)kob.Count)); 
            foreach(KonkreteBelegung k in kob)
            {
                data.AddRange(BitConverter.GetBytes(k.x));
                data.AddRange(BitConverter.GetBytes(k.y));
                data.AddRange(BitConverter.GetBytes(k.id));
                data.AddRange(BitConverter.GetBytes(k.orientierung));
            }
            client.GetStream().Write(data.ToArray(), 0, data.Count);
            onLog("TeilPlazierung gesendet");
            return;

        }
        public void restart()
        {
            List<byte> data = new List<byte>();
            data.Add(5); //Code
            client.GetStream().Write(data.ToArray(), 0, data.Count);
            onLog("Restart gesendet");
            return;
        }
        void decodeReadBuffer(byte[] buffer, int count)
        {
            switch (buffer[0])
            {
                case 1://Kunde hat Namen gesendet
                    name = System.Text.Encoding.ASCII.GetString(buffer, 1, count - 1);
                    onLog("Name erhalten");
                    break;
                case 2://Kunde will alle plazierten Teile
                    onLog("Alle Plazierungen angefordert");
                    sendPlatzierung(server.grid.getAlleBelegungen());

                    break;

                case 3://Kunde will Teil plazieren
                    onLog("Teil gefunden");
                    int x = BitConverter.ToInt32(buffer, 1);
                    int y = BitConverter.ToInt32(buffer, 5);
                    int teilID = BitConverter.ToInt32(buffer, 9);
                    int orient = BitConverter.ToInt32(buffer, 13);

                    onTeilGefunden(x,y, new OrientiertesTeil(teilID, orient));
                    break;

                default:
                    onLog("Unbekannter Code");
                    break;
            }
        }

        public event EventHandler<String> logEvent;
        public event EventHandler connectionLostEvent;
        void onLog(string message)
        {
            if (logEvent != null) logEvent(this, message);
        }
        void onConnectionLost()
        {
            if (connectionLostEvent != null) connectionLostEvent(this, null);
        }
        TcpClient client;

        public string Name { get { return name; } }
        String name = "unbekannt";
        Server server;
        public ClientSS(TcpClient client, Server server)
        {
            this.server = server;
            this.client = client;
            startReading();
        }    

        public void disconnect()
        {
            if (client != null) client.Close();
            onLog("Verbindung getrennt");
            onConnectionLost();
        }
        async void startReading()
        {
            byte[] buffer = new byte[1000000 * 10];
            try
            {
                while (true)
                {
                    Task<int> x = client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    await x;
                    int receivedBytes = x.Result;
                    if (!client.Connected) onLog("Nicht mehr verbunden");
                    onLog(receivedBytes.ToString() + " Bytes empfangen");
                    if (receivedBytes > 0)
                    {
                        decodeReadBuffer(buffer, receivedBytes);                      
                    }
                    else
                    {
                        disconnect();
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

