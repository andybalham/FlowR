using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class ConfigurableActivityRequest : FlowActivityRequest<ConfigurableActivityResponse>
    {
        [DesignTimeValue]
        public string SetValue { get; set; }

        public string BoundValue { get; set; }
    }

    public class ConfigurableActivityResponse
    {
        public string OutputValue { get; set; }
    }

    public class ConfigurableActivity : RequestHandler<ConfigurableActivityRequest, ConfigurableActivityResponse>
    {
        protected override ConfigurableActivityResponse Handle(ConfigurableActivityRequest request)
        {
            return new ConfigurableActivityResponse();
        }
    }
}
