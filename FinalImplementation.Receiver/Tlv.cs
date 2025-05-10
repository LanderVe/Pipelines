using System.Buffers;
using System.Text;

namespace FinalImplementation.Receiver;

record struct Tlv(TlvType Type, string Value)
{
    public static Tlv Empty { get; } = new Tlv();

    public static bool TryParse(ref ReadOnlySequence<byte> buffer, out Tlv tlv)
    {
        if (buffer.Length < 3)
        {
            tlv = Empty;
            return false;
        }

        SequenceReader<byte> reader = new(buffer);
        reader.TryRead(out byte typeAsByte);
        TlvType type = (TlvType)typeAsByte;
        reader.TryReadBigEndian(out short lengthAsShort);
        ushort length = (ushort)lengthAsShort;

        if (!reader.TryReadExact(length, out ReadOnlySequence<byte> valueSequence))
        {
            tlv = Empty;
            return false;
        }

        string value = Encoding.UTF8.GetString(valueSequence);
        tlv = new(type, value);
        buffer = buffer.Slice(length + 3);
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