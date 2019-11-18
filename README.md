[![Build Status](https://dev.azure.com/andyblackledge/FlowR/_apis/build/status/FlowR-CI?branchName=master)](https://dev.azure.com/andyblackledge/FlowR/_build/latest?definitionId=5&branchName=master)

# Introduction 
FlowR is a .NET framework for building in-process flows based on a series of decoupled exchanges. It leverages the MediatR framework and dependency injection to enable such flows to be defined in a declarative fashion. The result are process flows that can be tested in a straightfoward manner and produce comprehensive logging as standard.

# Hello FlowR

FlowR models in-process flows as a sequence of request/response exchanges. It uses the [MediatR](https://github.com/jbogard/MediatR) framework to despatch the requests to decoupled handlers.

A flow in FlowR is modelled as as a request, a response and a handler. The following example implements a flow that is made up of a single activity that outputs 'Hello' plus a name passed in and returns the text that was outputted.

We first define the request response exchange for the flow. The request must subclass `FlowActivityRequest` and the response `FlowResponse`.

```csharp
    public class SayHelloRequest : FlowActivityRequest<SayHelloResponse>
    {
        public string Name { get; set; }
    }

    public class SayHelloResponse : FlowResponse
    {
        public string OutputtedText { get; set; }
    }
```

We then define the handler for the flow by subclassing `FlowHandler`. This requires us to declare a constructor to receive an `IMediator` instance. This instance is used by `FlowHandler` to despatch the requests to the activities in the flow.

We also provide a `FlowDefinition`. This is where we define the activities in our flow and how they are configured. In this case, we have a single activity that handles `SayGreetingRequest` and responds. It has a single configuration option, which is the greeting to use ('Hello' in this case).

```csharp
    public class SayHelloHandler : FlowHandler<SayHelloRequest, SayHelloResponse>
    {
        public SayHelloHandler(IMediator mediator) : base(mediator)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("SayHello", new FlowActivityDefinition<SayGreetingRequest, SayGreetingResponse>()
                    .SetValue(req => req.Greeting, "Hello"));
        }
    }
```

The activity is defined in a similar way. First we define the request and response

```csharp
    public class SayGreetingRequest : FlowActivityRequest<SayGreetingResponse>
    {
        public string Greeting { get; set; }

        [BoundValue]
        public string Name { get; set; }
    }

    public class SayGreetingResponse
    {
        public string OutputtedText { get; set; }
    }
```

The `Name` property is annotated with the `BoundValue` attribute to indicate that at this property will be bound at runtime to a value. As this is not a flow, the response does not need to subclass `FlowResponse`

With the request and response defined, we can define the handler:

```csharp
    public class SayGreetingHandler : IRequestHandler<SayGreetingRequest, SayGreetingResponse>
    {
        public Task<SayGreetingResponse> Handle(SayGreetingRequest request, CancellationToken cancellationToken)
        {
            var text = $"{request.Greeting} {request.Name}";

            Console.WriteLine(text);

            return Task.FromResult(new SayGreetingResponse { OutputtedText = text });
        }
    }
```

This builds the text to output, outputs it to the console and then returns a response containing the text that was outputted.

To run the flow, we need to register the MediatR assembly and the request/response/handler assembly with an IoC container. In this example, we use the default Microsoft implementation and a MediatR extension method. Once this is done, we create obtain an `IMediator` implementation, create a `SayHelloRequest` instance and send it via the `IMediator` implementation.

```csharp
static async Task Main(string[] args)
{
    var serviceProvider =
        new ServiceCollection()
            .AddMediatR(typeof(SayHelloRequest).Assembly)
            .BuildServiceProvider();

    var mediator = serviceProvider.GetService<IMediator>();

    var response = await mediator.Send(new SayHelloRequest { Name = "FlowR" });

    Console.WriteLine($"response.Text: {response.OutputtedText}");
    Console.WriteLine($"response.Trace: {response.Trace}");
}
```

The result is as follows:

```
Hello FlowR
response.Text: Hello FlowR
response.Trace: SayHello
```

When the `SayHelloRequest` is handled, the sequence of events is as follows:

1. An empty flow values dictionary is created
1. The flow value 'Name' is set to the value of the `SayHelloRequest` `Name` property, i.e. 'FlowR'
1. A `SayGreetingRequest` instance is created
1. The `Greeting` property is set to the design-time value 'Hello'
1. The `Name` property is bound to the flow value 'Name', i.e. 'FlowR'
1. The `SayGreetingRequest` is despatched via the `IMediator` instance
1. The `SayGreetingHandler` is invoked, the console is written to and a `SayGreetingResponse` returned
1. The flow value 'OutputtedText' is set to the value of the `SayGreetingResponse` `OutputtedText` property, i.e. 'Hello FlowR'
1. A `SayHelloResponse` instance is created
1. The `OutputtedText` property is set to the flow value 'OutputtedText', i.e. 'Hello FlowR'

The `response.Trace` is a property of `FlowResponse` and allows a flow to return a summary of the flow executed. In this case, as we only have one activity, it simply returns the name we gave the activity in the `FlowDefinition`.