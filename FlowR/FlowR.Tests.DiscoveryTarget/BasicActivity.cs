using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    [Description("Basic activity description")]
    public class BasicActivityRequest : FlowActivityRequest<BasicActivityResponse>
    {
    }

    public class BasicActivityResponse
    {
    }

    public class BasicActivity : RequestHandler<BasicActivityRequest, BasicActivityResponse>
    {
        protected override BasicActivityResponse Handle(BasicActivityRequest request)
        {
            return new BasicActivityResponse();
        }
    }
}
