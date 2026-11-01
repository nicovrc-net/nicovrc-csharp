using System.Net.Sockets;
using System.Text;

public sealed class TcpConnection : IAsyncDisposable
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;

    public event Action<string>? MessageReceived;

    public TcpConnection(TcpClient client)
    {
        _client = client;
        _stream = client.GetStream();
    }

    public async Task StartReceiveAsync(CancellationToken ct)
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (!ct.IsCancellationRequested)
            {
                int length = await _stream.ReadAsync(buffer, ct);

                if (length == 0)
                    break;

                string message =
                    Encoding.UTF8.GetString(buffer, 0, length);

                MessageReceived?.Invoke(message);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async Task SendAsync(string message, CancellationToken ct = default)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(data, ct);
    }

    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
        _client.Dispose();
    }

    
}

public class Program {

    static async Task Main (string[] args){
        var listener = TcpListener.Create(25252);
        listener.Start();

        while (true) {
            var client = await listener.AcceptTcpClientAsync();
            _ = Task.Run(async () =>
            {
                var conn = new TcpConnection(client);

                conn.MessageReceived += async msg =>
                {
                    Console.WriteLine($"受信: {msg}");
                    string date = DateTime.UtcNow.ToString("R");
                    // 受信したら返信
                    string testHttp = """
                    HTTP/1.1 200 OK
                    Content-Type: text/plain; charset=UTF-8
                    Date: ${date}
                    Content-Length: 4

                    test
                    """;
                    await conn.SendAsync(testHttp);
                };

                await conn.StartReceiveAsync(CancellationToken.None);
            });
        }
    }
}