using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;
using System.Timers;

namespace server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            var wssv = new WebSocketServer ("ws://localhost");
            wssv.AddWebSocketService<PositionBehaviour> ("/recv");
            wssv.Start ();
            Console.Out.WriteLine("Uruchomiono serwer.");

            while (wssv.IsListening)
            {

            }

            wssv.Stop ();

        }
    }
}