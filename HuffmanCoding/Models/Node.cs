using System;

namespace HuffmanCoding.Models
{
    public class Node : IComparable<Node>
    {
        public Node(Symbol symbol)
        {
            Symbol = symbol;
        }

        public bool IsInternal { get; set; }

        public Symbol Symbol { get; set; }

        public Node LeftChild { get; set; }

        public Node RightChild { get; set; }

        public int CompareTo(Node other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (Symbol.Probability == other.Symbol.Probability)
                return 0;

            return Symbol.Probability > other.Symbol.Probability ? 1 : -1;
        }
    }
}
