using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using asterSocket;


namespace generalWorker
{
    class GeneralWorker
    {
        ~GeneralWorker()
        {

        }
        public GeneralWorker()
        {
            try
            {
                AsterSocket socket = new AsterSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Init();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Data);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.TargetSite);
                return;
            }
        }
    }
}
