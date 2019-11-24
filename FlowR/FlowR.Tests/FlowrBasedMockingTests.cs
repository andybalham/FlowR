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
        [InlineData(true, "Set_value_1_to_A", "Z", "B", "C")]
        [InlineData(true, "Set_value_2_to_B", "A", "Z", "C")]
        [InlineData(true, "Set_value_3_to_C", "A", "B", "Z")]
        public async void Can_mock_activity_by_request_type_and_name(bool isMocked,
            string stepName, string expectedValue1, string expectedValue2, string expectedValue3)
        {
            var (mediator, _) = GetMediator<MockActivityViaFlowRFlowRequest>();

            var flowContext = new FlowContext(null, null);

            if (isMocked)
            {
                flowContext
                    .MockActivity<SetStringFlowValueRequest, SetStringFlowValueResponse>(
                        stepName, request => new SetStringFlowValueResponse { Output = "Z" });
            }

            var response = await mediator.Send(new MockActivityViaFlowRFlowRequest { FlowContext = flowContext });

            Assert.Equal(expectedValue1, response.Value1);
            Assert.Equal(expectedValue2, response.Value2);
            Assert.Equal(expectedValue3, response.Value3);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void Can_only_mock_root_flow(bool isMocked)
        {
            // Arrange

            var (mediator, _) = GetMediator<MockActivityViaFlowRFlowRequest>();

            var flowContext = new FlowContext(null, null);

            if (isMocked)
            {
                flowContext
                    .MockActivity<CanMockOnlyRootSetValueRequest, CanMockOnlyRootSetValueResponse>(
                        request => new CanMockOnlyRootSetValueResponse { Value = false } );
            }

            // Act

            var response = await mediator.Send(new CanMockOnlyRootFlowRequest { FlowContext = flowContext });

            // Assert

            Assert.True(response.Value, "response.Value");
        }
    }
}
