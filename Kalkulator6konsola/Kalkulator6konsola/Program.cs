using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kalkulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stack<string> odwrotnosc;
            string wyrazeniestring = "", historia = "", wynik = "", zmienne = "";
            string[] opcje;
            do
            {
                int choice = 1;
                if (wyrazeniestring == "" || zmienne == "")
                { opcje = new string[] { "Podaj wyrażenie do policzenia", "Nadpisz zmienne", "Pomoc", "Historia" }; }

                else
                {
                    opcje = new string[] { "Podaj wyrażenie do policzenia", "Nadpisz zmienne dla " + wyrazeniestring, "Pomoc", "Historia" };
                }
                Console.Clear();
                Console.Write("Kalkulator do obliczeń arytmetycznych i algebraicznych.");
                Menu("Menu główne.", opcje, ref choice, wyrazeniestring);
                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        wyrazeniestring = Console.ReadLine();
                        wyrazeniestring = wyrazeniestring.Replace(" ", "");
                        //sprawdza czy nie ma błędnych znaków w podanym wyrażeniu ->działa póki co
                        for (int i = 0; i < wyrazeniestring.Length; i++)
                        {
                            if (char.IsLetter(wyrazeniestring[i]) == false && char.IsNumber(wyrazeniestring[i]) == false && !(wyrazeniestring[i] == '+' || wyrazeniestring[i] == '-' || wyrazeniestring[i] == '*' || wyrazeniestring[i] == '/' || wyrazeniestring[i] == '^' || wyrazeniestring[i] == '(' || wyrazeniestring[i] == ')' || wyrazeniestring[i] == ',' || wyrazeniestring[i] == '.'))
                            {
                                wyrazeniestring = "";
                                break;
                            }
                            if (i > 0 && char.IsLetter(wyrazeniestring[i]) == true && char.IsLetter(wyrazeniestring[i - 1]) == true)
                            {
                                wyrazeniestring = "";
                                break;
                            }
                        }
                        //sprawdza czy nie ma błędnej składni w podanym wyrażeniu ->chyba działa(testować)
                        if (wyrazeniestring == ""||wyrazeniestring == ",," || wyrazeniestring.Contains("++") || wyrazeniestring.Contains("+/") || wyrazeniestring.Contains("()") || wyrazeniestring.Contains("+*") || wyrazeniestring.Contains("+^") || wyrazeniestring.Contains("-+") || wyrazeniestring.Contains("---") || wyrazeniestring.Contains("-*") || wyrazeniestring.Contains("-/") || wyrazeniestring.Contains("-^") || wyrazeniestring.Contains("*+") || wyrazeniestring.Contains("*/") || wyrazeniestring.Contains("*^") || wyrazeniestring.Contains("**") || wyrazeniestring.Contains("/+") || wyrazeniestring.Contains("//") || wyrazeniestring.Contains("/*") || wyrazeniestring.Contains("/^") || wyrazeniestring.Contains("^+") || wyrazeniestring.Contains("^/") || wyrazeniestring.Contains("^*") || wyrazeniestring.Contains("^^"))
                        {
                            Console.WriteLine("Błędnie podane wyrażenie.\nSpróbuj ponownie.");
                            wyrazeniestring = "";
                            Console.ReadKey();
                            break;
                        }
                        odwrotnosc = ONP(wyrazeniestring, ref zmienne);
                        wynik = Liczenie(odwrotnosc).ToString();
                        if (zmienne != "") { historia = wyrazeniestring + " = " + wynik + " dla" + zmienne + "\n" + historia; }
                        else { historia = wyrazeniestring + " = " + wynik + "\n" + historia; }
                        zmienne = "";
                        Console.WriteLine("Wynik tego wyrażenia to {0}", wynik);
                        Console.ReadKey();
                        break;
                    case 2:
                        Console.Clear();
                        if (wyrazeniestring == "")
                        {
                            wyrazeniestring = Console.ReadLine();
                        }
                        odwrotnosc = ONP(wyrazeniestring, ref zmienne);
                        wynik = Liczenie(odwrotnosc).ToString();
                        if (zmienne != "") { historia = wyrazeniestring + " = " + wynik + " dla " + zmienne + "\n" + historia; }
                        else { historia = wyrazeniestring + " = " + wynik + "\n" + historia; }
                        zmienne = "";
                        Console.WriteLine("Wynik tego wyrażenia to {0}", wynik);
                        Console.ReadKey();
                        break;
                    case 3:
                        Console.WriteLine("+ <- znak dodawania\n- <-znak odejmowania\n* <- znak mnożenia\n/ <- znak dzielenia\n^ <- znak potęgowania");
                        Console.ReadKey();
                        break;
                    case 4:
                        Console.WriteLine("Historia działań:\n" + historia);
                        Console.ReadKey();
                        break;
                }


            } while (true);
        }
        private static void TestowanieONP(Stack<string> odwrotnosc)
        {
            //test czy działa
            while (odwrotnosc.Count > 0)
            {
                Console.Write(odwrotnosc.Pop() + " ");
            }
        }
        static Stack<string> ONP(string wyrazeniestring, ref string zmienne)
        {
            bool correctData;
            string bufor, wyrazeniePoKonwersji;
            float zamiana;
            wyrazeniePoKonwersji = wyrazeniestring;

            for (int i = 0; wyrazeniestring.Length > i; i++)
            {
                //szuka zmiennych(liter) w wyrażeniu i podaje w konsoli prośbę o podanie liczby do podstawienia.Zamienia wszystkie te same litery na jedną podaną zmienną
                if (char.IsLetter(wyrazeniestring[i]) && wyrazeniePoKonwersji.Contains(wyrazeniestring[i]))
                {
                    Console.Write("Wpisz wartość zmiennej {0}=", wyrazeniestring[i]);
                    do
                    {
                        correctData = float.TryParse(Console.ReadLine(), out zamiana);
                        if (!correctData)
                        {
                            Console.Write("Wpisz poprawną liczbę rzeczywistą dla {0}=", wyrazeniestring[i]);
                        }
                    } while (!correctData);
                    zmienne = zmienne + "   " + wyrazeniestring[i].ToString() + " = " + zamiana.ToString();
                    wyrazeniePoKonwersji = wyrazeniePoKonwersji.Replace(wyrazeniestring[i].ToString(), zamiana.ToString());
                }
            }
            char[] wyrazenie = new char[wyrazeniePoKonwersji.Length];
            for (int i = 0; i < wyrazeniePoKonwersji.Length; i++)
            {
                wyrazenie[i] = wyrazeniePoKonwersji[i];

            }

            Stack<string> stos = new Stack<string>();
            Stack<char> znaki = new Stack<char>();
            Stack<string> odwrotnosc = new Stack<string>();


            //algorytm konwertujący wyrażenie arytmetyczne do postaci Odwrotnej Adnotacji Polskiej
            for (int i = 0; i < wyrazenie.Length; i++)
            {
                //ignoruje spacje
                if (wyrazenie[i] == ' ')
                {

                }
                //sprawdza czy znakiem jest otwarcie nawiasu i od razu dodaje go do stosu znaków
                if (wyrazenie[i] == '(')
                {
                    i++;
                    stos.Push(Nawias(ref i, wyrazenie));
                    if (i > wyrazenie.Length)
                    {
                        continue;
                    }
                }
                //tworzy liczby ujemne
                if (i == 0 && wyrazenie[i] == '-')
                {
                    stos.Push(wyrazenie[i].ToString());
                    continue;
                }
                //sprawdza czy poprzedni znak w wyrażeniu jest liczbą albo kropką, jeśli tak to łączy poprzedni znak z obecnym tworząc liczby wielocyfrowe
                if ((i > 0 && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == true) || (i == 1 && wyrazenie[i - 1] == '-' && Liczba(ref wyrazenie[i]) == true))
                {
                    bufor = "";
                    bufor += stos.Pop();
                    bufor += wyrazenie[i];
                    stos.Push(bufor);
                    continue;
                }
                if (i > 2 && wyrazenie[i] != '-' && wyrazenie[i - 1] == '-' && (Liczba(ref wyrazenie[i - 2]) == false))
                {
                    bufor = "";
                    bufor += wyrazenie[i - 1];
                    bufor += wyrazenie[i];
                    stos.Push(bufor);
                    continue;
                }
                //sprawdza czy liczba jest cyfrą/literą i dodaje na stos
                else if (Liczba(ref wyrazenie[i]))
                {
                    //dodaje liczby na stos
                    stos.Push(wyrazenie[i].ToString());

                }
                //jeśli nie jest cyfrą/literą i nie ma otwarcia nawiasu to wrzuca na stos jeśli jest to znak * albo /
                else if (znaki.Count > 0)
                {
                    if (i > 1 && Liczba(ref wyrazenie[i - 1]) == false && wyrazenie[i] == '-')
                    {
                        continue;
                    }
                    if ((char)znaki.Peek() == '*' || (char)znaki.Peek() == '/' || (char)znaki.Peek() == '^')
                    {
                        stos.Push(znaki.Pop().ToString());
                    }

                }

                //powinno wrzucać znak + albo - do stosu jeśli następnym znakiem jest + lub -
                if (znaki.Count > 0)
                {
                    if (((wyrazenie[i] == '+' || wyrazenie[i] == '-') && (znaki.Peek() == '+' || znaki.Peek() == '-')) && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == false)
                    {
                        stos.Push(znaki.Pop().ToString());
                    }
                }

                //dodaje znaki + i - do stosu znaków
                if (wyrazenie[i] == '+' || wyrazenie[i] == '-')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                //dodaje znak * do stosu znaków
                if (wyrazenie[i] == '*')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                //dodaje znak / do stosu znaków
                if (wyrazenie[i] == '/')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                if (wyrazenie[i] == '^' && (stos.Peek() == "*" || stos.Peek() == "/"))
                {
                    if (stos.Peek() == "*")
                    {
                        znaki.Push('*');
                    }
                    else if (stos.Peek() == "/")
                    {
                        znaki.Push('/');
                    }
                    stos.Pop();
                    znaki.Push(wyrazenie[i]);
                }
                else if (wyrazenie[i] == '^')
                {
                    znaki.Push(wyrazenie[i]);
                }

            }
            //po zakończeniu pętli for wrzuca wszystkie znaki ze stosu znaków na stos wyrażenia
            while (znaki.Count > 0)
            {
                if ((char)znaki.Peek() == '(')
                {
                    znaki.Pop();
                }
                else
                {
                    stos.Push(znaki.Pop().ToString());
                }
            }
            //przerzuca wszystko na drugi stos, tworząc zapis Odwrotnej Notacji Polskiej
            while (stos.Count > 0)
            {
                odwrotnosc.Push((string)stos.Pop());
            }
            return odwrotnosc;
        }
        static float Liczenie(Stack<string> odwrotnosc)
        {
            float liczba1, liczba2;
            Stack<float> liczenie = new Stack<float>();
            while (odwrotnosc.Count > 0)
            {
                if (float.TryParse(odwrotnosc.Peek(), out float x))
                {
                    liczenie.Push(x);
                    odwrotnosc.Pop();
                }
                else
                {
                    if (odwrotnosc.Peek() == "+")
                    {
                        liczba2 = (float)Convert.ToDouble(liczenie.Pop());
                        liczba1 = (float)Convert.ToDouble(liczenie.Pop());
                        liczenie.Push(liczba1 + liczba2);
                        odwrotnosc.Pop();
                    }
                    else if (odwrotnosc.Peek() == "-")
                    {
                        liczba2 = (float)liczenie.Pop();
                        liczba1 = (float)liczenie.Pop();
                        liczenie.Push(liczba1 - liczba2);
                        odwrotnosc.Pop();
                    }
                    else if (odwrotnosc.Peek() == "*")
                    {
                        liczba2 = (float)liczenie.Pop();
                        liczba1 = (float)liczenie.Pop();
                        liczenie.Push(liczba1 * liczba2);
                        odwrotnosc.Pop();
                    }
                    else if (odwrotnosc.Peek() == "/")
                    {
                        liczba2 = (float)liczenie.Pop();
                        liczba1 = (float)liczenie.Pop();
                        liczenie.Push(liczba1 / liczba2);
                        odwrotnosc.Pop();
                    }
                    else if (odwrotnosc.Peek() == "^")
                    {

                        bool correctData;
                        int wykladnik;
                        correctData = int.TryParse(liczenie.Pop().ToString(), out wykladnik);
                        if (correctData == false) { Console.WriteLine("Wykładnik potęgi musi być liczbą całkowitą!"); break; }
                        liczba1 = (float)liczenie.Pop();
                        double potegowanie = Math.Pow(liczba1, wykladnik);
                        liczenie.Push((float)potegowanie);
                        odwrotnosc.Pop();
                    }
                }
            }
            float wynik = liczenie.Pop();
            return wynik;
        }
        static bool Liczba(ref char x)
        {
            if (x == '1' || x == '2' || x == '3' || x == '4' || x == '5' || x == '6' || x == '7' || x == '8' || x == '9' || x == '0' || x == '.' || x == ',')
            {
                if (x == '.')
                {
                    x = ',';
                }
                return true;
            }
            else { return false; }
        }
        static string Nawias(ref int k, char[] wyrazeniePozaNawiasem)
        {
            Stack<string> stos = new Stack<string>();
            Stack<char> znaki = new Stack<char>();
            Stack<string> odwrotnosc = new Stack<string>();
            int kopiowanie = 0;
            int iloscZnakow = 0;
            string bufor;
            int iloscNawiasow = 1;
            //sprawdza czy znakiem jest otwarcie nawiasu i od razu dodaje go do stosu znaków
            if (wyrazeniePozaNawiasem[k] == '(')
            {
                k++;
                stos.Push(Nawias(ref k, wyrazeniePozaNawiasem));//tu coś nie gra
            }
            for (int j = k; j < wyrazeniePozaNawiasem.Length && iloscNawiasow != 0; j++)
            {
                if (wyrazeniePozaNawiasem[j] == '(') { iloscNawiasow++; }
                if (wyrazeniePozaNawiasem[j] == ')') { iloscNawiasow--; }
                iloscZnakow++;
            }
            char[] wyrazenie = new char[iloscZnakow];
            for (; kopiowanie < iloscZnakow; k++)
            {
                wyrazenie[kopiowanie] = wyrazeniePozaNawiasem[k];
                kopiowanie++;

            }

            for (int i = 0; i < wyrazenie.Length; i++)
            {
                //ignoruje spacje
                if (wyrazenie[i] == ' ')
                {

                }
                //sprawdza czy znakiem jest otwarcie nawiasu i od razu dodaje go do stosu znaków
                if (wyrazenie[i] == '(')
                {
                    i++;
                    stos.Push(Nawias(ref i, wyrazenie));
                }
                if (wyrazenie[i]== ')')
                { break; }
                //tworzy liczby ujemne
                if (i == 0 && wyrazenie[i] == '-')
                {
                    stos.Push(wyrazenie[i].ToString());
                    continue;
                }
                //sprawdza czy poprzedni znak w wyrażeniu jest liczbą albo kropką, jeśli tak to łączy poprzedni znak z obecnym tworząc liczby wielocyfrowe
                if ((i > 0 && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == true) || (i == 1 && wyrazenie[i - 1] == '-' && Liczba(ref wyrazenie[i]) == true))
                {
                    bufor = "";
                    bufor += stos.Pop();
                    bufor += wyrazenie[i];
                    stos.Push(bufor);
                    continue;
                }
                if (i > 2 && wyrazenie[i - 1] == '-' && (wyrazenie[i - 2] == '+' || wyrazenie[i - 2] == '-' || wyrazenie[i - 2] == '*' || wyrazenie[i - 2] == '/' || wyrazenie[i - 2] == '^'))
                {
                    bufor = "";
                    bufor += wyrazenie[i - 1];
                    bufor += wyrazenie[i];
                    stos.Push(bufor);
                    continue;
                }
                //sprawdza czy liczba jest cyfrą/literą i dodaje na stos
                else if (Liczba(ref wyrazenie[i]))
                {
                    //dodaje liczby na stos
                    stos.Push(wyrazenie[i].ToString());

                }
                //jeśli nie jest cyfrą/literą i nie ma otwarcia nawiasu to wrzuca na stos jeśli jest to znak * albo /
                else if (znaki.Count > 0)
                {
                    if (i > 1 && Liczba(ref wyrazenie[i - 1]) == false && wyrazenie[i] == '-')
                    {
                        continue;
                    }
                    if ((char)znaki.Peek() == '*' || (char)znaki.Peek() == '/' || (char)znaki.Peek() == '^')
                    {
                        stos.Push(znaki.Pop().ToString());
                    }

                }

                //powinno wrzucać znak + albo - do stosu jeśli następnym znakiem jest + lub -
                if (znaki.Count > 0)
                {
                    if (((wyrazenie[i] == '+' || wyrazenie[i] == '-') && (znaki.Peek() == '+' || znaki.Peek() == '-')) && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == false)
                    {
                        stos.Push(znaki.Pop().ToString());
                    }
                }

                //dodaje znaki + i - do stosu znaków
                if (wyrazenie[i] == '+' || wyrazenie[i] == '-')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                //dodaje znak * do stosu znaków
                if (wyrazenie[i] == '*')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                //dodaje znak / do stosu znaków
                if (wyrazenie[i] == '/')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                if (wyrazenie[i] == '^' && (stos.Peek() == "*" || stos.Peek() == "/"))
                {
                    if (stos.Peek() == "*")
                    {
                        znaki.Push('*');
                    }
                    else if (stos.Peek() == "/")
                    {
                        znaki.Push('/');
                    }
                    stos.Pop();
                    znaki.Push(wyrazenie[i]);
                }
                else if (wyrazenie[i] == '^')
                {
                    znaki.Push(wyrazenie[i]);
                }

            }

            //po zakończeniu pętli for wrzuca wszystkie znaki ze stosu znaków na stos wyrażenia
            while (znaki.Count > 0)
            {
                if ((char)znaki.Peek() == '(')
                {
                    znaki.Pop();
                }
                else
                {
                    stos.Push(znaki.Pop().ToString());
                }
            }
            while (stos.Count > 0)
            {
                odwrotnosc.Push((string)stos.Pop());
            }
            string wynik = Liczenie(odwrotnosc).ToString();
            return wynik;
        }
        public static void Tmenu(ref int choice, int LastOption, ref System.ConsoleKey menu)
        {
            menu = Console.ReadKey().Key;
            switch (menu)
            {
                case ConsoleKey.Escape:
                    Console.WriteLine("WWyłączam program.");//dlaczego pierwsze w nie pokazuje sie w konsoli???
                    Thread.Sleep(500);
                    Environment.Exit(0);
                    break;
                case ConsoleKey.UpArrow:
                    if (choice == 1)
                    {
                        choice = LastOption;
                        break;
                    }
                    choice--;
                    break;
                case ConsoleKey.DownArrow:
                    if (choice == LastOption)
                    {
                        choice = 1;
                        break;
                    }
                    choice++;
                    break;
                default:
                    break;
            }
        } //techniczna strona menu
        public static void Menu(string header, string[] opcje, ref int choice, string wyrazenie)
        {

            System.ConsoleKey menu = ConsoleKey.O;
            do
            {
                Console.Clear();
                Console.WriteLine(header);
                for (int i = 1; i <= opcje.Length; i++)
                {
                    if (choice == i) { Console.WriteLine("->" + opcje[i - 1] + "" + "<-"); }
                    else
                    {
                        Console.WriteLine("  " + (opcje[i - 1]) + "" + "  ");
                    }
                }
                DateTime czas = DateTime.Now;
                Console.WriteLine("\n\n");
                if (wyrazenie != "")
                {
                    Console.WriteLine("Podane wyrażenie : " + wyrazenie);
                }
                Console.WriteLine("\n\n");
                Console.WriteLine("Użyj strzałek by nawigować po menu.\nEnter, aby zatwierdzić.\nWciśnij ESC aby wyłączyć program.\n" + czas);
                Console.WriteLine();
                Tmenu(ref choice, opcje.Length, ref menu);
            }
            while (menu != ConsoleKey.Enter);
        }//widoczna strona menu

    }
}