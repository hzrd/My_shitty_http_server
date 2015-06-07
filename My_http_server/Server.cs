using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace My_http_server
{
    class Server
    {
        public Server(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any,800);
            listener.Start();
            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), listener.AcceptTcpClient());
            }
        }

        ~Server()
        {
        }

        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient)StateInfo);
        }
    }
}
