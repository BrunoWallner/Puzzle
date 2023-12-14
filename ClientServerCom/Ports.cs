using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientServerCom
{
    public class Ports
    {
        public static int serverPort = 21000;
        public static int environmentPort = 22000;
        //public static int gridFilePort = 23000;
        public static void ShowIPs()
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
            MessageBox.Show(ips);
        }

    }
}
