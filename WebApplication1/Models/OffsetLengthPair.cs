namespace WebApplication1.Models
{
    public class OffsetLengthPair
    {
        public int offset { get; set; }
        public int length { get; set; }

        public OffsetLengthPair(int offset, int length)
        {
            this.offset = offset;
            this.length = length;
        }
    }
}
