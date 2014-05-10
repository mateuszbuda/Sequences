using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWK
{
    public class Sequence
    {
        public int[] seq;       //ciąg wybranych pozycji na liście
        public int[,] list;    //tablica z listami kolorów
        public int len;        //długość listy
        public int listSize;   //rozmiar list z kolorami
        public int cnt = 0;     //licznik losować w głównej pętli

        public Sequence(int _len, int _listSize = 4)
        {
            len = _len;
            listSize = _listSize < 3 ? 3 : _listSize;
            seq = new int[len];
            list = new int[len, listSize];

        }

        //konstruktor tworzy losowe listy
        //(elementy na liście są w losowej kolejności)
        //z zadanego przedziału
        //więc w taki układzie mógłbym nie losować wybieranego koloru z listy,
        //bo same listy są przemieszane a to, że losuję sprawia, że mam podwójne losowanie
        public Sequence(int _len, int minVal, int maxVal, int _listSize = 4)
        {
            if (maxVal - minVal < listSize - 1)
            {
                minVal = 0;
                maxVal = listSize - 1;
            }

            len = _len;
            listSize = (_listSize < 3 || _listSize > 10) ? 4 : _listSize;
            seq = new int[len];
            list = new int[len, listSize];

            Random r = new Random();

            for (int i = 0; i < len; i++)
            {
                List<int> tmp = new List<int>(listSize);
                //int tt = minVal;
                while (tmp.Count < listSize)
                {
                    int t = r.Next(minVal, maxVal + 1);
                    //t = tt;
                    if (!tmp.Contains(t))
                        tmp.Add(t);
                    //tt++;
                }

                for (int j = 0; j < listSize; j++)
                    list[i, j] = tmp[j];
            }
        }

        //nie ma konstruktora który przyjmowałby gotową tablicę list,
        //bo komu by się chciało pisać tyle list


        //sprawdza czy ciąg ma repetycję na pozycjach < n
        //zwraca rozmiar repetycji
        public int isRepetitive(int n)
        {
            if (n > len)
                throw new ArgumentException();
            //nie jestem pewien czy to dobrze, że rzucam wyjątek i go nigdzie nie łapię...

            int k = n - 1;

            while (k >= 0)
            {
                int h = (n - k + 1) / 2;

                if (this.hasRepetition(k, h))
                    return h;

                k -= 2;
            }

            return 0;
        }

        //sprawdza czy jest repetycja o długości h i początku na indeksie k
        private bool hasRepetition(int k, int h)
        {
            for (int i = k; i < k + h; i++)
                if (list[i, seq[i]] != list[i + h, seq[i + h]])
                    return false;

            return true;
        }

        //wypisuje ciąg wraz z listami w formacie [1 (a, b, c, d)] [2 (d, c, b, a)]
        //gdzie liczba to numer wybranego koloru na liście (numeracja od 0)
        //czyli w tym przykładzie ciąg będzie taki: {a, c}
        public void writeSequence()
        {

            for (int i = 0; i < len; i++)
            {
                Console.Write(string.Format("[{0} ({1}, {2}, {3}", seq[i], list[i, 0], list[i, 1], list[i, 2]));

                for (int j = 3; j < listSize; j++)
                    Console.Write(string.Format(", {0}", list[i, j]));

                Console.Write(")]");
                Console.WriteLine();
            }
        }

        //wypisuje wybrane elementy (nie koloruje bez repetycji)
        public void writeColoring()
        {
            for (int i = 0; i < len; i++)
                Console.Write(string.Format("{0}, ", list[i, seq[i]]));

            Console.WriteLine();
        }

        public void Color()
        {
            Random r = new Random();
            int h = 0;

            int i = 0;
            while (i < seq.GetLength(0))
            {
                seq[i] = r.Next(0, listSize);

                h = isRepetitive(i);

                i = i - h + 1;

                cnt++;
            }
        }

    }
}
