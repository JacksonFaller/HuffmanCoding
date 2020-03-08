
namespace HuffmanCoding.Models
{
    public class SymbolFrequency
    {
        public char Character { get; set; }

        public int Count { get; set; }

        public SymbolFrequency(char character, int count = 1)
        {
            Character = character;
            Count = count;
        }

        public override int GetHashCode()
        {
            return Character.GetHashCode() + Count.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SymbolFrequency symbolFrequency)
                return Character.Equals(symbolFrequency.Character) && Count.Equals(symbolFrequency.Count);

            return false;
        }
    }
}
