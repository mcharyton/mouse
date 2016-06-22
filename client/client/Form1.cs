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

        WebSocket wsHandle;

        long currTicks = 0;
        long deltaTicks;
        long prevTicks;

        double deltaPosX;
        double deltaPosY;

        double deltaTime;

        int lastPosX = 0;
        int lastPosY = 0;

        int connStatus;

        int nextConnectionAttemptCounter;

        double speedX, speedY;
        double posX, posY;
        
        public Form1()
        {
            InitializeComponent();

            nextConnectionAttemptCounter = 0;
            connStatus = 0;
        }



        
        double fun(int x, int cx)
        {
            double y;
            double dx = (double)x;
            dx /= (double)cx;
            y = 10e-4 * Math.Pow(dx, 4) + 0.1 * Math.Pow(dx, 2);
            if (x < 0)
                y = -y;
            return y;
        }



        // callback dla zdarzenia odebrania danych od serwera
        void OnMessage(object sender, MessageEventArgs e)
        {
            System.Console.Out.WriteLine(e.Data);
            labelData.Text = e.Data;
            // deserializujemy JSONa od serwera
            try
            {
                InputData data = InputData.Deserialize(e.Data);

                switch (data.type)
                {
                    case "touch":
                        //Console.Out.WriteLine("touch");
                        speedX = fun(data.x,1) * 2000;
                        speedY = fun(data.y,1) * 2000;
                        break;
                    case "joystick":
                        //Console.Out.WriteLine("touch");
                        //speedX = fun(data.x,10) * 1500;
                        //speedY = fun(data.y,10) * 1500;
                        speedX = data.x*100000;
                        speedY = data.y*100000;
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

        // callback dla zamknięcia połączenia WebSocket
        void OnClose(object sender, CloseEventArgs e)
        {
            System.Console.Out.WriteLine("Connection closed.");
            if (connStatus == 1)
            {
                labelStatus.Text = "Zerwano połączenie.";
                labelId.Text = "(brak)";
                connStatus = 0;
            }
            else
            {
                labelStatus.Text = "próbuje nawiązać połączenie...";
            }
            //((WebSocket)sender).ConnectAsync();
            EstablishConnection();
        }

        // callback dla błędu połączenia WebSocket
        void OnError(object sender, ErrorEventArgs e)
        {
            System.Console.Out.WriteLine("Connection error.");
            labelStatus.Text = "Błąd połączenia.";
        }

        // callback dla otwarcia połączenia WebSocket
        void OnOpen(object sender, EventArgs e)
        {
            connStatus = 1;
            System.Console.Out.WriteLine("Connection established.");
            labelStatus.Text = "Poprawnie połączono.";
            this.Update();
        }

        void EstablishConnectionTick(object sender, EventArgs e)
        {
            System.Console.Out.WriteLine("nextAttemptCounter: " + nextConnectionAttemptCounter);
            if( wsHandle.IsAlive){
                System.Console.Out.WriteLine("Connection is alive!");
                return;
            }

            if (nextConnectionAttemptCounter == 0)
            {
                wsHandle.Connect();
                if (wsHandle.IsAlive)
                {
                    ((System.Timers.Timer)sender).Enabled = false;
                }
                nextConnectionAttemptCounter = 3;
            }
            else
            {
                nextConnectionAttemptCounter--;
                labelStatus.Text = "Następna próba połączenia za " +
                    nextConnectionAttemptCounter+1 + " sekund";
                //System.Console.Out.WriteLine("nextConnCounter:"+nextConnectionAttemptCounter);
                //System.Console.Out.WriteLine("Decrementing nextConnCounter...");
                //System.Console.Out.WriteLine("nextConnCounter:" + nextConnectionAttemptCounter);
                
            }
        }

        void EstablishConnection()
        {
            wsHandle.ConnectAsync();
            /*if (wsHandle.IsAlive)
                return;

            System.Timers.Timer connectionTimer = new System.Timers.Timer();
            connectionTimer.Interval = 1000;
            connectionTimer.Elapsed += EstablishConnectionTick;
            connectionTimer.Enabled = true;

            while (!wsHandle.IsAlive)
            {
            }*/
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

        private void LoopTimerTick(object sendert, EventArgs et)
        {
            currTicks = stopwatchTimer.ElapsedTicks;
            deltaTicks = currTicks - prevTicks;

            deltaTime = (double)deltaTicks / (double)TimeSpan.TicksPerSecond / 5000000.0f;

            deltaPosX = deltaTime * speedX;
            deltaPosY = deltaTime * speedY;

            posX += deltaPosX;
            posY += deltaPosY;

            nextConnectionAttemptCounter = 0;

            if (posX < 0) posX = 0;
            else if (posX > 2000) posX = 2000;

            if (posY < 0) posY = 0;
            else if (posY > 2000) posY = 2000;

            if ((int)posX != lastPosX || (int)posY != lastPosY)
            {
                MoveCursor((int)deltaPosX, (int)deltaPosY);
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

        private void Form1_Load(object senderl, EventArgs el)
        {
            mgr = new MouseGestureRecognition2();
            doubleClickTimer = new System.Timers.Timer();
            loopTimer = new System.Timers.Timer();
            stopwatchTimer = new System.Diagnostics.Stopwatch();

            System.Console.Out.WriteLine("Started program.");

            wsHandle = new WebSocket("ws://167.114.242.19/");

            // dodanie funkcji callback
            wsHandle.OnMessage += OnMessage;
            wsHandle.OnClose += OnClose;
            wsHandle.OnError += OnError;
            wsHandle.OnOpen += OnOpen;

            // pobranie pozycji myszy
            WinCursorClient.POINT cursorPos = new WinCursorClient.POINT();
            WinCursorClient.GetCursorPos(ref cursorPos);
            posX = (double)cursorPos.x;
            posY = (double)cursorPos.y;

            // połączenie z serwerem
            EstablishConnection();

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

            // timer obsługujący wyliczanie prędkości
            loopTimer.Interval = 30;
            loopTimer.Elapsed += LoopTimerTick;
            loopTimer.Start();
            stopwatchTimer.Start();
            prevTicks = stopwatchTimer.ElapsedTicks;
        }

        private void oProgramieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = "Politechnika Warszawska\nWydział Elektryczny\n"+
                "Projekt zespołowy 2015/2016\n\nOpiekun:\ndr inż. Tomasz Leś\n\n" +
                "Autorzy:\n"+
                "Michał Charyton\nDominik Jakubiak\nMarcin Konczewski\n" +
                "Paweł Mac\nJan Orliński";
            string caption = "O programie";
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}
