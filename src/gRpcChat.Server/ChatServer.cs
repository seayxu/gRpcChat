using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcChat;
using Grpc.Core.Utils;

namespace gRpcChat.Server
{
    public class ChatServer: Chat.ChatBase
    {
        class Stream<T>
        {
            public IServerStreamWriter<T> ServerStream { get; set; }
            public ServerCallContext Context { get; set; }

            public Stream()
            {
            }

            public Stream(IServerStreamWriter<T> serverStream, ServerCallContext context)
            {
                ServerStream = serverStream;
                Context = context;
            }
        }

        List<string> Users = null;
        Dictionary<string, Stream<ChatMessage>> listenner = null;

        public ChatServer()
        {
            Users = new List<string>();
            listenner = new Dictionary<string, Stream<ChatMessage>>();
        }

        public override Task<ChatResponse> Join(ChatRequest request, ServerCallContext context)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} \r\n {request.Name} Join");

            bool status = true;

            if (Users.Contains(request.Name)) status=false;

            Users.Add(request.Name);

            return Task.FromResult(new ChatResponse() { Name = request.Name, Status = status });
        }

        public override async Task Listen(ChatRequest request, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            if (listenner.ContainsKey(request.Name)) listenner.Remove(request.Name);

            listenner.Add(request.Name, new Stream<ChatMessage>(responseStream, context));

            context.CancellationToken.WaitHandle.WaitOne();
            
            await Task.FromCanceled(context.CancellationToken);
        }

        public override Task<ChatMessageResponse> Send(ChatMessage request, ServerCallContext context)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} \r\n {request.Name} Send:{request.Message}");

            if (listenner.Count > 0)
            {
                foreach (var item in listenner)
                {
                    if (item.Key == request.Name) continue;

                    item.Value.ServerStream.WriteAsync(request);
                }
            }

            return Task.FromResult(new ChatMessageResponse() { Name = request.Name, Message = request.Message, Timestamp = request.Timestamp, Status = true });
        }

        public override Task<ChatResponse> Exit(ChatRequest request, ServerCallContext context)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} \r\n {request.Name} Exit");

            if (Users.Contains(request.Name)) Users.Remove(request.Name);

            return Task.FromResult(new ChatResponse() { Name = request.Name, Status = true });
        }
    }
}
