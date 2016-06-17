using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using WebSocketSharp;
using WebSocketSharp.Server;

using common;

namespace server
{
    class PositionBehaviour : WebSocketBehavior
    {
        Timer timer;
        int x, y;
        WinCursorServer.POINT p;

        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data;
            Console.Out.WriteLine("client says: " + msg);

            MouseData md = new MouseData(100, 200);
            //Send(md.Serialize());
            Send("{\"type\":\"acc\", \"y\":600}");
        }

        protected override void OnOpen()
        {
            Console.Out.WriteLine("Client connected.");
            timer = new Timer(10);
            p = new WinCursorServer.POINT();

            timer.Elapsed += (sender, e) =>
            {
                WinCursorServer.GetCursorPos(ref p);
                p.x = x; p.y = y;
                Send("{\"type\":\"touch\", \"x\":" + (p.x-x) + ", \"y\":" + (p.y-y) + " }");
                Console.Out.WriteLine("Message sent.");
                x = p.x;
                y = p.y;
            };
            timer.Start();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.Out.WriteLine("Client disconnected.");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.Out.WriteLine("Client error.");
        }
    }
}
