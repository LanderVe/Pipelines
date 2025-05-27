using FirstImplementation.Sender;
using System.Net.Sockets;

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
index += new Tlv(TlvType.Isbn, "13-46-79").WriteBytes(data);
index += new Tlv(TlvType.Author, "J.R.R. Tolkien").WriteBytes(data[index..]);
index += new Tlv(TlvType.Title, "Lord of the Rings").WriteBytes(data[index..]);
index += new Tlv(TlvType.Pages, "1178").WriteBytes(data[index..]);

// Final message
Memory<byte> message = bytes.AsMemory(0, index);

// send TCP message
await socket.SendAsync(message);

// Close connection
await socket.DisconnectAsync(false);