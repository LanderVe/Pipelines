using FinalImplementation.Receiver;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;


using Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPEndPoint endPoint = new(IPAddress.Loopback, 9999);
//socket.ReceiveBufferSize = 1024; // slows down traffic
socket.Bind(endPoint);
socket.Listen();
var listener = await socket.AcceptAsync();

await Listen(listener);

async Task Listen(Socket socket)
{
    //PipeOptions options = new(pauseWriterThreshold: 1024, resumeWriterThreshold: 100); // backpressure
    //Pipe pipe = new(options);

    Pipe pipe = new();
    Task writing = FillPipeAsync(socket, pipe.Writer); // Socket --> Pipe
    Task reading = ReadPipeAsync(pipe.Reader); // Pipe --> Parse TLVs

    await Task.WhenAll(reading, writing);
}

async Task FillPipeAsync(Socket socket, PipeWriter writer)
{
    const int minimumBufferSize = 512;

    while (true)
    {
        // Allocate at least 512 bytes from the PipeWriter.
        Memory<byte> memory = writer.GetMemory(minimumBufferSize);
        try
        {
            int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None);
            if (bytesRead == 0)
            {
                break;
            }
            // Tell the PipeWriter how much was read from the Socket.
            writer.Advance(bytesRead);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            break;
        }

        // Make the data available to the PipeReader.
        long start = Stopwatch.GetTimestamp();
        FlushResult result = await writer.FlushAsync();
        Console.WriteLine($"FlushAsync: {Stopwatch.GetElapsedTime(start).TotalMilliseconds}ms");

        // reader is done readding and doesn't care about further data
        if (result.IsCompleted)
        {
            break;
        }

        //await Task.Delay(1000);
    }

    // By completing PipeWriter, tell the PipeReader that there's no more data coming.
    await writer.CompleteAsync();
}

async Task ReadPipeAsync(PipeReader reader)
{
    while (true)
    {
        ReadResult result = await reader.ReadAsync();
        ReadOnlySequence<byte> buffer = result.Buffer;

        while (Tlv.TryParse(ref buffer, out Tlv tlv))
        {
            // Process the tlv
            ProcessTlv(tlv);
            await Task.Delay(20);
        }

        // Tell the PipeReader how much of the buffer has been consumed.
        reader.AdvanceTo(buffer.Start, buffer.End);

        // Stop reading if there's no more data coming.
        if (result.IsCompleted)
        {
            break;
        }
    }

    // Mark the PipeReader as complete.
    await reader.CompleteAsync();
}

void ProcessTlv(Tlv tlv)
{
    Console.WriteLine(tlv);
}