using System;
using System.Linq;
using FlowR;
using FlowR.Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TwentyQuestions
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(TwentyQuestionsRequest).Assembly)
                    .AddMediatR(typeof(FlowDiscoveryRequest).Assembly)
                    .AddLogging(builder => { builder.AddConsole(); })
                    .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug)
                    .AddTransient(typeof(IFlowLogger<>), typeof(CoreFlowLogger<>))
                    .AddSingleton<IConsoleService>(new ConsoleService());

            FlowDiscovery.RegisterFlowTypes(typeof(TwentyQuestionsRequest).Assembly,
                (intType, impType) => serviceCollection.AddSingleton(intType, impType));

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var mediator = serviceProvider.GetService<IMediator>();

                if (args.Length == 0)
                {
                    var response =
                        mediator.Send(new TwentyQuestionsRequest())
                            .GetAwaiter().GetResult();

                    Console.WriteLine("****************************************************");
                    Console.WriteLine($"Guess: {response.Guess}");
                    Console.WriteLine($"Trace: {response.Trace}");
                    Console.WriteLine("****************************************************");
                }
                else
                {
                    var flowDiscoveryResponse =
                        mediator.Send(new FlowDiscoveryRequest())
                            .GetAwaiter().GetResult();

                    var twentyQuestionsFlow =
                        flowDiscoveryResponse.Flows.First(f => 
                            f.Request.RequestType == typeof(TwentyQuestionsRequest));

                    Console.WriteLine("****************************************************");
                    Console.WriteLine(twentyQuestionsFlow.GetDotNotation());
                    Console.WriteLine("****************************************************");

                }
            }
        }
    }
}
