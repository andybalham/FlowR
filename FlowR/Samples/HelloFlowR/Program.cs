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
            var serviceProvider =
                new ServiceCollection()
                    .AddMediatR(typeof(SayHelloRequest).Assembly)
                    .BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();

            var response = mediator.Send(new SayHelloRequest { Name = "FlowR" }).GetAwaiter().GetResult();

            Console.WriteLine($"response.Text: {response.OutputtedText}");
            Console.WriteLine($"response.Trace: {response.Trace}");
        }
    }
}
