# Computational Agent Framework (CAF)

## Overview

__Computational Agent Framework (CAF)__ is a C# framework for developing computational entities that have a state machine architecture for the individual computation each performs, and externally behave in a dataflow fashion. You can write programs that are a bunch of entities communicating with each other in a well-determined manner.

## Requirements

- **.NET 10.0 SDK** or later
- **Visual Studio 2022 (17.12+)** or any compatible IDE/editor
- **Git** for source control

## Getting Started

### Prerequisites

Ensure you have the .NET 10.0 SDK installed. If you need to install it, download from: https://dotnet.microsoft.com/download/dotnet/10.0

### Get the Code Working

This repository contains a Visual Studio solution with two projects:
- A project containing the framework
- A project providing examples of how to use the framework

Clone and build:

```bash
git clone https://github.com/robertkarol/ComputationalAgentFramework
cd ComputationalAgentFramework
dotnet build
dotnet run --project Examples\Examples.csproj
```

## Core Concepts

### Computational Agents

A __Computational Agent (CA)__ is a state machine with three states:
- **Initialize**: Setup and preparation
- **Execute**: Main computation (the heart of the agent)
- **Finish**: Cleanup and finalization

Agents consume and produce data of specific types. The framework handles the execution lifecycle - you just implement the logic for each phase by inheriting from `ComputationalAgent` and implementing the required methods.

### Agent Communication

#### Single-Source Consumption

For agents consuming from a single source:
- Inherit from `ComputationalAgent<TConsumed, TProduced>`
- Add `[ConsumesFrom(typeof(ProducerAgent))]` attribute
- Implement `Consume(TConsumed data)` method

The framework links agents using attributes. For example, if Agent2 needs data from Agent1, simply attribute it with `[ConsumesFrom(typeof(Agent1))]`.

#### Multi-Source Consumption

For agents consuming from multiple sources:
- Inherit from `MultiSourceComputationalAgent<TProduced>`
- Add multiple `[ConsumesFrom(typeof(ProducerA))]` attributes
- Implement `ConsumeMultiple(IDictionary<Type, object> data)` method
- Access data by producer type from the dictionary

**Benefits:**
- Consume from multiple independent sources
- Sources execute in parallel (with ParallelRunner)
- Type-safe data access via dictionary
- Full backward compatibility

### Execution Model

Agents are added to a `Runner` or `ParallelRunner` and executed using a scheduling strategy:
- `Schedule.RunOnce`: Execute all agents once
- `Schedule.RunIndefinitely`: Execute agents repeatedly

Agent execution follows a dataflow pattern determined by topological sorting, ensuring each agent executes only after its dependencies have produced their data. Cycles in agent dependencies are not allowed.

## Execution Engines

### Runner (Sequential Execution)

Executes agents sequentially in topological order.

**Characteristics:**
- Single-threaded, sequential execution
- Deterministic execution order
- Inherently thread-safe
- Simple to debug

**Best For:**
- Debugging
- Linear pipelines
- When deterministic behavior is required
- Simple applications

### ParallelRunner (Parallel Execution)

Executes agents in parallel while respecting dependencies using a wave-based approach.

**Characteristics:**
- Multi-threaded, wave-based parallel execution
- Non-deterministic order within waves
- Uses `ConcurrentDictionary` for thread-safe data sharing
- Maximizes CPU core utilization

**Best For:**
- Performance-critical workloads
- Agent graphs with independent branches
- CPU or I/O intensive agents
- Production environments

#### Wave-Based Execution

Agents are executed in parallel waves:
- **Wave 1**: All agents with no dependencies
- **Wave 2**: All agents whose dependencies completed in Wave 1
- **Wave N**: Continues until all agents executed

Independent agents (no dependencies or dependents) execute in the first wave alongside other independent agents, maximizing parallelism.

#### API Compatibility

Both runners have identical APIs. Migration is seamless:
- `var runner = new Runner();` -> `var runner = new ParallelRunner();`
- All other methods (`AddAgent()`, `Run()`) remain the same

## Performance Characteristics

### Time Complexity by Graph Type

| Graph Type | Runner | ParallelRunner | Speedup |
|------------|--------|----------------|---------|
| Linear (n agents) | O(n) | O(n) | 1x |
| Full fan-out (1->n) | O(n+1) | O(2) | ~n/2x |
| Balanced tree (depth d) | O(2^d - 1) | O(d) | ~2^(d-1)/d |

