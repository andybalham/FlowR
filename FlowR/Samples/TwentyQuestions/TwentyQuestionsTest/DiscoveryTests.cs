using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using FlowR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TwentyQuestions.Core;
using Xunit;

namespace TwentyQuestions.Test
{
    public class DiscoveryTests
    {
        [Fact]
        public void Can_output_flow_diagrams()
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(FlowDiscoveryRequest).Assembly);

            typeof(TwentyQuestionsRequest).Assembly.RegisterFlowTypes(
                (intType, impType) => serviceCollection.AddSingleton(intType, impType));

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var mediator = serviceProvider.GetService<IMediator>();

                var response =
                    mediator.Send(new FlowDiscoveryRequest())
                        .GetAwaiter().GetResult();

                foreach (var flow in response.Flows)
                {
                    File.WriteAllText($"{flow.Request.RequestType.Name}.dot", flow.GetDotDiagram());
                }
            }
        }
    }
}
