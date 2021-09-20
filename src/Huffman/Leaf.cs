namespace Huffman
{
    class Leaf : No  //叶子结点
    {
        
        public char Character { get; set; }
        public Leaf(char character, int frequencia) : base(frequencia)
        {
            Character = character;
            Weight = frequencia;
        }
    }
}
