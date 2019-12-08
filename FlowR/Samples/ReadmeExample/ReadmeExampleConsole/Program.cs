using System;
using System.Linq;
using FlowR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ReadmeExampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(MyFlowRequest).Assembly)
                    .AddMediatR(typeof(FlowDiscoveryRequest).Assembly)
                ;

            typeof(MyFlowRequest).Assembly.RegisterFlowTypes(
                (intType, impType) => serviceCollection.AddSingleton(intType, impType));

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var mediator = serviceProvider.GetService<IMediator>();

                var response =
                    mediator.Send(new FlowDiscoveryRequest())
                        .GetAwaiter().GetResult();

                var flowDiagram = response.Flows.FirstOrDefault(f => f.Request.RequestType == typeof(MyFlowRequest));

                Console.WriteLine(flowDiagram?.GetDotDiagram());
            }
        }
    }
}
