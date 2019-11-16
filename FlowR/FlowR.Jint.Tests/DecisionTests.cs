using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FlowR.Jint.Tests
{
    public class DecisionTests
    {
        private class TestClass
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }
        }

        [Theory]
        [InlineData("PropertyValue1", 0)]
        [InlineData("PropertyValue2", 1)]
        [InlineData("PropertyValue3", 2)]
        public void Can_switch_on_flow_value(string stringPropertyValue, int expectedIndex)
        {
            var decisionRequest = 
                new FlowValueScriptDecisionRequest<string> { SwitchValueScript = "value.StringProperty" };
            decisionRequest.AddBranch(new[] { "PropertyValue1" });
            decisionRequest.AddBranch(new[] { "PropertyValue2" });
            decisionRequest.AddBranch(null);
            decisionRequest.FlowValue = new TestClass { StringProperty = stringPropertyValue };

            var actualIndex = decisionRequest.GetMatchingBranchIndex();

            Assert.Equal(expectedIndex, actualIndex);
        }

        [Theory]
        [InlineData(9, 0)]
        [InlineData(19, 1)]
        [InlineData(20, 2)]
        public void Can_switch_on_branch_evaluation(int intPropertyValue, int expectedIndex)
        {
            var decisionRequest = new EvaluateBranchScriptDecisionRequest();
            decisionRequest.AddBranch(new[] { "value.IntProperty < 10" });
            decisionRequest.AddBranch(new[] { "value.IntProperty >= 10 && value.IntProperty < 20" });
            decisionRequest.AddBranch(null);
            decisionRequest.SwitchValue = new TestClass { IntProperty = intPropertyValue };

            var actualIndex = decisionRequest.GetMatchingBranchIndex();

            Assert.Equal(expectedIndex, actualIndex);
        }
    }
}
