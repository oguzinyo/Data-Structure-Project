using System;
namespace BlockchainAnalysis.DataStructures {
    // LIFO (Last-In First-Out) prensibiyle calisan, thread-safe Yigit yapisi.
    // Push ve Pop islemleri O(1) sabit zaman karmasikligi ile gerceklesir.
    public class Stack {
        private Node top;
        private readonly object _lock = new object(); // Veri butunlugu icin thread-safety kilidi

        public void Push(int data) {
            lock (_lock) {
                Node newNode = new Node(data);
                newNode.Next = top;
                top = newNode;
            }
        }

        public int Pop() {
            lock (_lock) {
                if (top == null) { Console.WriteLine("Stack boş!"); return -1; }
                int poppedData = top.Data;
                top = top.Next;
                return poppedData;
            }
        }
    }
}
