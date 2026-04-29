using System;
namespace BlockchainAnalysis.DataStructures {
    // FIFO (First-In First-Out) prensibiyle calisan, thread-safe Kuyruk yapisi.
    // Tum ekleme (Enqueue) ve cikarma (Dequeue) islemleri O(1) karmasikligindadir.
    public class Queue {
        private Node front;
        private Node rear;
        private readonly object _lock = new object(); // Multithread calisma güvenligi icin lock objesi

        public void Enqueue(int data) {
            lock (_lock) {
                Node newNode = new Node(data);
                if (rear == null) { front = rear = newNode; return; }
                rear.Next = newNode; rear = newNode;
            }
        }

        public int Dequeue() {
            lock (_lock) {
                if (front == null) { Console.WriteLine("Queue boş!"); return -1; }
                int dequeuedData = front.Data;
                front = front.Next;
                if (front == null) rear = null;
                return dequeuedData;
            }
        }
    }
}
