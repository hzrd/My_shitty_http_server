using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace My_http_server
{
    class Program
    {
        static void Main(string[] args)
        {
            //int MaxThreadsCount = Environment.ProcessorCount * 4;
            //ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            //ThreadPool.SetMinThreads(2, 2);
            //new Server(800);
            new SslServer("SslServer.cer");
        }
    }
}
