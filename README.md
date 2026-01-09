# CAF

## Overview

__Computational Agent Framework (CAF)__ is a C# framework for developing computational entities that have a state machine architecture for the individual computation each performs, and externally behave in a dataflow fashion. In other words, you can write programs that are a bunch of entities communicating with each other in a well-determined manner.

## Requirements

- **.NET 10.0 SDK** or later
- **Visual Studio 2022 (17.12+)** or any compatible IDE/editor
- **Git** for source control

## Getting Started

### Prerequisites

Ensure you have the .NET 10.0 SDK installed:

```bash
dotnet --version
# Should show 10.0.x or later
```

If you need to install it, download from: https://dotnet.microsoft.com/download/dotnet/10.0

### Get the Code Working

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

## Architecture

### Computational Agents

A __Computational Agent (CA)__ is the centric concept of this framework. It is a state machine that contains three states: _Initialize_, _Execute_, and _Finish_. _Execute_ is the heart of our CA, while the other two states are preparing/cleaning-up things for a successful execution. Besides having these states, a CA must consume and produce data (of a specific type), phases which take place implicitly during the execution. All the previous logic is executed by the framework, while the user only needs to provide the actual code that will get executed as part of handling each phase encountered. Just have a class inheriting from our abstract `ComputationalAgent` and implement the required methods.

### Agent Communication

A program is comprised of several CAs that talk to each other. For now, the framework supports only single source data consumption, but produced data can be consumed by multiple CAs. How is the link between agents made? The framework provides a very simple mechanism based on C#'s attributes. For example, if you have three agents: `Agent1`, `Agent2`, and `Agent3`, and data flows in the mentioned order, then `Agent2` should get data from `Agent1`. That is simply accomplished by having `Agent2` attributed with `[ConsumesFrom(typeof(Agent1))]`. An agent must consume data from agents producing the same data type it consumes, because otherwise a runtime error will be encountered.

### Execution

Once you define CAs and link them, instantiate the agent classes, add them to a `Runner` (or `ParallelRunner`), and run them using a scheduling strategy. For now, only one instance of each concrete agent definition is allowed.

From the scheduling perspective, the only two strategies supported are single runs (`Schedule.RunOnce`) and forever runs (`Schedule.RunIndefinitely`), which control the execution of the whole system: we either run once and stop the program or we keep executing all agents repeatedly.

Agent execution happens in a dataflow manner. A topological sort algorithm provides the dataflow capabilities by finding an execution order where when an agent executes, it has all the necessary data. This implies that we don't allow cycles in our CA linking.

## Runner vs ParallelRunner

The framework provides two execution engines:

### Runner (Sequential Execution)

The original `Runner` executes agents sequentially in topological order:
- **Execution Model**: Single-threaded, sequential
- **Best For**: Debugging, deterministic execution, linear pipelines
- **Thread Safety**: Single-threaded, inherently thread-safe
- **Performance**: Agents execute one at a time

### ParallelRunner (Parallel Execution)

The `ParallelRunner` executes agents in parallel while respecting dependencies:
- **Execution Model**: Multi-threaded, wave-based parallel execution
- **Best For**: Performance-critical workloads, agent graphs with independent branches
- **Thread Safety**: Uses `ConcurrentDictionary` for thread-safe data sharing
- **Performance**: Independent agents execute simultaneously

#### Wave-Based Execution

ParallelRunner executes agents in parallel "waves":
- **Wave 1**: All agents with no dependencies execute in parallel
- **Wave 2**: All agents whose dependencies completed in Wave 1 execute in parallel
- **Wave N**: Continues until all agents have executed

#### When to Use Each Runner

**Use ParallelRunner When:**
- You have independent agent branches
- Performance is critical
- Agents are CPU or I/O intensive
- You want to leverage multi-core processors

**Use Runner When:**
- You need deterministic sequential execution
- Debugging and want predictable order
- Agents have side effects requiring strict ordering
- The agent graph is linear (no parallelism opportunity)

#### Performance Benefits

**Example Scenario:**
Given an agent graph where Agent1 has no dependencies, and both Agent2 and Agent3 depend on Agent1 but not each other:

- **Sequential Runner**: 3 seconds (Agent1=1s, Agent2=1s, Agent3=1s)
- **Parallel Runner**: 2 seconds (Wave 1: Agent1=1s, Wave 2: Agent2+Agent3 parallel=1s)
- **Performance improvement**: 33% faster

**Loose Agents:**
Agents with no dependencies and no dependents (loose agents) will execute in the first wave alongside other independent agents, maximizing parallelism.

#### API Compatibility

Both runners have identical APIs - migration is as simple as:

- Before: `var runner = new Runner();`
- After: `var runner = new ParallelRunner();`

All other methods (`AddAgent()`, `Run()`) remain the same.

## Performance Metrics

### Time Complexity by Graph Type

