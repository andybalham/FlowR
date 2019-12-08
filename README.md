[![Build Status](https://dev.azure.com/andyblackledge/FlowR/_apis/build/status/FlowR-CI?branchName=master)](https://dev.azure.com/andyblackledge/FlowR/_build/latest?definitionId=5&branchName=master)

# Introduction 
FlowR is a .NET Standard 2.0 framework for building in-process flows based on a series of decoupled exchanges. It leverages the MediatR framework and dependency injection to enable flows to be defined declaratively using a fluent API.

# Fluent API

The following example gives a flavour of how a flow is defined using the fluent API. 

A flow is made up of a sequence of activity and decision steps. Activities perform processing, whilst decisions control the path through the flow.

```csharp
    public class MyFlowHandler : FlowHandler<MyFlowRequest, MyFlowResponse>
    {
        public MyFlowHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(
            FlowDefinition<MyFlowRequest, MyFlowResponse> flowDefinition)
        {
            flowDefinition

                .Do("Activity1", 
                    new FlowActivityDefinition<Activity1Request, Activity1Response>())
                
                .Check("FlowValue", 
                    new FlowDecisionDefinition<FlowValueDecision<string>, string>()
                        .BindInput(req => req.SwitchValue, "FlowValue"))
                .When("2").Goto("Activity2")
                .Else().Goto("Activity3")

                .Do("Activity2", 
                    new FlowActivityDefinition<Activity2Request, Activity2Response>())
                .End()

                .Do("Activity3", 
                    new FlowActivityDefinition<Activity3Request, Activity3Response>());
        }
    }
```

# Visualisation

FlowR can visualise the defined flows in [DOT graph description language](https://en.wikipedia.org/wiki/DOT_(graph_description_language)), which can be rendered by many open source packages. For example, the image below was generated from the Twenty Questions sample:

<img src="https://github.com/andybalham/FlowR/blob/master/FlowR/Samples/TwentyQuestions/FlowDiagram.png" width="640">

# Samples

There are a number of sample projects that demonstrate the features of FlowR

* [Hello FlowR](https://github.com/andybalham/FlowR/wiki/Hello-FlowR-Sample) - A console application that demonstrates a simple flow containing a single activity that combines design-time and run-time values to say hello.

* [Twenty Questions](https://github.com/andybalham/FlowR/wiki/Twenty-Questions-Sample) - A console application that demonstrates a multi-branch flow, how it can be tested and how it can be visualised.

* [Business Example](https://github.com/andybalham/FlowR/tree/master/FlowR/Samples/BusinessExample) is a web API that makes loan decisions based using a process implemented using FlowR. It demonstrates the following features of FlowR:

  * Flow logic can be tested in isolation
  * Activities can be tested in isolation
  * Flows can be visualised
  * Flows have comprehensive logging by default
