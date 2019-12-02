[![Build Status](https://dev.azure.com/andyblackledge/FlowR/_apis/build/status/FlowR-CI?branchName=master)](https://dev.azure.com/andyblackledge/FlowR/_build/latest?definitionId=5&branchName=master)

# Introduction 
FlowR is a .NET framework for building in-process flows based on a series of decoupled exchanges. It leverages the MediatR framework and dependency injection to enable such flows to be defined in a declarative fashion. The result are process flows that can be tested in a straightfoward manner and produce comprehensive logging as standard.

# Hello FlowR

FlowR models in-process flows as a sequence of request/response exchanges. It uses the [MediatR](https://github.com/jbogard/MediatR) framework to despatch the requests to decoupled handlers.

A flow in FlowR is modelled as as a request, a response and a handler. The following example implements a flow that is made up of a single activity that outputs 'Hello' plus a name passed in and returns the text that was outputted.

We first define the request response exchange for the flow.

```csharp
    public class SayHelloRequest : FlowActivityRequest<SayHelloResponse>
    {
        public string Name { get; set; }
    }

    public class SayHelloResponse
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

        protected override void ConfigureDefinition(
            FlowDefinition<SayHelloRequest, SayHelloResponse> flowDefinition)
        {
            flowDefinition
                .Do("SayHello", new FlowActivityDefinition<SayGreetingRequest, SayGreetingResponse>()
                    .SetValue(req => req.Greeting, "Hello"));
        }
    }
```

The activity is defined in a similar way. First we define the request and response:

```csharp
    public class SayGreetingRequest : FlowActivityRequest<SayGreetingResponse>
    {
        public string Greeting { get; set; }

        public string Name { get; set; }
    }

    public class SayGreetingResponse
    {
        public string OutputtedText { get; set; }
    }
```

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

To run the flow, we need to register the MediatR assembly and the request/response/handler assembly with an IoC container. In this example, we use the default Microsoft implementation and a MediatR extension method. We use the resulting service provider to obtain an `IMediator` implementation, create a `SayHelloRequest` instance and send it via the `IMediator` implementation.

```csharp
    static void Main(string[] args)
    {
        var mediator =
            new ServiceCollection()
                .AddMediatR(typeof(SayHelloRequest).Assembly)
                .BuildServiceProvider()
                .GetService<IMediator>();

        var response = 
            mediator.Send(new SayHelloRequest { Name = "FlowR" })
                .GetAwaiter().GetResult();

        Console.WriteLine($"response.OutputtedText: {response.OutputtedText}");
    }
```

The result is as follows:
    
```
Hello FlowR
response.OutputtedText: Hello FlowR
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
