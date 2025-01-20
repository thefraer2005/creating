
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UnoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer server = new GameServer();
            server.Start();
        }
    }
}
