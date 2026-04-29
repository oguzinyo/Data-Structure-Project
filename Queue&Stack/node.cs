namespace BlockchainAnalysis.DataStructures {
    // Bagli liste yapisinin temel tasi olan dugum (node) yapisi
    public class Node {
        public int Data;
        public Node Next;
        public Node(int data) { Data = data; Next = null; }
    }
}
