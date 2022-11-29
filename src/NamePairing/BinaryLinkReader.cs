using System.Text;

namespace NamePairing;

public class BinaryLinkReader : BinaryReader
{
    public BinaryLinkReader(byte[] input) : base(new MemoryStream(input), Encoding.UTF8, true)
    {
    }
}