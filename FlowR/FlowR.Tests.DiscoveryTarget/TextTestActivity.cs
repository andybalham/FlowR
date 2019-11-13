using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class TextTestActivityRequest : FlowActivityRequest<TextTestActivityResponse>
    {
        public string SetValue { get; set; }

        public override string GetText()
        {
            return $"SetValue={this.SetValue}";
        }
    }

    public class TextTestActivityResponse
    {
    }

    public class TextTestActivity : RequestHandler<TextTestActivityRequest, TextTestActivityResponse>
    {
        protected override TextTestActivityResponse Handle(TextTestActivityRequest request)
        {
            return new TextTestActivityResponse();
        }
    }
}
