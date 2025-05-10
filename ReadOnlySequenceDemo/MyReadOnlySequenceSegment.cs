using System.Buffers;

namespace ReadOnlySequenceDemo;

class MyReadOnlySequenceSegment<T> : ReadOnlySequenceSegment<T>
{
    public MyReadOnlySequenceSegment(ReadOnlyMemory<T> memory)
    {
        Memory = memory;
    }
    public MyReadOnlySequenceSegment<T> Append(ReadOnlyMemory<T> memory)
    {
        var segment = new MyReadOnlySequenceSegment<T>(memory)
        {
            RunningIndex = RunningIndex + Memory.Length
        };
        Next = segment;
        return segment;
    }
}