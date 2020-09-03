using gRpcChat.Client;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello gRpcChat!");
            Console.Write("Name:");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = Guid.NewGuid().ToString();

            Console.WriteLine();

            Console.WriteLine("Hello " + name);

            string message = null;

            Chater chater = new Chater(name);

            chater.Login();

            do
            {
                message = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(message)) continue;

                chater.Send(message);

            } while (message!="q");

            Console.WriteLine($"{name} exit");
            Console.ReadLine();
        }
    }
}
