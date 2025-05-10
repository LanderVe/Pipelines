using System.Buffers.Binary;
using System.Text;

namespace FirstImplementation.Receiver;

record struct Tlv(TlvType Type, string Value)
{
    public static Tlv Empty { get; } = new Tlv();

    public static bool TryParse(ref ReadOnlySpan<byte> buffer, out Tlv tlv)
    {
        if (buffer.Length < 3)
        {
            tlv = Empty;
            return false;
        }

        TlvType type = (TlvType)buffer[0];
        ushort length = BinaryPrimitives.ReadUInt16BigEndian(buffer[1..3]);
        if (buffer.Length < length + 3)
        {
            tlv = Empty;
            return false;
        }
        string value = Encoding.ASCII.GetString(buffer.Slice(3, length));

        tlv = new Tlv(type, value);
        buffer = buffer[(length + 3)..];
        return true;
    }
}

enum TlvType
{
    None = 0,
    Isbn = 1,
    Author = 2,
    Title = 3,
    Pages = 4,
}