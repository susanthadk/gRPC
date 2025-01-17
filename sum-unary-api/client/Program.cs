﻿
using Calculator;
using Grpc.Core;

const string target = "127.0.0.1:50052";

Channel channel = new Channel(target, ChannelCredentials.Insecure);

channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine("The client connected successfully");
});

//var client = new DummyService.DummyServiceClient(channel);

var client = new CalculatorService.CalculatorServiceClient(channel);

var request = new SumRequest()
{
    A = 3,
    B = 10
};

var response = client.Sum(request);
Console.WriteLine(response.Result);

channel.ShutdownAsync().Wait();
Console.ReadKey();