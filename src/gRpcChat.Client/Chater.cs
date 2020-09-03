using Grpc.Core;
using GrpcChat;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gRpcChat.Client
{
    public class Chater
    {
        public string Name { get; private set; }
        public string Target { get; private set; } = "127.0.0.1:4001";

        Channel channel = null;
        Chat.ChatClient client = null;

        CancellationTokenSource cts = null;
        Task ListenTask = null;

        public Chater(string name, string target = "127.0.0.1:4001")
        {
            Name = name;
            Target = target;
            channel = new Channel(Target, ChannelCredentials.Insecure);

            client = new Chat.ChatClient(channel);
        }

        public void Join()
        {
            var response = client.Join(new ChatRequest() { Name = Name });

            System.Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [{response.Name}] join {response.Status}");

            cts = new CancellationTokenSource();

            ListenTask = Task.Run(async() =>
            {
                var listen = client.Listen(new ChatRequest() { Name = Name });

                CancellationTokenSource _cts = null;
                _cts = new CancellationTokenSource();

                do
                {
                    try
                    {
                        await listen.ResponseStream.MoveNext(cts.Token);

                        //if (!ret) continue;
                        if (listen.ResponseStream.Current == null) continue;

                        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}->{listen.ResponseStream.Current.Name}:{listen.ResponseStream.Current.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                } while (!cts.IsCancellationRequested);

            }, cts.Token);
        }

        public void Send(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrEmpty(message)) return;

            var response = client.Send(new ChatMessage() { Name = Name, Message = message,Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") });

            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}->Me:{message}->{response.Status}");
        }

        public void Exit()
        {
            var response = client.Exit(new ChatRequest() { Name = Name });

            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}->exit->{response.Status}");
        }
    }
}
