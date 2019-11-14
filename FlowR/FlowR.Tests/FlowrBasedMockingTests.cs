using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlowR.Microsoft.Extensions.Logging;
using FlowR.StepLibrary.Activities;
using FlowR.Tests.Domain.FlowrBasedMockingTests;
using FlowR.Tests.Domain.FlowTests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace FlowR.Tests
{
    public class FlowRBasedMockingTests : TestBase
    {
        public FlowRBasedMockingTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(false, "A", "B", "C")]
        [InlineData(true, "Z", "Z", "Z")]
        public async void Can_mock_activity_by_request_type_only(bool isMocked, 
            string expectedValue1, string expectedValue2, string expectedValue3)
        {
            var (mediator, _) = GetMediator<MockActivityViaFlowRFlowRequest>();

            var flowContext = new FlowContext(null, null);

            if (isMocked)
            {
                flowContext
                    .MockActivity<SetStringFlowValueRequest, SetStringFlowValueResponse>(
                        request => new SetStringFlowValueResponse { Output = "Z" });
            }

            var response = await mediator.Send(new MockActivityViaFlowRFlowRequest { FlowContext = flowContext});

            Assert.Equal(expectedValue1, response.Value1);
            Assert.Equal(expectedValue2, response.Value2);
            Assert.Equal(expectedValue3, response.Value3);
        }

        [Theory]
        [InlineData(false, null, "A", "B", "C")]
        [InlineData(true, "SetValue1", "Z", "B", "C")]
        [InlineData(true, "SetValue2", "A", "Z", "C")]
        [InlineData(true, "SetValue3", "A", "B", "Z")]
        public async void Can_mock_activity_by_request_type_and_override_key(bool isMocked,
            string overrideKey, string expectedValue1, string expectedValue2, string expectedValue3)
        {
            var (mediator, _) = GetMediator<MockActivityViaFlowRFlowRequest>();

            var flowContext = new FlowContext(null, null);

            if (isMocked)
            {
                flowContext
                    .MockActivity<SetStringFlowValueRequest, SetStringFlowValueResponse>(
                        overrideKey, request => new SetStringFlowValueResponse { Output = "Z" });
            }

            var response = await mediator.Send(new MockActivityViaFlowRFlowRequest { FlowContext = flowContext });

            Assert.Equal(expectedValue1, response.Value1);
            Assert.Equal(expectedValue2, response.Value2);
            Assert.Equal(expectedValue3, response.Value3);
        }
    }
}
