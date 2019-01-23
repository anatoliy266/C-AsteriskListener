using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

using asterWorker;

namespace asterSocket
{
    class AsterSocket : Socket
    {
        public AsterSocket(AddressFamily address = AddressFamily.InterNetwork, SocketType type = SocketType.Stream, ProtocolType protocol = ProtocolType.Tcp) : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            
        }
        public async void Init()
        {
            try
            {
                string host = "ami.ats.550550.ru";
                int port = 5038;
                byte[] request = Encoding.ASCII.GetBytes("Action: Login\r\nActionID: 0\r\nUsername: reader\r\nSecret: /*SECRET PASSWORD :)*/\r\nEvents: On\r\n\r\n");
                SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs();
                asyncEventArgs.RemoteEndPoint = new DnsEndPoint(host, port);
                asyncEventArgs.SetBuffer(new byte[8192], 0, 8192);
                Connect(asyncEventArgs.RemoteEndPoint);
                
                Send(request);
                asyncEventArgs.Completed += OnCompleted;
                
                await Task.Run(() => ReceiveAsync(asyncEventArgs));
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
        private async void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            AsterWorker worker = new AsterWorker();
            await Task.Run(() => worker.OnAsyncCompleted(this, e));
            await Task.Run(() => ReceiveAsync(e));
        }
    }
}
