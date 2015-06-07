using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace My_http_server
{
    class SslServer
    {
        public SslServer(string certificate)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 443);
            listener.Start();

            while(true)
            {
                new SslClient(listener.AcceptTcpClient(), certificate);
            }
        }
    }
}
