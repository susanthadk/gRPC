﻿using Greet;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Greet.GreetingService;

namespace server
{
    public class GreetingServiceImpl : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            string result = string.Format("hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);
            return Task.FromResult(new GreetingResponse() { Result = result});
        }

        public override async Task GreetManyTimes(GreetManyTimesRequest request, IServerStreamWriter<GreetManyTimesResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received he request : ");
            Console.WriteLine(request.ToString());

            string result = String.Format("hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);

            foreach (int i in Enumerable.Range(1, 10))
            {
                await responseStream.WriteAsync(new GreetManyTimesResponse() { Result = String.Format("{0}. {1}", i, result) });
            }
        }

        public override async Task<LongGreetResponse> LongGreet(IAsyncStreamReader<LongGreetRequest> requestStream, ServerCallContext context)
        {
            string result = "";

            while (await requestStream.MoveNext())
            {
                result += string.Format("Helllo {0} {1} {2}",
                        requestStream.Current.Greeting.FirstName,
                        requestStream.Current.Greeting.LastName,
                        Environment.NewLine);
            }

            return new LongGreetResponse() { Result = result };
        }

        public override async Task GreetEveryone(IAsyncStreamReader<GreetEveryoneRequest> requestStream, IServerStreamWriter<GreetEveryoneResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var result = String.Format("Hello {0} {1}",
                                    requestStream.Current.Greeting.FirstName,
                                    requestStream.Current.Greeting.LastName);

                await Console.Out.WriteLineAsync("Received : " + result);
                await responseStream.WriteAsync(new GreetEveryoneResponse() { Result = result });
            }
        }
    }
}
