using System.Net;
using System.Net.Sockets;
using System.Text;

var listener = TcpListener.Create(25252);

listener.Start();

Console.WriteLine("Listening...");

while (true)
{
    TcpClient client = await listener.AcceptTcpClientAsync();

    _ = Task.Run(() => HandleClientAsync(client));
}

static async Task HandleClientAsync(TcpClient client)
{
    await using var stream = client.GetStream();

    byte[] buffer = new byte[4096];

    try
    {
        while (true)
        {
            int received = await stream.ReadAsync(buffer);

            if (received == 0)
            {
                Console.WriteLine("Client disconnected");
                break;
            }

            string message = Encoding.UTF8.GetString(
                buffer.AsSpan(..received));

            Console.WriteLine($"RECV: {message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}