using FinalImplementation.Sender;
using System.Net.Sockets;

// Configure
int chunkSize = 32; // 100, 4, 1000
int delay = 200; // 200, 200, 10
int messageDuplicates = 1; // 1, 1, 10
int sendIterations = 1000; // 1, 1, 10000

Console.WriteLine("Press any key to start");
Console.ReadKey();

// make TCP connection
using Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
await socket.ConnectAsync("localhost", 9999);

// Create buffer
byte[] bytes = new byte[1024];

// Write TLV data
Span<byte> data = bytes;
int index = 0;
for (int i = 0; i < messageDuplicates; i++)
{
    index += new Tlv(TlvType.Isbn, "13-46-79").WriteBytes(data[index..]);
    index += new Tlv(TlvType.Author, "J.R.R. Tolkien").WriteBytes(data[index..]);
    index += new Tlv(TlvType.Title, "Lord of the Rings").WriteBytes(data[index..]);
    index += new Tlv(TlvType.Pages, "1178").WriteBytes(data[index..]);
}

// Final message
bytes = data[..index].ToArray();

var chunks = bytes.Chunk(chunkSize);
for (int i = 0; i < sendIterations; i++)
{
    foreach (var chunk in chunks)
    {
        await socket.SendAsync(chunk);
        await Task.Delay(delay);
    }
}

await socket.DisconnectAsync(false);