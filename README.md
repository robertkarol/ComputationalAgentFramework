# CAF

## Overview

__Computational Agent Framework (CAF)__ is a C# framework for developing computational entities that have a state machine architecture for the individual computation each performs, and externally behave in a dataflow fashion. In other words, you can write programs that are supposed to be a bunch of entities communicating with eachother in a well-determined manner.

## Requirements

- **.NET 10.0 SDK** or later
- **Visual Studio 2022 (17.12+)** or any compatible IDE/editor
- **Git** for source control

## Getting started

### Prerequisites

Ensure you have the .NET 10.0 SDK installed:

```bash
dotnet --version
# Should show 10.0.x or later
```

If you need to install it, download from: https://dotnet.microsoft.com/download/dotnet/10.0

### Get the code working

This repository contains a Visual Studio solution with two projects:
- a project containing the proposed framework
- a project providing basic examples of how to write programs that use the framework

Clone this repository and build the solution:

```bash
git clone https://github.com/robertkarol/ComputationalAgentFramework
cd ComputationalAgentFramework
dotnet build
```

### Running the Examples

```bash
dotnet run --project Examples\Examples.csproj
```

### Architecture

A __Computational Agent (CA)__ is the centric concept of this framework. It a state machine that contains for now only three states: _Initialize_, _Execute_, and _Finish_. _Execute_ is the heart of our CA, while the other two states are preparing/cleaning-up things for a successful execution. Beside having these states, a CA must consume and produce data (of a specific type), phases which take place implictly during the execution. All the previous logic is executed by the framework, while the user only needs to provide the actual code that will get executed as part of handling each phase encountered. So, just have a class inheriting from our abstract `ComputationalAgent` and implement things until the compiler stops crying.

A program is comprised of several CAs that talk to eachother. For now, the framework supports only single source data consumption, but a produced data can be consumed by multiple CAs. How is the link between agents made? The framework provides a very simple mechanism base on C#'s attributes. Let's take an example first. You see that we have in our examples project three agents: `Agent1`, `Agent2`, and `Agent3`. The data flows in the mentioned order. So, that means that `Agent2` should get data from `Agent1`. That is simply accomplished by having `Agent2` attributed with `[ConsumesFrom(typeof(Agent1))]`. It's as simple as this, but be aware: and agent must consume data from agents producing the same data it cosumes, because otherwise a runtime error will be encountered.

So, until now we can define CAs and link them. But how does the execution take place? The only job you had is done! You only need to intatiate the agent classes, add them to a `Runner`, and run them using a scheduling strategy that will come as an additional controling mechanism (on top of the dataflow execution). Another aspect worth mentioning in that for now only one instance of each concrete agent definition is allowed.
The only part that's not straightforward and needs to be explained is the execution. From the scheduling perspective, the only two supported for now are single runs (`Schedule.RunOnce`) and forever runs (`Schedule.RunIndefinitely`), which control the execution of the whole system: we either run once an stop the program or we keep executing all agents on and on. The agent execution happens in the utterly mentioned dataflow manner. For this to be possible, we need to find an execution order where when an agent executes, it has all the necessary data. A bright computer scientist will instantly recognize that this is a topological sort problem, so this is the algorithm data solves provides the dataflow capabilities. Another constraint this implies is that we don't allow cycles in our CA linking.

### Other limitations

There severe other well known limitations, beside the others mentioned in the previous section:
- only agents living in the same thread and process are supported
- no parallel execution when possible (e.g decoupled sub-graphs of CAs)
- no control over the state transitioning
- no dynamic reconfiguration
