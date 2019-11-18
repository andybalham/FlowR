using System;
using System.Threading.Tasks;
using FlowR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HelloFlowR
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider =
                new ServiceCollection()
                    .AddMediatR(typeof(SayHelloRequest).Assembly)
                    .BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();

            var response = await mediator.Send(new SayHelloRequest { Name = "FlowR" });

            Console.WriteLine($"response.Text: {response.OutputtedText}");
            Console.WriteLine($"response.Trace: {response.Trace}");
        }
    }
}
