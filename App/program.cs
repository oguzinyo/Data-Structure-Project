using System;
using BlockchainAnalysis.DataStructures;

// Developed by: Ummet Erkan

namespace BlockchainAnalysis.App {
    class Program {
        static void Main() {
            Console.WriteLine("--- Queue & Stack Testi Basliyor ---");

            // Kuyruk (Queue) testi: BFS (Genislemesine Arama) algoritmasi temel mantigi simule edilir
            Queue q = new Queue();
            q.Enqueue(1001); // Veri kuyruga eklenir (FIFO)
            Console.WriteLine($"Kuyruktan Cikan (BFS): {q.Dequeue()}");

            // Yigit (Stack) testi: DFS (Derinlemesine Arama) algoritmasi temel mantigi simule edilir
            Stack s = new Stack();
            s.Push(5001); // Veri yigita eklenir (LIFO)
            Console.WriteLine($"Yigitttan Cikan (DFS): {s.Pop()}");
        }
    }
}
