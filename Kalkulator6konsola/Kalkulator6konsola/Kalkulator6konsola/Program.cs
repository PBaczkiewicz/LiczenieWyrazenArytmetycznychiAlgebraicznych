using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalkulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string option = "";
            do
            {
                //Console.Clear();
                Console.Write("Kalkulator do obliczeń arytmetycznych i algebraicznych.\n+ <- znak dodawania\n- <-znak odejmowania\n* <- znak mnożenia\n/ <- znak dzielenia\nPodaj wyrażenie do obliczenia :");
                bool nawias = false, correctData = false;
                string wyrazeniestring, wyrazeniePoKonwersji = "", bufor = "";
                float liczba1, liczba2, zamiana;



                wyrazeniestring = Console.ReadLine();
                wyrazeniePoKonwersji = wyrazeniestring;
                #region
                //szuka zmiennych (liter) w wyrażeniu i podaje w konsoli prośbę o podanie liczby do podstawienia. Zamienia wszystkie te same litery na jedną podaną zmienną
                for (int i = 0; wyrazeniestring.Length > i; i++)
                {
                    if (char.IsLetter(wyrazeniestring[i]) && wyrazeniePoKonwersji.Contains(wyrazeniestring[i]))
                    {
                        Console.Write("Wpisz wartość zmiennej {0}=", wyrazeniestring[i]);
                        do
                        {
                            correctData = float.TryParse(Console.ReadLine(), out zamiana);
                            if (!correctData)
                            {
                                Console.Write("Wpisz poprawną liczbę rzeczywistą dla {0}=");
                            }
                        } while (!correctData);
                        wyrazeniePoKonwersji = wyrazeniePoKonwersji.Replace(wyrazeniestring[i].ToString(), zamiana.ToString());
                    }
                }
                char[] wyrazenie = new char[wyrazeniePoKonwersji.Length];
                for (int i = 0; i < wyrazeniePoKonwersji.Length; i++)
                {
                    wyrazenie[i] = wyrazeniePoKonwersji[i];

                }
                #endregion
                Stack<string> stos = new Stack<string>();
                Stack<char> znaki = new Stack<char>();
                Stack<string> odwrotnosc = new Stack<string>();
                Stack<float> liczenie = new Stack<float>();

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
                        nawias = true;
                        znaki.Push(wyrazenie[i]);
                    }
                    //sprawdza czy poprzedni znak w wyrażeniu jest liczbą albo kropką, jeśli tak to łączy poprzedni znak z obecnym tworząc liczby 2+ cyfrowe
                    if (i > 0 && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == true)
                    {
                        bufor = "";
                        bufor += stos.Pop();
                        bufor += wyrazenie[i];
                        stos.Push(bufor);
                    }
                    //sprawdza czy liczba jest cyfrą/literą i dodaje na stos
                    else if (Liczba(ref wyrazenie[i]))
                    {
                        //dodaje liczby na stos
                        stos.Push(wyrazenie[i].ToString());

                    }
                    //jeśli nie jest cyfrą/literą i nie ma otwarcia nawiasu to wrzuca na stos jeśli jest to znak * albo /
                    else if (znaki.Count > 0 && nawias == false)
                    {
                        if ((char)znaki.Peek() == '*' || (char)znaki.Peek() == '/')
                        {
                            stos.Push(znaki.Pop().ToString());
                        }
                    }

                    //sprawdza czy nie było otwarcia nawiasu poprzednio ->do poprawienia w chuj
                    if (nawias == true && i > 1 && Liczba(ref wyrazenie[i]) == false)
                    {
                        if ((char)znaki.Peek() == '*' || (char)znaki.Peek() == '/')
                        {
                            stos.Push(znaki.Pop().ToString());
                        }
                        //jeśli następnym znakiem jest zamknięcie nawiasu to dodaje znaki na stos aż do znaku otwarcia nawiasu
                        if (wyrazenie[i] == ')')
                        {
                            while ((char)znaki.Peek() != '(' && znaki.Contains('(') == true)
                            {
                                stos.Push(znaki.Pop().ToString());
                            }
                            znaki.Pop();
                            nawias = false;
                        }
                    }

                    //powinno wrzucać znak + albo - do stosu jeśli następnym znakiem jest + lub -
                    if (znaki.Count > 0)
                    {
                        if ((wyrazenie[i] == '+' || wyrazenie[i] == '-') && (znaki.Peek() == '+' || znaki.Peek() == '-'))
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

                //test czy działa
                //while (odwrotnosc.Count > 0)
                //{
                //    Console.Write(odwrotnosc.Pop() + " ");//ŹLE WPISUJE ZNAKI + -
                //}
                

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
                    }
                }
                Console.WriteLine("Wynik tego wyrażenia to {0}", liczenie.Pop());
                
                option = "";
                Console.WriteLine("Policzyć kolejne wyrażenie? Wpisz T, aby ponowić");
                option = Console.ReadLine();
            } while (option == "T" || option == "t");
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
    }
}