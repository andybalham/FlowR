using MediatR;

namespace FlowR.Tests.Domain.OverrideProviderTests
{
    public class OverridableActivity : RequestHandler<OverridableActivityRequest, OverridableActivityResponse>
    {
        protected override OverridableActivityResponse Handle(OverridableActivityRequest request)
        {
            return new OverridableActivityResponse
            {
                OverridableOutputValue = request.OverridableInputValue,
                NonOverridableOutputValue = request.NonOverridableInputValue,
            };
        }
    }
}