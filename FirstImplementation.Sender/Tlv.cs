using System.Buffers.Binary;
using System.Text;

namespace FirstImplementation.Sender;

record struct Tlv(TlvType Type, string Value)
{
    public static Tlv Empty { get; } = new Tlv();

    public readonly int WriteBytes(Span<byte> bytes)
    {
        bytes[0] = (byte)Type;
        int length = Encoding.ASCII.GetBytes(Value, bytes[3..]);
        BinaryPrimitives.WriteUInt16BigEndian(bytes[1..3], (ushort)length);
        return 3 + length;
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