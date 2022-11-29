using System.Text;

namespace NamePairing;

public class BinaryLinkReader : BinaryReader
{
    public BinaryLinkReader(byte[] input) : base(new MemoryStream(input), Encoding.UTF8, true)
    {
    }
}

public sealed class BinaryLinkWriter : BinaryWriter
{
    public BinaryLinkWriter(byte[] formatMagic) : base(new MemoryStream(), Encoding.UTF8, true) {
        Write(formatMagic);
    }

    public BinaryLinkWriter WriteStringBytes(byte[] input) {
        Write7BitEncodedInt(input.Length);
        Write(input);
        return this;
    }

    public BinaryLinkWriter WriteNullByte() {
        Write(new byte[] {0x00});
        return this;
    }

    
    public MemoryStream Stream => (MemoryStream)BaseStream;
}