using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Filozofowie
{
    class FewPhilosophers
    {
        private Thread[] philosophers; // tablica wątków filozofów
        private Semaphore[] forks; // tablica binarnych semaforów widelce
        private Semaphore waiter; // semafor dopuszczający co najwyżej n - 1 filozofów do stołu

        // Konstruktor klasy FewPhilosophers inicjalizujący semafory oraz wątki 
        public FewPhilosophers(int n)
        {
            waiter = new Semaphore(n - 1, n - 1);

            forks = new Semaphore[n];
            for (int i = 0; i < n; i++)
            {
                forks[i] = new Semaphore(1, 1); // semafory binarne
            }

            philosophers = new Thread[n];
            for (int i = 0; i < n; i++)
            {
                philosophers[i] = new Thread(new ThreadStart(Philosoph)); // tworzymy wątki
                int numberToName = i + 1;
                philosophers[i].Name = "Filozof " + numberToName; // nadajemy nazwy wątkom
            }
        }

        // Uruchamia wszystkie wątki.
        public void Start()
        {
            foreach (Thread t in philosophers)
            {
                t.Start();
            }
        }

        // Kończy wszystkie wątki 
        public void End()
        {
            foreach (Thread t in philosophers)
            {
                t.Interrupt();
            }
        }

        // Wątek filozofa, który w nieskończonej pętli wykonuje następujące czynności:
        // - myśli przez losowy czas
        // - próbuje zdobyć dwa widelce
        // - je przez losowy czas 
        public void Philosoph()
        {
            try
            {
                // wydobywamy z nazwy numer filozofa
                int number = Int32.Parse(Thread.CurrentThread.Name.Split(' ')[1]) - 1;
                Random rand = new Random();
                int n = philosophers.Length; // ilość filozofów
                while (true)
                {
                    Console.WriteLine(Thread.CurrentThread.Name + " think");
                    Thread.Sleep(rand.Next(300, 1000)); // myśli przez losowy czas
                    waiter.WaitOne(); // sprawdzenie czy może usiąść do stołu
                    forks[number].WaitOne(); // bierze lewy widelec
                    forks[(number + 1) % n].WaitOne(); // bierze prawy widelec
                    Console.WriteLine(Thread.CurrentThread.Name + " eat");
                    Thread.Sleep(rand.Next(300, 600)); // je przez losowy czas
                    Console.WriteLine(Thread.CurrentThread.Name + " back to think");
                    forks[number].Release(); // odkłada lewy widelec
                    forks[(number + 1) % n].Release(); // odkłada prawy widelec
                    waiter.Release(); // dopuszczamy innego filozofa do stołu                   
                }
            }
            catch (ThreadInterruptedException) { }
        }

        static void Main(string[] args)
        {
            int n = 0;
            while (n < 5 || n > 10)
            {
                Console.WriteLine("Number of philosophers (min 5, max 10): ");
                n = Int32.Parse(Console.ReadLine());
            }
            FewPhilosophers fp = new FewPhilosophers(n);
            fp.Start(); // zaczynamy wątki
            Thread.Sleep(5000); // czas trwania programu
            fp.End(); // przerywamy wątki
            Console.ReadKey();
        }
    }
}