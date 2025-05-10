using System.Buffers;
using System.Text;

namespace ReadOnlySequenceDemo;

static class TlvTest
{
    public static void Run()
    {
        // create TLVs
        var s1 = "13-46-79"u8.ToArray();
        var s2 = "J.R.R. Tolkien"u8.ToArray();
        var s3 = "Lord of the Rings"u8.ToArray();
        var s4 = "1178"u8.ToArray();
        byte[] tlv1 = [1, 0, (byte)s1.Length, .. s1];
        byte[] tlv2 = [2, 0, (byte)s2.Length, .. s2];
        byte[] tlv3 = [3, 0, (byte)s3.Length, .. s3];
        byte[] tlv4 = [4, 0, (byte)s4.Length, .. s4];

        // Create Segments
        var first = new MyReadOnlySequenceSegment<byte>(tlv1);
        var last = first
            .Append(tlv2)
            .Append(tlv3)
            .Append(tlv4);

        // Create ReadOnlySequence
        ReadOnlySequence<byte> sequence = new(first, 0, last, tlv4.Length);

        // Read TLVs
        ReadTlvsWithSequenceReader(sequence);
    }

    static void ReadTlvsWithSequenceReader(ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);

        while (!reader.End)
        {
            reader.TryRead(out byte type);
            reader.TryReadBigEndian(out short length);
            reader.TryReadExact(length, out var value);
            Console.WriteLine($"Type: {type}, Length: {length}, Value: {Encoding.UTF8.GetString(value)}");
        }
    }
}
