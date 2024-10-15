using Dummy;
using Greet;
using Grpc.Core;

const string target = "127.0.0.1:50052";

Channel channel = new Channel(target, ChannelCredentials.Insecure);

await channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine("The client connected successfully");
});

//var client = new DummyService.DummyServiceClient(channel);

var client = new GreetingService.GreetingServiceClient(channel);



//// Unary
//DoSimpleGreet(client);
//// Server streaming
//await DoManyGreeings(client);
//// client streaming
//await DoLongGreet(client);
// bi-di streaming
await DoGreetEveryone(client);

channel.ShutdownAsync().Wait();
Console.ReadKey();

static void DoSimpleGreet(GreetingService.GreetingServiceClient client)
{
    var greeting = new Greeting()
    {
        FirstName = "Susantha",
        LastName = "Kumara"
    };

    var request = new GreetingRequest() { Greeting = greeting };
    var response = client.Greet(request);
    Console.WriteLine(response.Result);
}

static async Task DoManyGreeings(GreetingService.GreetingServiceClient client)
{
    var greeting = new Greeting()
    {
        FirstName = "Susantha",
        LastName = "Kumara"
    };

    var request = new GreetManyTimesRequest() { Greeting = greeting };
    var response = client.GreetManyTimes(request);
    while (await response.ResponseStream.MoveNext())
    {
        Console.WriteLine(response.ResponseStream.Current.Result);
        await Task.Delay(500);
    }
}

static async Task DoLongGreet(GreetingService.GreetingServiceClient client)
{
    var greeting = new Greeting()
    {
        FirstName = "Susantha",
        LastName = "Kumara"
    };

    var request = new LongGreetRequest() { Greeting = greeting };
    var stream = client.LongGreet();
    foreach (int i in Enumerable.Range(1, 10))
    {
        await stream.RequestStream.WriteAsync(request);
    }
    await stream.RequestStream.CompleteAsync();
    var response = await stream.ResponseAsync;
    Console.WriteLine(response.Result);
}

static async Task DoGreetEveryone(GreetingService.GreetingServiceClient client)
{
    var stream = client.GreetEveryone();

    Greeting[] greetings =
    {
        new Greeting() {FirstName = "John", LastName = "Doe"},
        new Greeting() {FirstName = "Clement", LastName = "Jean"},
        new Greeting() {FirstName = "Patricia", LastName = "Hertz"}
    };

    foreach (var greeting in greetings)
    {
        await Console.Out.WriteLineAsync("Sending : " + greeting.ToString());
        await stream.RequestStream.WriteAsync(new GreetEveryoneRequest()
        {
            Greeting = greeting
        });
    }

    var responseReadeTask = Task.Run(async () =>
    {
        while (await stream.ResponseStream.MoveNext())
        {
            await Console.Out.WriteLineAsync("Received : " + stream.ResponseStream.Current.Result);
        }
    });

    await stream.RequestStream.CompleteAsync();
    await responseReadeTask;
}