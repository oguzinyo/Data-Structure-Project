using System;
namespace BlockchainAnalysis.DataStructures {
    public class Stack {
        private Node top;
        private readonly object _lock = new object();

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