### Execution Flow Patterns

**Linear Graph: A -> B -> C**
- Runner: 3 steps
- ParallelRunner: 3 waves (same as sequential, no benefit)

**Fan-Out Graph: A -> [B, C, D]**
- Runner: 4 steps (sequential)
- ParallelRunner: 2 waves (B, C, D execute in parallel)
- **Speedup: 2x faster**

**Complex DAG: A -> [B, C] -> D**
- Runner: 4 steps
- ParallelRunner: 3 waves (B and C execute in parallel)
- **Speedup: 1.33x faster**

**Multi-Source: [A, B] -> C**
- Runner: 3 steps
- ParallelRunner: 2 waves (A and B execute in parallel)
- **Speedup: 1.5x faster**

### Performance Benefits Example

Given an agent graph where Agent1 has no dependencies, and both Agent2 and Agent3 depend on Agent1:
- **Sequential Runner**: 3 seconds (Agent1->Agent2->Agent3)
- **Parallel Runner**: 2 seconds (Wave 1: Agent1, Wave 2: Agent2+Agent3 parallel)
- **Performance improvement: 33% faster**

## Use Case Decision Matrix

| Scenario | Recommended Runner | Reason |
|----------|-------------------|---------|
| Linear pipeline | Runner | No parallelism opportunity |
| Independent branches | ParallelRunner | Maximum parallelism benefit |
| Multi-source aggregation | ParallelRunner | Producers run in parallel |
| Small agent count (<5) | Runner | Overhead not worth it |
| Large agent count (>10) | ParallelRunner | Better resource utilization |
| Development/Debugging | Runner | Easier to trace execution |
| Production/Performance | ParallelRunner | Faster execution time |
| CPU-intensive agents | ParallelRunner | Maximize core usage |
| I/O-intensive agents | ParallelRunner | Overlap I/O wait times |
| Side-effect-heavy agents | Runner | Predictable ordering |
| Pure computation | ParallelRunner | Safe to parallelize |

## Multi-Source Data Consumption

### Overview

Agents can consume data from multiple independent sources simultaneously, enabling powerful aggregation and data combination patterns.

### When to Use Multi-Source

**Use Multi-Source When:**
- You need to aggregate data from multiple independent sources
- Sources can be produced in parallel
- All source data is required before processing
- Implementing join, merge, or aggregation patterns

**Use Single-Source When:**
- You have a simple pipeline (A -> B -> C)
- Data flows from one producer to one consumer
- Sequential processing is needed
- You want simpler agent implementation

### Multi-Source Features

**Execution Behavior:**
- Multi-source agents depend on ALL their producers
- Agent executes only after ALL producers complete

**Sequential Runner:**
1. All producers execute sequentially
2. Multi-source consumer executes after all producers

**Parallel Runner:**
- Wave 1: Independent producers execute in parallel
- Wave 2: Multi-source consumer executes after all complete

### Common Multi-Source Patterns

**Data Aggregation:**
Combine data from multiple independent sources (e.g., multiple sensors).

**Join Operations:**
Perform SQL-like joins on data from different streams.

**Cross-Validation:**
Validate data from one source against another.

**Feature Engineering:**
Combine features from multiple sources for machine learning.

**Event Correlation:**
Correlate events from different monitoring sources.

### Multi-Source Best Practices

**DO:**
- Use `TryGetValue` for safe dictionary access
- Check for null values after accessing dictionary
- Document which sources are required vs optional
- Use multi-source for true aggregation scenarios
- Test with both runners

**DON'T:**
- Use direct dictionary access without checks
- Use multi-source for sequential dependencies (use single-source chain instead)
- Assume data always exists in dictionary
- Forget to handle missing or null data
- Create circular dependencies

### Data Access Patterns

**Safe Dictionary Access:**
Check if key exists before accessing.

**With Default Values:**
Provide fallback values for missing data.

**Checking All Sources:**
Verify all required sources provided data.

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
- Agents must be thread-safe if they access shared resources

## Implementation Details

### Core Components

