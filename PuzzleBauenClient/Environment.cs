using ClientServerCom;
using PuzzleBauen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleBauenClient
{
    public class Environment
    {      

        private Environment(PuzzleScanCollection scan, OpenCvSharp.Point size)
        {
            this.size = size;
            this.scanCollection = scan;
        }
        public PuzzleScanCollection ScanCollection
        {
            get { return scanCollection; }
        }
        public OpenCvSharp.Point Size
        {
            get { return size; }
        }
        PuzzleScanCollection scanCollection;
        OpenCvSharp.Point size;
        public static async Task<Environment> vonServerHolen(string serverName)
        {
            try
            {
                TcpClient localkunde = new TcpClient();// new TcpClient(serverName, Ports.scanFilePort);
                await localkunde.ConnectAsync(serverName, Ports.environmentPort);

                MemoryStream ms = new MemoryStream();
                
                await localkunde.GetStream().CopyToAsync(ms);
                ms.Position = 0;
                byte[] intBuf = new byte[4];
                

                ms.Read(intBuf, 0, 4);
                int x = BitConverter.ToInt32(intBuf, 0);
                

                ms.Read(intBuf, 0, 4);
                int y = BitConverter.ToInt32(intBuf, 0);
                
                PuzzleScanCollection ret = PuzzleScanCollection.load(ms);
                localkunde.Close();
                
                return new Environment(ret, new OpenCvSharp.Point(x, y)); ;
            }
            catch (Exception e)
            {
                //              onLog("ScanCollection Exception");
                return null;
            }
        }
    }
}
