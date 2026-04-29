using System;
using BlockchainAnalysis.DataStructures;

namespace BlockchainAnalysis.App {
    class Program {
        static void Main() {
            Console.WriteLine("--- Queue & Stack Testi Basliyor ---");
            
            Queue q = new Queue();
            q.Enqueue(1001);
            Console.WriteLine($"Kuyruktan Cikan (BFS): {q.Dequeue()}");

            Stack s = new Stack();
            s.Push(5001);
            Console.WriteLine($"Yigitttan Cikan (DFS): {s.Pop()}");
        }
    }
}