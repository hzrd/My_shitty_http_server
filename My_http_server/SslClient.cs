using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.IO;
using System.Text.RegularExpressions;

namespace My_http_server
{
    class SslClient
    {
        public SslClient(TcpClient client, string certificate)
        {
            //Handshake
            X509Certificate cert = new X509Certificate(certificate);
            SslStream sslstream = new SslStream(client.GetStream(), false); //опасносте
            try
            {
                sslstream.AuthenticateAsServer(cert, false, SslProtocols.Default, true);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //sslstream.ReadTimeout = 2000;
            //sslstream.WriteTimeout = 2000;

            //Получаем запрос и все необходимые данные из него
            Regex r = new Regex(@"(GET|POST)\s+/(\S+.\w+(\?\S*)?)?\s+HTTP/\d+.\d+");
            string request = ReadMessage(sslstream);
            //если запрос пустой - закрываем

            if (request == "")
            {
                sslstream.Close();
                client.Close();
            }

            //получаем путь требуемого файла
            else
            {
                string type = r.Match(request).Groups[1].Value;
                string test = "/" + r.Match(request).Groups[2].Value;
                string[] parameters = null;
                string extension;
                if (test.Length == 1)
                    test += "index.html";
                if (test.IndexOf('?') != -1)
                {
                    parameters = test.Remove(0, test.IndexOf('?')+1).Split(new char[] {'&','='}); //параметры
                    test = test.Remove(test.IndexOf('?'));                                        //путь к файлу
                    extension = test.Remove(0, test.LastIndexOf('.') + 1);                        //расирение
                }

                else
                {
                    extension = test.Remove(0, test.LastIndexOf('.') + 1);
                }

                if (test.IndexOf("..")!=-1)
                {
                    SendError(sslstream, 400);
                    client.Close();
                }

                //выводи некоторой инфы
                Console.WriteLine(request);
                Console.WriteLine();
                Console.WriteLine("File: " + test);
                if (parameters!=null)
                {
                    Console.WriteLine("Parameters: ");
                    for (int i=0; i<parameters.Length; i++)
                    {
                        parameters[i] = Uri.UnescapeDataString(parameters[i]);
                        Console.WriteLine(parameters[i]);
                    }
                }
                Console.WriteLine();



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

                string path = "D:/savinkov" + test;

                //если файла не существует - отправляем ошибку
                if (!File.Exists(path))
                {
                    SendError(sslstream, 404);
                    client.Close();
                    return;
                }

                if (ContentType == "text/html")
                {
                    StreamReader sr = new StreamReader(path);
                    path = "D:/savinkov/temp.html";
                    string str = sr.ReadToEnd();
                    sr.Close();
                    if (parameters !=null)
                    for (int i = 0; i < parameters.Length-1; i++)
                        if ((parameters[i] == "denis") && (parameters[i + 1] != ""))
                            str = str.Replace("те", "ДЕНИС " + parameters[i + 1]+"!!!");
                    ProcessLalka(ref str);
                    StreamWriter sw = new StreamWriter(path);
                    sw.Write(str);
                    sw.Close();
                }



                FileStream fs;
                byte[] buffer = new byte[1024];
                try
                {
                    fs = new FileStream(path, FileMode.Open);
                    string ContentLength = fs.Length.ToString();
                    string header = "HTTP/1.1 200 OK\nContent-Type: " + ContentType + "\nContent-Length: " + ContentLength + "\n\n";
                    buffer = Encoding.UTF8.GetBytes(header);
                    sslstream.Write(buffer, 0, buffer.Length);
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                    sslstream.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                sslstream.Close();
                client.Close();

                if (File.Exists("D:/savinkov/temp.html"))
                    File.Delete("D:/savinkov/temp.html");
            }

        }


        //читаем сообщение клиента
        static string ReadMessage(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            try
            {
                while (true) 
                {
                    int bytes = -1;
                    bytes = sslStream.Read(buffer, 0, buffer.Length);
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);
                    if (messageData.ToString().IndexOf("\r\n\r\n") != -1)
                    {
                        break;
                    }
                    if (bytes==0)
                    {
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return messageData.ToString();
        }

        //отправка ошибки
        private void SendError(SslStream sslStream, int Code)
        {
            string CodeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            string Html = "<html><body><h1>" + CodeStr + "</h1></body></html>";
            string Str = "HTTP/1.1 " + CodeStr + "\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            sslStream.Write(Buffer, 0, Buffer.Length);
        }

        private void ProcessLalka(ref string html)
        {
            
            Regex tag = new Regex(@"<lalka>([а-яА-Я ]+)</lalka>");
            foreach(Match c in tag.Matches(html))
            {
                string a = c.Value;
                string s = c.Groups[1].Value;
                string res = "<div class = \"left_contacts\"><p>" +s+ "</p></a></div>";
                html = html.Replace(a, res);
            }
        }
    }
}
