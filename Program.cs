
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;

namespace net.nicovrc.dev
{
    public sealed class TcpConnection : IAsyncDisposable {
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

        public async Task SendAsync(byte[] data, CancellationToken ct = default)
        {
            //byte[] data = Encoding.UTF8.GetBytes(message);
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
                        //Console.WriteLine($"受信: \n{msg}");

                        //GetURL.SayHello();                    
                    
                        string uri = HTTPFunc.GetHTTPURI(msg) ?? "/";
                        byte[] body = Encoding.UTF8.GetBytes("nicovrc-csharp/"+NicovrcSetting.Version+"\n\n"+msg);

                        //Console.WriteLine(uri);
                        if (uri.StartsWith("/?url="))
                        {
                            body = Encoding.UTF8.GetBytes("Bad Request");
                            await conn.SendAsync(GetURL.Run(msg, uri) ?? HTTPFunc.CreateHTTPData(HTTPFunc.GetHTTPVersion(msg), 503, "text/plain; charset=UTF-8", null, null, body));
                            return;
                        }

                        await conn.SendAsync(HTTPFunc.CreateHTTPData(null, 200, "text/plain; charset=UTF-8", null, null, body));
                    };

                    await conn.StartReceiveAsync(CancellationToken.None);
                });
            }
        }
    }
}