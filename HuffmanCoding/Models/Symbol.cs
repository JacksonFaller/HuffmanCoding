
namespace HuffmanCoding.Models
{
    /// <summary>
    /// Structure representing character and probability of finding it in text
    /// </summary>
    public struct Symbol
    {
        /// <summary>
        /// Character that symbol represents
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Probability of finding symbol
        /// </summary>
        public float Probability { get; set; }

        public Symbol(float probability, char character = default)
        {
            Probability = probability;
            Character = character;
        }

        public override int GetHashCode()
        {
            return Character.GetHashCode() + Probability.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;

            var symbol = (Symbol)obj;
            return Character.Equals(symbol.Character) && Probability.Equals(symbol.Probability);
        }
    }
}
