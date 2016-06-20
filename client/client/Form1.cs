using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WebSocketSharp;
using WebSocketSharp.Server;

using System.Runtime.Serialization;

namespace client
{
    public partial class Form1 : Form
    {

        MouseGestureRecognition2 mgr;
        System.Timers.Timer doubleClickTimer;
        System.Timers.Timer mgrTimer;
        System.Timers.Timer loopTimer;
        System.Diagnostics.Stopwatch stopwatchTimer;

        long currTicks = 0;
        long deltaTicks;
        long prevTicks;

        double deltaPosX;
        double deltaPosY;

        double deltaTime;

        int lastPosX = 0;
        int lastPosY = 0;

        double speedX, speedY;
        double posX, posY;
        
        public Form1()
        {
            InitializeComponent();
        }

        double fun(int x)
        {
            double y;
            double dx = (double)x;
            dx /= 25.0;
            y = 10e-4 * Math.Pow(dx, 4) + 0.1 * Math.Pow(dx, 2);
            if (x < 0)
                y = -y;
            return y;
        }



        // callback dla zdarzenia odebrania danych od serwera
        void OnMessage(object sender, MessageEventArgs e)
        {
            System.Console.Out.WriteLine(e.Data);
            labelStatus.Text = e.Data;
            // deserializujemy JSONa od serwera
            try
            {
                InputData data = InputData.Deserialize(e.Data);

                switch (data.type)
                {
                    case "touch":
                        //Console.Out.WriteLine("touch");
                        speedX = fun(data.x) * 200;
                        speedY = fun(data.y) * 200;
                        //MoveCursor(data.x/2 , data.y /2);
                        break;
                    case "joystick":
                        //Console.Out.WriteLine("touch");
                        speedX = fun(data.x);
                        speedY = fun(data.y);
                        break;

                    case "btn":
                        //Console.Out.WriteLine("btn");
                        TriggerClick(data.btn);
                        break;

                    case "id":
                        /// TODO
                        this.labelId.Text = data.x.ToString();
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

        void OnClose(object sender, CloseEventArgs e)
        {
            System.Console.Out.WriteLine("Connection closed.");
            labelStatus.Text = "Zerwano połączenie.";
            labelId.Text = "";
            ((WebSocket)sender).ConnectAsync();
        }

        void OnError(object sender, ErrorEventArgs e)
        {
            System.Console.Out.WriteLine("Connection error.");
            labelStatus.Text = "Błąd połączenia.";
        }

        void OnOpen(object sender, EventArgs e)
        {
            System.Console.Out.WriteLine("Connection established.");
            labelStatus.Text = "Poprawnie połączono.";
            this.Update();
        }



        void MoveCursor(int x, int y)
        {
            WinCursorClient.POINT p = new WinCursorClient.POINT();

            WinCursorClient.GetCursorPos(ref p);
            p.x += Convert.ToInt16(x);
            p.y += Convert.ToInt16(y);

            WinCursorClient.ClientToScreen(WinCursorClient.GetDesktopWindow(), ref p);
            WinCursorClient.SetCursorPos(p.x, p.y);
        }

        void MoveCursorAbsolute(int x, int y)
        {
            WinCursorClient.POINT p = new WinCursorClient.POINT();

            p.x = Convert.ToInt16(x);
            p.y = Convert.ToInt16(y);

            WinCursorClient.ClientToScreen(WinCursorClient.GetDesktopWindow(), ref p);
            WinCursorClient.SetCursorPos(p.x, p.y);
        }

        void TriggerClick(string type)
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

        void EstablishConnection(WebSocket ws)
        {
            ws.ConnectAsync();
        }

        private void Form1_Load_1(object sendero, EventArgs eo)
        {
            mgr = new MouseGestureRecognition2();
            doubleClickTimer = new System.Timers.Timer();
            loopTimer = new System.Timers.Timer();
            stopwatchTimer = new System.Diagnostics.Stopwatch();

            System.Console.Out.WriteLine("Started program.");

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
                posX = (double)cursorPos.x;
                posY = (double)cursorPos.y;

                // połączenie z serwerem
                ws.ConnectAsync();

                // timer obsługujący podwójne kliknięcie
                doubleClickTimer = new System.Timers.Timer();
                doubleClickTimer.AutoReset = false;
                doubleClickTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                {
                    WinCursorClient.POINT p = new WinCursorClient.POINT();
                    WinCursorClient.GetCursorPos(ref p);
                    WinCursorClient.mouse_event(WinCursorClient.MOUSEEVENTF_LEFTDOWN | WinCursorClient.MOUSEEVENTF_LEFTUP, (uint)p.x, (uint)p.y, 0, (System.UIntPtr)0);
                };

                // timer obsługujący usługę rozpoznawania gestów
                mgrTimer = new System.Timers.Timer();
                int delay = 2;
                mgrTimer.Interval = delay;
                mgrTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                {
                    mgr.TimerTick(delay);
                };
                // mgrTimer.Enabled = true;

                loopTimer.Interval = 30;
                loopTimer.Elapsed += LoopTimerTick;
                loopTimer.Start();
                stopwatchTimer.Start();
                prevTicks = stopwatchTimer.ElapsedTicks;
            }
        }

        private void LoopTimerTick(object sendert, EventArgs et)
        {
            currTicks = stopwatchTimer.ElapsedTicks;
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                mgrTimer.Start();
            else
                mgrTimer.Stop();
        }
    }
}
