using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.Serialization;

using WebSocketSharp;
using WebSocketSharp.Server;

using System.Diagnostics;

namespace client
{
    class ClientProgram
    {
        static MouseGestureRecognition mgr;
        static Timer doubleClickTimer;

        static double speedX, speedY;
        static double posX, posY;

        

        // callback dla zdarzenia odebrania danych od serwera
        static void OnMessage(object sender, MessageEventArgs e)
        {
            System.Console.Out.WriteLine(e.Data);
            // deserializujemy JSONa od serwera
            try
            {
                InputData data = InputData.Deserialize(e.Data);

                switch (data.type)
                {
                    case "touch":
                        //Console.Out.WriteLine("touch");
                        speedX = data.x;
                        speedY = data.y;
                        //MoveCursor(data.x/2 , data.y /2);
                        break;
                    case "joystick":
                        //Console.Out.WriteLine("touch");
                        speedX = data.x/1000.0;
                        speedY = data.y/1000.0;
                        //MoveCursor(data.x/2 , data.y /2);
                        break;

                    case "btn":
                        //Console.Out.WriteLine("btn");
                        TriggerClick(data.btn);
                        break;

                    case "id":
                        /// TODO
                        /// // zaimplementować parowanie komputer-telefon
                        break;

                    default:
                        Console.Out.WriteLine("Nierozpoznany typ JSONA");
                        break;
                }
            }
            catch (SerializationException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        static void OnClose(object sender, CloseEventArgs e)
        {
            System.Console.Out.WriteLine("Connection closed.");
            ((WebSocket)sender).ConnectAsync();

            /*bool notConnected = true;

            while (notConnected)
            {
                
            }*/
        }

        static void OnError(object sender, ErrorEventArgs e)
        {
            System.Console.Out.WriteLine("Connection error.");
        }

        static void OnOpen(object sender, EventArgs e)
        {
            System.Console.Out.WriteLine("Connection established.");
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

        static void MoveCursorAbsolute(int x, int y)
        {
            WinCursorClient.POINT p = new WinCursorClient.POINT();

            p.x = Convert.ToInt16(x);
            p.y = Convert.ToInt16(y);

            WinCursorClient.ClientToScreen(WinCursorClient.GetDesktopWindow(), ref p);
            WinCursorClient.SetCursorPos(p.x, p.y);
        }

        static void TriggerClick(string type)
        {
            WinCursorClient.POINT p = new WinCursorClient.POINT();
            WinCursorClient.GetCursorPos(ref p);

            switch (type)
            {
                case "left":
                    WinCursorClient.mouse_event(WinCursorClient.MOUSEEVENTF_LEFTDOWN | WinCursorClient.MOUSEEVENTF_LEFTUP, (uint)p.x, (uint)p.y, 0, (System.UIntPtr)0);
                    break;
                case "right":
                    WinCursorClient.mouse_event(WinCursorClient.MOUSEEVENTF_RIGHTDOWN | WinCursorClient.MOUSEEVENTF_RIGHTUP, (uint)p.x, (uint)p.y, 0, (System.UIntPtr)0);
                    break;
                case "doubleLeft":
                    WinCursorClient.mouse_event(WinCursorClient.MOUSEEVENTF_LEFTDOWN | WinCursorClient.MOUSEEVENTF_LEFTUP, (uint)p.x, (uint)p.y, 0, (System.UIntPtr)0);
                    doubleClickTimer.Start();
                    break;
            }
        }

        static void Main(string[] args)
        {
            //vp = new VirtualPoint();
            mgr = new MouseGestureRecognition();
            doubleClickTimer = new Timer();

            using (var ws = new WebSocket("ws://167.114.242.19/"))
            {
                // dodanie funkcji callback
                ws.OnMessage += OnMessage;
                ws.OnClose += OnClose;
                ws.OnError += OnError;
                ws.OnOpen += OnOpen;

                // pobranie pozycji myszy
                WinCursorClient.POINT cursorPos = new WinCursorClient.POINT();
                WinCursorClient.GetCursorPos(ref cursorPos);
                posX = (double) cursorPos.x;
                posY = (double) cursorPos.y;

                // połączenie z serwerem
                ws.ConnectAsync();

                // timer obsługujący podwójne kliknięcie
                doubleClickTimer = new Timer();
                doubleClickTimer.AutoReset = false;
                doubleClickTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    WinCursorClient.POINT p = new WinCursorClient.POINT();
                    WinCursorClient.GetCursorPos(ref p);
                    WinCursorClient.mouse_event(WinCursorClient.MOUSEEVENTF_LEFTDOWN | WinCursorClient.MOUSEEVENTF_LEFTUP, (uint)p.x, (uint)p.y, 0, (System.UIntPtr)0);
                };

                // timer obsługujący usługę rozpoznawania gestów
                Timer mgrTimer = new Timer();
                mgrTimer.Interval = 50;
                mgrTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    mgr.TimerTick();
                };
                mgrTimer.Enabled = true;

                Stopwatch timer = new Stopwatch();
                timer.Start();
                long prevTicks = timer.ElapsedTicks;
                long currTicks = 0;
                long deltaTicks;

                double deltaPosX;
                double deltaPosY;

                double deltaTime;

                int lastPosX = 0;
                int lastPosY = 0;

                while (true)
                {
                    currTicks = timer.ElapsedTicks;
                    deltaTicks = currTicks - prevTicks;

                    deltaTime = (double)deltaTicks / (double)TimeSpan.TicksPerSecond / 5000000.0f;

                    deltaPosX = deltaTime * speedX;
                    deltaPosY = deltaTime * speedY;

                    posX += deltaPosX;
                    posY += deltaPosY;

                    if (posX < 0) posX = 0;
                    else if (posX > 2000) posX = 2000;

                    if (posY < 0) posY = 0;
                    else if (posY > 2000) posY = 2000;

                    if ((int)posX != lastPosX || (int)posY != lastPosY)
                    {
                        MoveCursorAbsolute((int)posX, (int)posY);
                        lastPosX = (int)posX;
                        lastPosY = (int)posY;
                    }
                    
                }
            }
        }
    }
}