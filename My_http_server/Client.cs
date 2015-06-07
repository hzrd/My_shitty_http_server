using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace My_http_server
{
    class Client
    {
        public Client(TcpClient client)
        {
            Regex r = new Regex(@"GET\s+/(\S+.\w+(\?\S*)?)?\s+HTTP/\d+.\d+");
            byte[] buffer = new byte[1024];
            client.GetStream().Read(buffer, 0, 1024);
            string request = Encoding.UTF8.GetString(buffer);
            string test = "/"+r.Match(request).Groups[1].Value;
            string[] commands;
            string comms = "";
            string extension;
            if (test.Length == 1)
                test += "index.html";
            if (test.IndexOf('?') != -1)
            {
                comms = test.Remove(0, test.IndexOf('?'));
                test = test.Remove(test.IndexOf('?'));
                extension = test.Remove(0,test.LastIndexOf('.')+1);
            }

            else
            {
            extension = test.Remove(0, test.LastIndexOf('.') + 1);
            }
            Console.WriteLine(request);
            Console.WriteLine(test);
            Console.WriteLine(comms);

            //if (commands!=null)
            //foreach(string s in commands)
            //    Console.WriteLine(s);

            Console.WriteLine(extension);

            string ContentType = string.Empty;
            switch (extension)
            {
                case "htm":
                case "html":
                    ContentType = "text/html";
                    break;
                case "css":
                    ContentType = "text/css";
                    break;
                case "js":
                    ContentType = "text/javascript";
                    break;
                case "jpg":
                    ContentType = "image/jpeg";
                    break;
                case "jpeg":
                case "png":
                case "gif":
                    ContentType = "image/" + extension;
                    break;
                default:
                    if (extension.Length > 1)
                    {
                        ContentType = "application/" + extension;
                    }
                    else
                    {
                        ContentType = "application/unknown";
                    }
                    break;
            }

            string path = "D:/savinkov"+test;
            FileStream fs;

            try
            {
                fs = new FileStream(path, FileMode.Open);
                string ContentLength = fs.Length.ToString();
                string header = "HTTP/1.1 200 OK\nContent-Type: " + ContentType + "\nContent-Length: " + ContentLength + "\n\n";
                buffer = Encoding.ASCII.GetBytes(header);
                client.GetStream().Write(buffer, 0, buffer.Length);
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                client.GetStream().Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            catch
            {

            }
            client.Close();
        }
    }
}
