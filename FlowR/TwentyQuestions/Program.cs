using System;
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
                    .AddLogging(builder => { builder.AddConsole(); })
                    .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug)
                    .AddTransient(typeof(IFlowLogger<>), typeof(CoreFlowLogger<>));

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var mediator = serviceProvider.GetService<IMediator>();

                var response =
                    mediator.Send(new TwentyQuestionsRequest())
                        .GetAwaiter().GetResult();
            }
        }
    }
}
