using Grpc.Core;
using Prime;
using server;

const int Port = 50051;

Server server = null;

try
{
    server = new Server()
    {
        Services = { PrimeNumberService.BindService(new PrimeNumberServiceImpl()) },
        Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
    };

    server.Start();
    Console.WriteLine("The server is listening on the port : " + Port);
    Console.ReadKey();
}
catch (IOException ex)
{
    Console.WriteLine("The server is failed to start : " + ex.Message);
    throw;
}
finally
{
    if (server != null)
    {
        server.ShutdownAsync().Wait();
    }
}