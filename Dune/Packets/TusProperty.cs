namespace Dune.Packets
{
    public struct TusProperty
    {
        public const int Size = sizeof(byte) + 2 * sizeof(uint);

        public byte Index;
        public uint Value1;
        public uint Value2;

        public TusProperty(uint value1, uint value2)
            : this()
        {
            Value1 = value1;
            Value2 = value2;
        }
        public TusProperty(byte index, uint value1, uint value2)
            : this()
        {
            Index = index;
            Value1 = value1;
            Value2 = value2;
        }

        public override string ToString()
        {
            return Index + " " + Value1 + " " + Value2;
        }
    }
}