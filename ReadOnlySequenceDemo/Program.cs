using ReadOnlySequenceDemo;
using System.Buffers;
using System.Buffers.Binary;

byte[] b1 = [1, 2, 3, 4];
byte[] b2 = [5, 6, 7, 8];
byte[] b3 = [9, 10, 11, 12];

MyReadOnlySequenceSegment<byte> first = new(b1);
MyReadOnlySequenceSegment<byte> last = first.Append(b2)
    .Append(b3);

ReadOnlySequence<byte> sequence = new(first, 0, last, b3.Length);

//Console.WriteLine($"Length: {sequence.Length}");

//var pos = sequence.PositionOf((byte)7);

//SequencePosition position = sequence.Start;
//foreach(var segment in sequence)
//{
//	foreach (var item in segment.Span)
//	{
//        Console.WriteLine(item);
//	}
//}

//var slice = sequence.Slice(1, 8);
//Console.WriteLine($"Slice Length: {slice.Length}");

//Console.WriteLine(ReadInt(sequence));
//Console.WriteLine(ReadInt(sequence.Slice(2)));

//TlvTest.Run();

static int ReadInt(ReadOnlySequence<byte> sequence)
{
    var intSlice = sequence.Slice(0, 4);
    if (intSlice.IsSingleSegment)
    {
        return BinaryPrimitives.ReadInt32BigEndian(intSlice.FirstSpan);
    }
    else
    {
        Span<byte> bytes = stackalloc byte[4];
        intSlice.CopyTo(bytes);
        return BinaryPrimitives.ReadInt32BigEndian(bytes);
    }
}

static int ReadIntWithReader(ReadOnlySequence<byte> sequence)
{
    SequenceReader<byte> reader = new(sequence);
    return reader.TryReadBigEndian(out int value) ? value : 0;
}