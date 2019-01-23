using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace asterWorker
{
    class AsterWorker
    {
        public AsterWorker()
        {

        }

        public void ReadReceiveData(byte[] data, int size)
        {
            try
            {
                string _resp = Encoding.ASCII.GetString(data, 0, size);
                Console.WriteLine("********************");
                Console.WriteLine(_resp);
                Console.WriteLine("********************");
                string[] sep = { "\r\n\r\n" };
                string[] _sep = { "\r\n" };
                string[] __sep = { ": " };
                string[] _respList = _resp.Split(sep, 5, StringSplitOptions.RemoveEmptyEntries);

                foreach (var asterEvent in _respList)
                {
                    Dictionary<string, string> eventHash = new Dictionary<string, string>();
                    if (asterEvent.Split(sep, 5, StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                    {
                        var missingEvents = asterEvent.Split(sep, 30, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var missingEvent in missingEvents)
                        {
                            FillDictionary(missingEvent);
                        }
                    } else
                    {
                        FillDictionary(asterEvent);
                    }
                }
                _respList = null;
                Console.WriteLine("Worker : worker on work");
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Data);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.TargetSite);
                return;
            }
        }

        public void OnAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine(e.LastOperation);
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    OnAsyncRequest(sender, e);
                    break;
                case SocketAsyncOperation.Send:
                    OnAsyncSend(sender, e);
                    break;
                case SocketAsyncOperation.Connect:
                    OnConnected(sender, e);
                    break;
                default:
                    Console.WriteLine("not send/receive event");
                    return;
            }
            return;
        }


        // on connected to server
        private void OnConnected(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                Console.WriteLine("connected");                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Data);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.TargetSite);
                return;
            }
        }

        // on completed sending request to server
        private void OnAsyncSend(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                Console.WriteLine("request sended");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Data);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.TargetSite);
                return;
            }
        }

        //if completed operation - Receive -> Read received data
        private void OnAsyncRequest(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                AsterWorker worker = new AsterWorker();
                ReadReceiveData(e.Buffer, e.BytesTransferred);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Data);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.TargetSite);
                return;
            }
        }

        private void FillDictionary(string ev)
        {
            Dictionary<string, string> eventHash = new Dictionary<string, string>();
            string[] _sep = { "\r\n" };
            string[] __sep = { ": " };
            var eventFields = ev.Split(_sep, 40, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in eventFields)
            {
                try
                {
                    eventHash.Add(item.Split(__sep, 2, StringSplitOptions.RemoveEmptyEntries).First(), item.Split(__sep, 2, StringSplitOptions.RemoveEmptyEntries).Last());
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(item.Split(__sep, 2, StringSplitOptions.RemoveEmptyEntries).First());
                }
            }
            WriteToFile(eventHash, ev);
        }

        private void WriteToFile(Dictionary<string, string> eventHash, string item)
        {
            FileStream file;
            string[] sepVal = { "." };
            string path;

            if (eventHash.Keys.ToList().IndexOf("Uniqueid") != -1)
            {
                path = "D:\\projects\\asterApp\\" + eventHash["Uniqueid"].Split(sepVal, 2, StringSplitOptions.RemoveEmptyEntries)[0] + ".txt";
            }
            else
            {
                path = "D:\\projects\\asterApp\\trash.txt";
            }
            if (File.Exists(path))
            {
                file = File.Open(path, FileMode.Append);
            }
            else
            {
                file = File.Create(path);
            }
            file.Write(Encoding.ASCII.GetBytes(Convert.ToString(DateTime.Now)), 0, Encoding.ASCII.GetBytes(Convert.ToString(DateTime.Now)).Count());
            file.Write(Encoding.ASCII.GetBytes("\r\n\r\n"), 0, Encoding.ASCII.GetBytes("\r\n\r\n").Count());
            file.Write(Encoding.ASCII.GetBytes(item), 0, Encoding.ASCII.GetBytes(item).Length);
            file.Write(Encoding.ASCII.GetBytes("\r\n\r\n"), 0, Encoding.ASCII.GetBytes("\r\n\r\n").Count());
            file.Flush();
            file.Close();
            file = null;
        }
    }
}