Both runners use:
- **Topological Sorting**: Determines execution order based on dependencies
- **ConsumesFrom Attribute**: Defines data flow relationships (supports multiple attributes)
- **Schedule**: Controls single-run or indefinite execution
- **State Machine**: Initialize -> Execute -> Finish lifecycle

### ParallelRunner Algorithm

1. **Build dependency graph** by analyzing `ConsumesFrom` attributes (including multiple)
2. **Initialize all agents in parallel** (no data dependencies at this phase)
3. **Execute agents in waves**:
   - Identify agents whose ALL dependencies are satisfied
   - Execute them in parallel using `Parallel.ForEach`
   - Store produced data in thread-safe cache (`ConcurrentDictionary`)
   - Mark agents as completed
   - Repeat until all agents executed
4. **Finalize all agents in parallel** (no data dependencies at this phase)

### Dependency Resolution

- Single-source agents: Have one dependency (their producer)
- Multi-source agents: Have N dependencies (all their producers)
- Topological sort ensures correct execution order
- ParallelRunner groups agents into waves based on dependency levels

## Current Limitations

- Only agents in the same process are supported
- ParallelRunner requires thread-safe agents if they access shared resources
- Multi-source agents access data by producer type (cannot distinguish multiple instances of same type)
- No control over state transitioning
- No dynamic reconfiguration
- Only one instance of each concrete agent type is allowed
- Cycles in agent linking are not allowed

## Examples

The Examples project includes:

**Basic Examples:**
- **Agent1, Agent2, Agent3**: Basic single-source agent dependencies
- **LooseAgent**: Independent agent with no dependencies or dependents

**Performance Examples:**
- **SlowAgent1, SlowAgent2, SlowAgent3**: Agents with simulated work for timing
- **ParallelRunnerDemo**: Performance comparison between runners

**Multi-Source Examples:**
- **DataSourceA, DataSourceB**: Independent data producers
- **MultiSourceAgent**: Demonstrates consuming from multiple sources
- **MultiSourceDemo**: Shows multi-source execution in both runners

## Benchmarking

To properly benchmark the difference between runners:
1. Use agents with actual work (Thread.Sleep or computation)
2. Test with graphs that have parallelism opportunities
3. Run multiple times and average results
4. Account for thread pool warm-up time
5. Measure both throughput and latency

## Troubleshooting

### Multi-Source Agent Not Receiving Data
- Verify all producers are added to the runner
- Check `ConsumesFrom` attributes specify correct producer types
- Ensure producers actually produce data (ProducedData is not null)

### KeyNotFoundException When Accessing Data
- Always use `TryGetValue` or check `ContainsKey` before accessing
- Handle cases where data might be missing

### Type Casting Errors
- Ensure producer's TProduced type matches what you're casting to
- Use safe casting patterns (`is`, `as`)

### Agent Not Executing in Parallel
- Multi-source agents wait for ALL producers
- Verify producers have no dependencies between them
- Check dependency graph structure

## Future Enhancements

Potential improvements:
- Strongly-typed multi-source with generic parameters for each source
- Optional vs required source specification
- Support for multiple instances of same producer type
- Configurable degree of parallelism in ParallelRunner
- Performance metrics and profiling built-in
- Cancellation token support
- Async/await pattern for I/O-bound agents
- Custom task scheduler support
- Dynamic agent reconfiguration
- Distributed execution across processes/machines

## Summary

The Computational Agent Framework provides a powerful way to build dataflow-based applications with support for:

- **Single-Source Agents**: Traditional one-to-one data flow
- **Multi-Source Agents**: Aggregate data from multiple independent producers
- **Sequential Execution**: Deterministic, easy-to-debug execution with `Runner`
- **Parallel Execution**: High-performance multi-core execution with `ParallelRunner`
- **Mixed Agent Graphs**: Combine single-source and multi-source agents seamlessly

Both runners maintain API compatibility and support all agent types, making it easy to evolve your application from simple pipelines to complex data aggregation graphs.

### Key Takeaways

- Use **Runner** for debugging, simple pipelines, and when deterministic behavior is required
- Use **ParallelRunner** for performance-critical production workloads with parallelizable agent graphs
- Use **Multi-Source Agents** when you need to aggregate data from multiple independent producers
- Both runner types support both single-source and multi-source agents transparently
- Migration between runners is seamless with identical APIs

