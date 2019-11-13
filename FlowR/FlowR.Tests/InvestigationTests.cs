using FlowR.Tests.Domain.Investigations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FlowR.Tests
{
    public class InvestigationTests : TestBase
    {
        public InvestigationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Can_use_ASP_NET_Core_DI_container()
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddDebugLogging(this)
                    .AddSingleton<IFooService, FooService>()
                    .AddSingleton<IBarService, BarService>()
                    .BuildServiceProvider();

            var logger = serviceCollection.GetLogger(this);

            logger.LogDebug("Starting application");

            var bar = serviceCollection.GetService<IBarService>();
            bar.DoSomeRealWork();

            logger.LogDebug("All done!");
        }

        [Fact]
        public async void Can_use_MediatR_to_invoke_a_handler()
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddDebugLogging(this)
                    .AddMediatR(typeof(Ping).Assembly)
                    .BuildServiceProvider();

            var logger = serviceCollection.GetLogger(this);
            var mediator = serviceCollection.GetService<IMediator>();

            var response = await mediator.Send(new Ping());

            logger.LogDebug($"response='{response}'");

            Assert.Equal("Pong", response);
        }

        [Fact]
        public async void Can_send_MediatR_request_by_reflection()
        {
            var serviceProvider =
                new ServiceCollection()
                    .AddDebugLogging(this)
                    .AddMediatR(typeof(PingHandler).Assembly)
                    .BuildServiceProvider();

            var logger = serviceProvider.GetLogger(this);
            var mediator = serviceProvider.GetService<IMediator>();

            var mediatorSend = typeof(Mediator).GetMethod("Send");

            var mediatorSendGeneric = mediatorSend.MakeGenericMethod(typeof(string));

            var activityResponse =
                await (dynamic)mediatorSendGeneric.Invoke(mediator, new object[] { new Ping(), new CancellationToken() });

            if (activityResponse is string stringResponse)
            {
                logger.LogDebug($"response='{stringResponse}'");

                Assert.Equal("Pong", stringResponse);
            }
            else
            {
                Assert.True(false, $"response was type: {activityResponse.GetType().FullName}");
            }
        }

        [Fact]
        public async void Can_override_base_implementation_in_MS_DI()
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddDebugLogging(this)
                    .AddMediatR(typeof(PingHandler).Assembly);

            // Inject our overridden implementation of the handler
            serviceCollection.AddTransient(typeof(IRequestHandler<Ping, string>), typeof(PingKongHandler));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetLogger(this);
            var mediator = serviceProvider.GetService<IMediator>();

            var response = await mediator.Send(new Ping());

            logger.LogDebug($"response='{response}'");

            Assert.Equal("Kong", response);
        }

        public class PingKongHandler : IRequestHandler<Ping, string>
        {
            public Task<string> Handle(Ping request, CancellationToken cancellationToken)
            {
                return Task.FromResult("Kong");
            }
        }

        [Fact]
        public async void Can_override_base_implementation_with_mock_in_MS_DIAsync()
        {
            var serviceProvider =
                new ServiceCollection()
                    .AddDebugLogging(this)
                    .AddMediatR(typeof(Ping).Assembly)
                    .MockRequestHandler<Ping, string>((rq, ct) => "Kong")
                    .BuildServiceProvider(this, out var mediator, out var logger);

            var response = await mediator.Send(new Ping());

            logger.LogDebug($"response='{response}'");

            Assert.Equal("Kong", response);
        }

        [Fact]
        public void Can_extract_property_name_from_lambda()
        {
            // https://www.automatetheplanet.com/get-property-names-using-lambda-expressions/

            var valuePropertyName = GetMemberName<LambdaClass>(c => c.IntProperty);
            Assert.Equal(nameof(LambdaClass.IntProperty), valuePropertyName);

            var referencePropertyName = GetMemberName<LambdaClass>(c => c.StringProperty);
            Assert.Equal(nameof(LambdaClass.StringProperty), referencePropertyName);
        }

        class LambdaClass
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }

        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            // Reference type property or field
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            // Property, field of method returning value type
            if (expression is UnaryExpression unaryExpression)
            {
                return GetMemberName(unaryExpression);
            }

            throw new ArgumentException("Invalid property expression");
        }

        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression methodExpression)
            {
                return methodExpression.Method.Name;
            }

            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        }
    }
}
