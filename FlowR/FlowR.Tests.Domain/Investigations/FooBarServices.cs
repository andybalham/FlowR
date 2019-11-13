using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR.Tests.Domain.Investigations
{
    public interface IFooService
    {
        void DoThing(int number);
    }

    public interface IBarService
    {
        void DoSomeRealWork();
    }

    public class BarService : IBarService
    {
        private readonly ILogger<BarService> _logger;
        private readonly IFooService _fooService;

        public BarService(IFooService fooService, ILogger<BarService> logger)
        {
            _logger = logger;

            _fooService = fooService;
        }

        public void DoSomeRealWork()
        {
            _logger.LogInformation("Starting to do some real work");

            for (int i = 0; i < 10; i++)
            {
                _fooService.DoThing(i);
            }

            _logger.LogInformation("Finished doing some real work");
        }
    }

    public class FooService : IFooService
    {
        private readonly ILogger<FooService> _logger;
        public FooService(ILogger<FooService> logger)
        {
            _logger = logger;
        }

        public void DoThing(int number)
        {
            _logger.LogInformation($"Doing the thing {number}");
        }
    }
}
