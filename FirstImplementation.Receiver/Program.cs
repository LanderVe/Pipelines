using System.Net.Sockets;
using System.Net;
using FirstImplementation.Receiver;

// Listen for incoming connections
using Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPEndPoint endPoint = new(IPAddress.Loopback, 9999);
socket.Bind(endPoint);
socket.Listen();
var listener = await socket.AcceptAsync();

await ProcessData(listener);
listener.Close();
Console.ReadKey();


async static Task ProcessData(Socket listener)
{
    byte[] buffer = new byte[1024];
    int bytesRead = await listener.ReceiveAsync(buffer);

    ReadOnlySpan<byte> data = buffer.AsSpan(0, bytesRead);
    while (Tlv.TryParse(ref data, out Tlv tlv))
    {
        Console.WriteLine(tlv);
    }
}