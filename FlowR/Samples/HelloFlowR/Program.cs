using System;
using System.Threading.Tasks;
using FlowR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HelloFlowR
{
    class Program
    {
        static void Main(string[] args)
        {
            var mediator =
                new ServiceCollection()
                    .AddMediatR(typeof(SayHelloRequest).Assembly)
                    .BuildServiceProvider()
                    .GetService<IMediator>();

            var response = 
                mediator.Send(new SayHelloRequest { Name = "FlowR" })
                    .GetAwaiter().GetResult();

            Console.WriteLine($"response.OutputtedText: {response.OutputtedText}");
        }
    }
}
