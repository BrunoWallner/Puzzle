using System;
using Fleck;
using System.Threading.Channels;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Windows;
using OpenCvSharp;
using System.IO;
using System.Windows.Threading;
using System.Windows.Interop;

using PuzzleBauen;
using System.Linq;

namespace PuzzleBauenClient
{
    public class WebSocketListener
    {
        public delegate string Hook(string msg);
        public Hook hosok;
        public ChannelWriter<string> writer;
        ChannelReader<string> reader;
        public Dictionary<string, Fleck.IWebSocketConnection> sockets = new Dictionary<string, Fleck.IWebSocketConnection>();

        string ip;
        WebSocketServer listener;

        public WebSocketListener(Hook hook)
        {
            this.hosok = hook;
            this.ip = "ws://0.0.0.0:8001/";
            this.listener = new WebSocketServer(this.ip);
            this.listener.RestartAfterListenError = true;

            FleckLog.LogAction = (_, _, _) => { };

            var channel = Channel.CreateUnbounded<string>();
            this.writer = channel.Writer;
            this.reader = channel.Reader;
        }

        public void Start()
        {
            this.listener.Start(socket =>
            {
                socket.OnMessage = message =>
                {
                    string msg = message; // make extra sure its a string

                    string[] tokens = msg.Split(':');
                    if (tokens.Length < 2) { return; }

                    string uid = tokens[0];
                    string request = tokens[1];

                    switch (request)
                    {
                        case "login":
                            this.sockets[uid] = socket;
                            break;
                        default:
                            break;
                    }
                    string response = this.hosok(msg);
                    if (!String.IsNullOrEmpty(response))
                    {
                        socket.Send(response);
                    }
                };
                socket.OnClose = () =>
                {
                    string uidToRemove = null;
                    foreach (KeyValuePair<string, Fleck.IWebSocketConnection> kvp in sockets)
                    {
                        if (kvp.Value == socket) {
                            uidToRemove = kvp.Key;
                        }
                    }
                    if (uidToRemove != null)
                    {
                        sockets.Remove(uidToRemove);
                    }
                };
            });
        }
        public void SendAll(string msg)
        {
            foreach (var (uid, socket) in this.sockets)
            {
                if (socket != null)
                {
                    socket.Send(msg);
                }
            }
        }

        public bool stillConnected(string uid)
        {
            return this.sockets.ContainsKey(uid);
        }
    }
}
