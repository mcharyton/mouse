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
    public partial class MouseGestureRecognition2 : Form
    {
        /* FIELDS */

        // zegar
        //private readonly Timer _timer = new Timer();

        // opóźnienie zegara
        //private int delay;

        // indeks w tablicy 'moves' do którego ma zostać wpisany numer ruchu kursora  
        private int index;

        // tablica wykonanych ruchów
        private int[] moves;

        // lista wczytanych gestów
        private LinkedList<int[]> list;

        // aktualna pozycja kursora na ekranie
        private Point pCurrent;

        // wcześniejsza pozycja kursora na ekranie
        private Point pPrevious;

        // aktualnie wykonany ruch kursorem
        int rc;

        // wcześniejszy wykonany ruch kursorem
        int rp;

        // licznik - wskazuje czas od ostatniego ruchu kursorem
        private int n;

        // zmienna wskazująca czy kursor się porusza
        private bool moving;

        /* CONSTRUCTOR */
        public MouseGestureRecognition2()
        {
           // InitializeComponent();

            // inicjalizacja parametrów programu
            init();
        }

        // inicjalizacja parametrów programu
        private void init()
        {
            //delay = 30;
            index = 0;
            n = 0;
            moving = false;
            moves = new int[10];
            list = new LinkedList<int[]>();
            rp = -1;

            // pobranie aktualnych pozyci kursora na ekranie
            pPrevious = new Point(Cursor.Position.X, Cursor.Position.Y);

            // inicjalizacja tablicy 'moves', -1 -> brak ruchu
            for (int i = 0; i < moves.Length; i++)
            {
                moves[i] = -1;
            }

            // inicjalizacja zegra
           // _timer.Interval = delay;
           // _timer.Tick += TimerTick;
           // _timer.Enabled = true;

            // wczytanie gestów z pliku
            readFromFile("shapes.dat");
            //printListOnConsole();
        }


        /* METHODS */

        // metoda do wyszukiwania gestów z tablicy 'moves'
        private int findPattern()
        {
            // indeks wyszukiwania w tablicy 'moves'
            int start;

            // indeks wyszukiwania w tablicy 'tab'
            int i;

            // dla każdego gestu z listy
            foreach (int[] tab in list)
            {
                start = (moves.Length - tab.Length + 1 + index) % moves.Length;
                for (i = 1; i < tab.Length; i++)
                {
                    // jeśli są różne to przerywany
                    if (tab[i] != moves[start])
                    {
                        break;
                    }

                    // jeśli są zgodne to porównujemy dalej
                    start++;
                    start = start % moves.Length;
                }


                if (i == tab.Length)
                {
                    // jeżeli tu jesteśmy to znaczy że odnaleźliśmy gest
                    // czyścimy tablicę 'moves' i zwracmy odnależiony gest
                    clear();
                    return tab[0];
                }
            }

            // w przypadku braku pozytywnych wyników wyszukiwania zwracamy -1
            return -1;
        }

        // metoda do czyścimy tablicę 'moves'
        private void clear()
        {
            for (int i = 0; i < moves.Length; i++)
                moves[i] = -1;
        }

        // metoda wczytująca do listy gesty z pliku, nazwa pliku podawana jako argument
        private void readFromFile(string fname)
        {
            string line = "";
            string[] p;
            int[] pint;
            using (StreamReader sr = new StreamReader(fname))
            {
                // wczytywanie linia po linie do momentu natrafienia na koniec pliku  
                while ((line = sr.ReadLine()) != null)
                {

                    line = line.Trim();

                    // jeżeli linia zaczyna się od '//' to traktujemy ją jako komentarz - pomijamy 
                    if (!line.StartsWith("//") && !line.Equals(""))
                    {

                        // podział wczytanej linie - separator = ' '
                        p = line.Split(' ');
                        pint = new int[p.Length];
                        for (int i = 0; i < p.Length; i++)
                            pint[i] = int.Parse(p[i]);

                        // dodanie gestu do listy
                        list.AddFirst(pint);
                    }
                }
            }
        }

        // metoda pomocnicza - wyświtla znane gesty do okna konsoli 
        public void printListOnConsole()
        {

            Console.Write("\n\n[ ");
            foreach (int[] tab in list)
            {
                foreach (int i in tab)
                    Console.Write(i + " ");
                Console.WriteLine();
            }
            Console.Write("]\n");
        }


        // metoda przeliczająca pozycje kursora na odpowiedni ruch
        // parametry:
        // Point c - aktualna pozycja kursora na ekranie
        // Point p - wcześniejsza pozycja kursora na ekranie
        // eps - maksymalna odległość do której nie wyznaczamy nowego ruchu
        private int calculate(Point c, Point p, int eps)
        {
            // wyznacznie odległości między punktamu c i p
            int r = (int)distance(c, p);

            // obwód okręgu
            double l = (2 * Math.PI * r) / 8.0;

            // jeśli odległość jest mniejsza to zwracamy -1 -> brak ruchu
            if (r > eps)
            {
                // kierunek w górę
                if (distance(new Point(p.X, p.Y - r), c) < l)
                    return 0;
                // kierunek w prawo
                if (distance(new Point(p.X + r, p.Y), c) < l)
                    return 1;
                // kierunek w dół
                if (distance(new Point(p.X, p.Y + r), c) < l)
                    return 2;
                // kierunek w lewo
                if (distance(new Point(p.X - r, p.Y), c) < l)
                    return 3;
                // brak ruchu
                else
                    return -1;
            }
            else
            {
                return -1;
            }
        }

        // metoda do obliczania odległości między dwoma punktami - odległośc euklidesowa
        private double distance(Point c, Point p)
        {
            double dX = p.X - c.X;
            double dY = p.Y - c.Y;
            double multi = dX * dX + dY * dY;
            double rad = Math.Round(Math.Sqrt(multi), 3);

            return rad;
        }

        // metoda wykonywana w każdej iteracji
        //void TimerTick(object sender, EventArgs e)
        public void TimerTick(int delay)
        {

            // ustalenie aktualnych pozycji kursora
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            pCurrent = new Point(x, y);

            // jeżeli odległość między punktami jest większa od zera to oznacza że kursor się porusza
            if (distance(pCurrent, pPrevious) > 0)
            {
                moving = true;
                n = 0;
            }
            // w przeciwnym razie ustawiamy 'moving' na false i zaczynamy liczyć czas o zakończenia ruchu
            else
            {
                moving = false;
                n++;
            }

            // jeżeli po upłynięciu wyznaczonego czasu, kursor nie został poruszony to czyścimy wyliczone ruchy
            if (100 / delay < n)
            {
                clear();
                n = 0;
            }

            // przeliczenie pozycji kursora na odpowiedni ruch
            rc = calculate(pCurrent, pPrevious, delay);
           
            // jeśli jest to nowy ruch, inny od poprzedniego
            if (rc != -1 && rc != rp)
            {
                
                // wpisujemy go w odpowiednie miejsce w tablicy 'moves'
                rp = rc;
                moves[index] = rc;
                index++;
                index = index % moves.Length;

                // sprawdzenie czy tablica 'moves' zawiera znany nam gest
                int pattern = findPattern();

                // jeśli tak to wykonujemy czynność odpowiednią do tego gestu 
                if (pattern != -1)
                {
                    if (pattern == 0)
                    {
                        // pobieranie ścieżki do folderu - ścieżka jako łańcuch tekstowy
                        string myComputerPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                        //uruchamiamy explorer i wskazujemy na zadaną ścieżkę - "mój komputer" 
                        System.Diagnostics.Process.Start("explorer", myComputerPath);
                    }
                    else if (pattern == 1)
                    {
                        // pobieranie ścieżki do folderu - ścieżka jako łańcuch tekstowy
                        string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                        //uruchamiamy explorer i wskazujemy na zadaną ścieżkę - "moje dokumenty"
                        System.Diagnostics.Process.Start("explorer", myDocumentsPath);
                    }
                    else if (pattern == 2)
                    {
                        // uruchoienie wyszukiwarki google
                        string url = "http://google.com/";
                        System.Diagnostics.Process.Start(url);
                    }
                    else if (pattern == 3)
                    {
                        // uruchomienie panelu sterowania
                        System.Diagnostics.Process.Start("control");
                    }

                }

            }

            pPrevious = pCurrent;


            if (n > 10000)
                n = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
