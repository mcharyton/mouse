using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    public partial class MouseGestureRecognition : Form
    {
        /* FIELDS */
        //private readonly Timer _timer = new Timer();
        //private int delay;

        private int index;
        private int movesIndex;
        private int[] stack;
        private int[] red;
        private int[] moves;
        private LinkedList<int[]> list;

        private Point pCurrent;
        private Point pPrevious;
        int rc, reducedCurrent, reducedPrevious;

        /* CONSTRUCTOR */
        public MouseGestureRecognition()
        {
            //InitializeComponent();

            init();
        }

        private void init() {
            //delay = 50;
            index = 0;
            movesIndex = 0;
            stack = new int[4];
            red = new int[9];
            moves = new int[10];
            list = new LinkedList<int[]>();
            reducedPrevious = -1;
            pPrevious = new Point(Cursor.Position.X, Cursor.Position.Y);

            for (int i = 0; i < stack.Length; i++)
            {
                stack[i] = -1;
            }

            for (int i = 0; i < moves.Length; i++)
            {
                moves[i] = -1;
            }
            //_timer.Interval = delay;
            //_timer.Tick += TimerTick;
            //_timer.Enabled = true;

            readFromFile("shapes.dat");
            //printListOnConsole();
        }


        /* METHODS */
        private int findPattern()
        {
            int start;
            int i;

            foreach (int[] tab in list)
            {
                start = (moves.Length - tab.Length + 1 + movesIndex) % moves.Length;
                for (i = 1; i < tab.Length; i++)
                {
                    if(tab[i] != moves[start])
                    {
                        break;
                    }

                    start++;
                    start = start % moves.Length;
                }

                if (i == tab.Length)
                {
                    clear();
                    return tab[0];
                }
            }

            return -1;
        }

        private void clear()
        {
            for (int i = 0; i < moves.Length; i++)
                moves[i] = -1;
        }

        private void readFromFile(string fname)
        {
            string line = "";
            string[] p;
            int[] pint;
            using (StreamReader sr = new StreamReader(fname))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (!line.StartsWith("//") && !line.Equals(""))
                    {
                        p = line.Split(' ');
                        pint = new int[p.Length];
                        for (int i = 0; i < p.Length; i++)
                            pint[i] = int.Parse(p[i]);
                        list.AddFirst(pint);
                    }
                }
            }
        }

        public void printListOnConsole() {

            Console.Write("\n\n[ ");
            foreach (int[] tab in list)
            {
                foreach (int i in tab)
                    Console.Write(i + " ");
                Console.WriteLine();
            }
            Console.Write("]\n");
        }

        private int calculate(Point c, Point p, int eps)
        {
            // 0 -> right; 1 -> right up; 2 -> right down
            // -1 NONE; 3 -> up; 4 -> down;
            // 5 -> left; 6 -> left up; 7 -> left down
            double diffX = c.X - p.X;
            double diffY = c.Y - p.Y;
            double absX = Math.Abs(diffX);
            double absY = Math.Abs(diffY);

            if (absX < 2 && absY < 2)
                return -1;
            else if (diffX > 0 && absY < eps)
            {
                return 0;
            }
            else if (diffX > 0 && diffY < 0)
            {
                return 1;
            }
            else if (diffX > 0 && diffY > 0)
            {
                return 2;
            }
            else if (absX < eps && diffY < 0)
            {
                return 3;
            }
            else if (absX < eps && diffY > 0)
            {
                return 4;
            }
            else if (diffX < 0 && absY < eps)
            {
                return 5;
            }
            else if (diffX < 0 && diffY < 0)
            {
                return 6;
            }
            else
            {
                return 7;
            }

        }

        private int reduce(int p)
        {
            stack[index] = p;
            index++;
            index = index % stack.Length;

            for (int i = 0; i < stack.Length; i++)
            {
                red[stack[i] + 1]++;
            }

            int maxIndex = 0;
            int maxVal = red[0];
            red[0] = 0;

            for (int i = 1; i < 9; i++)
            {
                if (red[i] > maxVal)
                {
                    maxVal = red[i];
                    maxIndex = i;
                }
                red[i] = 0;
            }

            if (maxVal < 3)
                maxIndex = -1;
            else
                maxIndex = maxIndex - 1;

            return maxIndex;
        }

        //void TimerTick(object sender, EventArgs e)
        public void TimerTick()
        {
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            //Console.Out.WriteLine(""+ x + " " + y);
            pCurrent = new Point(x, y);
            //label1.Text = "x: " + x + " y: " + y;

            rc = calculate(pCurrent, pPrevious, 10);

            if (rc != -1)
            {
                reducedCurrent = reduce(rc);

                if (reducedCurrent != -1 && reducedCurrent != reducedPrevious)
                {
                    reducedPrevious = reducedCurrent;
                    moves[movesIndex] = reducedCurrent;
                    movesIndex++;
                    movesIndex = movesIndex % moves.Length;
                    //label2.Text = "Znak: " + reducedCurrent;
                    Console.Out.WriteLine(reducedCurrent);
                    int pattern = findPattern();

                    if (pattern != -1)
                    {
                        if (pattern == 0)
                        {
                            //get the folder paths
                            string myComputerPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                            //open explorer and point it at the paths
                            System.Diagnostics.Process.Start("explorer", myComputerPath);
                        }
                        else if (pattern == 1)
                        {
                            //get the folder paths
                            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            //open explorer and point it at the paths
                            System.Diagnostics.Process.Start("explorer", myDocumentsPath);
                        }
                        else if (pattern == 2)
                        {
                            string url = "http://google.com/";
                            System.Diagnostics.Process.Start(url); 
                        }
                        else if (pattern == 3)
                        {
                            System.Diagnostics.Process.Start("control");
                        }

                    }
                    
                }
            }

            pPrevious = pCurrent;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