| Graph Type | Runner | ParallelRunner | Speedup |
|------------|--------|----------------|---------|
| Linear (n agents) | O(n) | O(n) | 1x |
| Full fan-out (1?n) | O(n+1) | O(2) | ~n/2x |
| Balanced tree (depth d) | O(2^d - 1) | O(d) | ~2^(d-1)/d |

### Execution Flow Examples

**Linear Graph: A ? B ? C**
- Runner: 3 steps
- ParallelRunner: 3 waves (same as sequential, no benefit)

**Fan-Out Graph: A ? [B, C, D]**
- Runner: 4 steps
- ParallelRunner: 2 waves (2x faster)

**Complex DAG: A ? [B, C] ? D**
- Runner: 4 steps
- ParallelRunner: 3 waves (1.33x faster)

## Thread Safety

### Runner
- Single-threaded execution
- Deterministic execution order
- Simple to debug
- No synchronization needed

### ParallelRunner
- Multi-threaded execution using Task Parallel Library (TPL)
- Non-deterministic order within waves
- More complex to debug
- Uses `ConcurrentDictionary` for synchronization
- Thread-safe data sharing between agents

## Use Case Decision Matrix

| Scenario | Recommended Runner | Reason |
|----------|-------------------|---------|
| Linear pipeline | Runner | No parallelism opportunity |
| Independent branches | ParallelRunner | Maximum parallelism benefit |
| Small agent count (<5) | Runner | Overhead not worth it |
| Large agent count (>10) | ParallelRunner | Better resource utilization |
| Development/Debugging | Runner | Easier to trace execution |
| Production/Performance | ParallelRunner | Faster execution time |
| CPU-intensive agents | ParallelRunner | Maximize core usage |
| I/O-intensive agents | ParallelRunner | Overlap I/O wait times |
| Side-effect-heavy agents | Runner | Predictable ordering |
| Pure computation | ParallelRunner | Safe to parallelize |

## Implementation Details

### Core Components

Both runners use:
- **Topological Sorting**: Determines agent execution order based on dependencies
- **ConsumesFrom Attribute**: Defines data flow relationships between agents
- **Schedule**: Controls whether agents run once or indefinitely
- **State Machine**: Initialize ? Execute ? Finish lifecycle

### ParallelRunner Algorithm

The ParallelRunner uses a wave-based execution model:

1. **Build dependency graph** by analyzing `ConsumesFrom` attributes
2. **Initialize all agents in parallel** (no data dependencies)
3. **Execute agents in waves**:
   - Identify agents whose dependencies are satisfied
   - Execute them in parallel using `Parallel.ForEach`
   - Store produced data in thread-safe cache
   - Mark agents as completed
   - Repeat until all agents executed
4. **Finalize all agents in parallel** (no data dependencies)

### Thread-Safe Data Sharing

ParallelRunner ensures thread safety through:
- `ConcurrentDictionary` for tracking completed agents and shared data
- Immutable dependency graph built before execution
- No shared mutable state between agents during execution
- TPL handles thread management automatically

## Current Limitations

Known limitations of the framework:
- Only agents living in the same thread and process are supported (for Runner)
- ParallelRunner requires agents to be thread-safe if they access shared resources
- Only single source data consumption per agent
- No control over the state transitioning
- No dynamic reconfiguration
- Only one instance of each concrete agent type is allowed
- Cycles in agent linking are not allowed

## Future Enhancements

Potential improvements for the framework:
- Support for multiple dependencies per agent
- Configurable degree of parallelism in ParallelRunner
- Performance metrics and profiling built-in
- Cancellation token support for graceful shutdown
- Async/await pattern for I/O-bound agents
- Custom task scheduler support
- Dynamic agent reconfiguration
- Distributed execution across processes/machines

## Examples

The Examples project includes:
- **Agent1, Agent2, Agent3**: Basic demonstration of agent dependencies
- **SlowAgent1, SlowAgent2, SlowAgent3**: Agents with simulated work for performance testing
- **LooseAgent**: Independent agent with no dependencies or dependents
- **ParallelRunnerDemo**: Performance comparison between Runner and ParallelRunner

## Benchmarking

To properly benchmark the difference between runners:
1. Use agents with actual work (sleep or computation)
2. Test with graphs that have parallelism opportunities
3. Run multiple times and average results
4. Account for thread pool warm-up time
5. Measure both throughput and latency

## Summary

The Computational Agent Framework provides a simple yet powerful way to build dataflow-based applications. With the addition of `ParallelRunner`, you can now choose between deterministic sequential execution and high-performance parallel execution based on your needs. Both runners maintain API compatibility, making it easy to switch between them as your requirements evolve.

**Key Takeaway**: Use `ParallelRunner` for performance-critical production workloads with parallelizable agent graphs. Use `Runner` for simple pipelines, debugging, or when deterministic behavior is required.
