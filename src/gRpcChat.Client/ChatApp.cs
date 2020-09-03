using System;
using System.Threading.Tasks;

namespace gRpcChat.Client
{
    public class ChatApp
    {
        public void Run()
        {
            Console.WriteLine("Hello gRpcChat!");

            Console.Write("Name:");

            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = Guid.NewGuid().ToString();

            Console.WriteLine();

            Console.WriteLine("Hello " + name);

            string message = null;

            Chater chater = new Chater(name);

            chater.Join();

            bool running = true;
            bool paused = true;

            Task.Run(async () =>
            {
                while (running)
                {
                    try
                    {
                        if (!paused) chater.Send($"{name}-{DateTime.Now.ToString("HH:mm:ss.fff")}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{name} exception:{ex.Message}");
                    }

                    await Task.Delay(100);
                }
            });

            do
            {
                try
                {
                    message = Console.ReadLine();

                    switch (message)
                    {
                        case "0":
                            paused = true;
                            break;
                        case "1":
                            paused = false;
                            break;
                        default:
                            break;
                    }

                    if (string.IsNullOrWhiteSpace(message) || string.IsNullOrEmpty(message)) return;

                    chater.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            } while (message != "q");

            running = false;

            Console.WriteLine($"{name} exit");
            Console.ReadLine();
        }
    }
}
