using Grpc.Core;
using GrpcChat;
using System;

namespace gRpcChat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello gRpc Chat Server!");

            //Chat.BindService(new ChatServer());
            Grpc.Core.Server server = new Grpc.Core.Server()
            {
                Services = { Chat.BindService(new ChatServer()) },
                Ports = { new ServerPort("127.0.0.1", 4001, ServerCredentials.Insecure) }
            };

            server.Start();
            string message = null;

            do
            {
                message = Console.ReadLine();
                //if (string.IsNullOrWhiteSpace(message)) continue;
                
                //client.Send(new SendMessageRequest() { Name = name, Message = message });

                //Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} \r\n Me:{listen.ResponseStream.Current.Message}");

            } while (message != "q");

            Console.ReadLine();
        }
    }
}
