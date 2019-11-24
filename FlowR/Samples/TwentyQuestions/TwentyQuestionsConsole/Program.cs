using System;
using System.Linq;
using FlowR;
using FlowR.Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwentyQuestions.Core;

namespace TwentyQuestions
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(TwentyQuestionsRequest).Assembly)
                    .AddLogging(builder => { builder.AddConsole(); })
                    .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug)
                    .AddTransient(typeof(IFlowLogger<>), typeof(CoreFlowLogger<>))
                    .AddSingleton<IConsoleService>(new ConsoleService());

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var mediator = serviceProvider.GetService<IMediator>();

                var response =
                    mediator.Send(new TwentyQuestionsRequest())
                        .GetAwaiter().GetResult();

                Console.WriteLine("****************************************************");
                Console.WriteLine($"Guess: {response.Guess}");
                Console.WriteLine($"Trace: {response.Trace}");
                Console.WriteLine("****************************************************");
            }
        }
    }
}
