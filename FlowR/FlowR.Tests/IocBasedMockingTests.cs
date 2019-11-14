using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FlowR.Microsoft.Extensions.Logging;
using FlowR.StepLibrary.Activities;
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
    }
}
