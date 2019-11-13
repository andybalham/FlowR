using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FlowR.Microsoft.Extensions.Logging;
using FlowR.StepLibrary.Activities;
using FlowR.StepLibrary.Decisions;
using FlowR.Tests.Domain.FlowTests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FlowR.Tests
{
    public class IocBasedMockingTests : TestBase
    {
        public IocBasedMockingTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async void Activity_can_be_mocked()
        {
            const string mockedOutputValue = "MockedOutputValue";

            new ServiceCollection()
                .AddMediatR(typeof(ActivityOutputFlowRequest).Assembly)
                .MockRequestHandler<ActivityOutputFlowRequest, ActivityOutputFlowResponse>((req, ct) =>
                    new ActivityOutputFlowResponse { OutputValue = mockedOutputValue })
                .BuildServiceProvider(this, out var mediator, out _);

            var response = await mediator.Send(new ActivityOutputFlowRequest());

            Assert.Equal(mockedOutputValue, response.OutputValue);
        }

        [Theory]
        [InlineData(false, 1, "X")]
        [InlineData(false, 2, "Y")]
        [InlineData(false, 3, "Z")]
        [InlineData(true, 1, "Z")]
        [InlineData(true, 2, "Z")]
        [InlineData(true, 3, "Z")]
        public async void Decisions_can_be_mocked(bool isMocked, int intValue, string expectedBranchValue)
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(IFlowHandler).Assembly)
                    .AddMediatR(typeof(SetBoolFlowValueRequest).Assembly)
                    .AddMediatR(typeof(MockedDecisionFlowRequest).Assembly);

            if (isMocked)
            {
                serviceCollection =
                    serviceCollection
                        .MockDecisionHandler<IntFlowValueDecision>((req, ct) => req.Branches.Count() - 1);
            }

            serviceCollection
                .BuildServiceProvider(this, out var mediator, out _);

            var request = new MockedDecisionFlowRequest { IntValue = intValue };

            var response = await mediator.Send(request);

            Assert.Equal(expectedBranchValue, response.BranchValue);
        }
    }
}
