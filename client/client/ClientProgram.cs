using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using WebSocketSharp;
using WebSocketSharp.Server;

using System.Diagnostics;

namespace client
{
    class ClientProgram
    {
        static VirtualPoint vp;
        static MouseGestureRecognition mgr;

        // callback dla zdarzenia odebrania danych od serwera
        static void OnMessage(object sender, MessageEventArgs e)
        {
            // deserializujemy JSONa od serwera 
            InputData data = InputData.Deserialize(e.Data);

            switch (data.type)
            {
                case "touch":
                    Console.Out.WriteLine("touch");
                    MoveCursor(data.x/15, data.y/15);
                    break;
                case "btn":
                    Console.Out.WriteLine("btn");
                    break;
                default:
                    Console.Out.WriteLine("Nierozpoznany typ JSONA");
                    break;
            }
        }

        static void MoveCursor(int x, int y)
        {
            WinCursorClient.POINT p = new WinCursorClient.POINT();

            WinCursorClient.GetCursorPos(ref p);
            p.x += Convert.ToInt16(x);
            p.y += Convert.ToInt16(y);

            WinCursorClient.ClientToScreen(WinCursorClient.GetDesktopWindow(), ref p);
            WinCursorClient.SetCursorPos(p.x, p.y);
        }

        static void Main(string[] args)
        {
            vp = new VirtualPoint();
            mgr = new MouseGestureRecognition();

            using (var ws = new WebSocket("ws://167.114.242.19"))
            {
                // dodanie funkcji callback obsługującej odebranie danych
                ws.OnMessage += OnMessage;

                // połączenie z serwerem
                ws.Connect();

                // timer obsługujący usługę rozpoznawania gestów
                Timer mgrTimer = new Timer();
                mgrTimer.Interval = 50;
                mgrTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    mgr.TimerTick();
                };
                mgrTimer.Enabled = true;

                

                while (true)
                {
                    /*newTicks = timer.ElapsedTicks;
                    deltaTime = (newTicks - prevTicks) * milisecPerTick;
                    prevTicks = newTicks;
                    vp.solve((float)deltaTime);
                    MoveCursor((int)(vp.vel.x * (float)deltaTime), (int)(vp.vel.y * (float)deltaTime));*/
                }
            }
        }
    }
}