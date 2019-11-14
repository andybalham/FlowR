using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FlowR.Tests
{
    static class TestExtensions
    {
        public static ILogger<T> GetLogger<T>(this ServiceProvider serviceProvider)
        {
            return serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger<T>();
        }

        public static IServiceCollection AddDebugLogging(this IServiceCollection serviceCollection, TestBase testBase,
            StringBuilder loggingOutputBuilder = null)
        {
            serviceCollection
                .AddLogging(configure =>
                {
                    configure.SetMinimumLevel(LogLevel.Debug);
                    configure.AddProvider(testBase);
                    if (loggingOutputBuilder != null)
                    {
                        configure.AddProvider(new CaptureLoggerProvider(loggingOutputBuilder));
                    }
                });

            return serviceCollection;
        }

        public static IServiceCollection MockRequestHandler<TRq, TRs>(this IServiceCollection serviceCollection, 
                Func<TRq, CancellationToken, TRs> mockHandler)
            where TRq : IRequest<TRs>
        {
            var handlerMock = new Mock<IRequestHandler<TRq, TRs>>();

            handlerMock.Setup(h =>
                h.Handle(It.IsAny<TRq>(), It.IsAny<CancellationToken>()))
                    .Returns((TRq req, CancellationToken ct) => Task.FromResult(mockHandler(req, ct)));

            serviceCollection.AddSingleton(typeof(IRequestHandler<TRq, TRs>), handlerMock.Object);

            return serviceCollection;
        }

        public static ServiceProvider BuildServiceProvider(this IServiceCollection serviceCollection, out IMediator mediator)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            mediator = serviceProvider.GetService<IMediator>();

            return serviceProvider;
        }

        public static ServiceProvider BuildServiceProvider(this IServiceCollection serviceCollection, TestBase testBase, 
            out IMediator mediator, out ILogger logger)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            mediator = serviceProvider.GetService<IMediator>();
            logger = serviceProvider.GetLogger(testBase);

            return serviceProvider;
        }
    }
}
