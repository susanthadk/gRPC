using Grpc.Core;
using Max;

const string target = "127.0.0.1:50052";

Channel channel = new Channel(target, ChannelCredentials.Insecure);

await channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine("The client connected successfully");
});

var client = new FindMaxService.FindMaxServiceClient(channel);
var stream = client.FindMaximum();

int[] numbers = { 1, 5, 3, 6, 2, 20 };
foreach (int number in numbers)
{
    await stream.RequestStream.WriteAsync(new FindMaxRequest() { Number = number});
}

var responseReaderTask = Task.Run(async () =>
{
    while (await stream.ResponseStream.MoveNext())
    {
        Console.WriteLine(stream.ResponseStream.Current.Max);
        //await Console.Out.WriteLineAsync(stream.ResponseStream.Current.Max.ToString());
    }
});

await stream.RequestStream.CompleteAsync();
await responseReaderTask;

channel.ShutdownAsync().Wait();
Console.ReadKey();