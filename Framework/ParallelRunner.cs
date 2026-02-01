using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Framework.Scheduler;
using ComputationalAgentFramework.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Framework
{
    public class ParallelRunner
    {
        private IDictionary<string, IComputationalAgent> _agents;
        private IEnumerable<TopologicalSort.Item> _agentsInExecutionOrder;
        private IDictionary<string, List<string>> _dependencyGraph;
        private ConcurrentDictionary<string, bool> _executedBatchAgents;
        private StreamingCoordinator _streamingCoordinator;

        public ParallelRunner()
        {
            _agents = new Dictionary<string, IComputationalAgent>();
            _dependencyGraph = new Dictionary<string, List<string>>();
            _executedBatchAgents = new ConcurrentDictionary<string, bool>();
        }

        public void AddAgent(IComputationalAgent agent)
        {
            _agents.Add(agent.ToString(), agent);
        }

        public void Run(Schedule schedule)
        {
            var scheduler = new SchedulerFactory().Create(schedule);

            _agentsInExecutionOrder = GetAgentExecutionOrder();
            BuildDependencyGraph();
            _executedBatchAgents.Clear();
            _streamingCoordinator = new StreamingCoordinator(_agents);

            InitializeAgents();
            try
            {
                while (scheduler.HasMoreEpochsToRun())
                {
                    while (!scheduler.CanRun())
                    {
                        Thread.Sleep(100);
                    }

                    if (scheduler.CanRun())
                    {
                        // Phase 1: Execute batch agents in parallel
                        ExecuteBatchAgentsInParallel();
                        
                        // Phase 2: Execute streaming pipeline
                        if (_streamingCoordinator.HasActiveStreams())
                        {
                            ExecuteStreamingPipeline();
                        }
                        
                        // Notify scheduler about agent states (for RunUntilStreamComplete)
                        if (scheduler is RunUntilStreamCompleteScheduler streamScheduler)
                        {
                            streamScheduler.NotifyEpochComplete(_agents.Values);
                        }
                        
                        scheduler.ThickEpoch();
                    }
                }
            }
            finally
            {
                FinalizeAgents();
            }
        }

        private IEnumerable<TopologicalSort.Item> GetAgentExecutionOrder()
            => TopologicalSort.Sort(GetItems(), x => x.Dependencies, new TopologicalSort.ItemEqualityComparer());

        private void BuildDependencyGraph()
        {
            _dependencyGraph.Clear();
            foreach (var agent in _agentsInExecutionOrder)
            {
                if (string.IsNullOrEmpty(agent.Name))
                {
                    continue;
                }

                var dependencies = agent.Dependencies
                    .Where(d => !string.IsNullOrEmpty(d.Name))
                    .Select(d => d.Name)
                    .ToList();

                _dependencyGraph[agent.Name] = dependencies;
            }
        }

        private void InitializeAgents()
        {
            Parallel.ForEach(_agentsInExecutionOrder.Where(a => !string.IsNullOrEmpty(a.Name)), agent =>
            {
                _agents[agent.Name].Initialize();
            });
        }

        private void ExecuteBatchAgentsInParallel()
        {
            var completedAgents = new ConcurrentDictionary<string, bool>();
            var agentData = new ConcurrentDictionary<string, object>();
            var batchAgents = _agentsInExecutionOrder
                .Where(a => !string.IsNullOrEmpty(a.Name))
                .Where(a => !(_agents[a.Name] is IStreamingAgent))
                .ToList();

            while (completedAgents.Count < batchAgents.Count)
            {
                var readyAgents = batchAgents
                    .Where(agent => !completedAgents.ContainsKey(agent.Name))
                    .Where(agent => !_executedBatchAgents.ContainsKey(agent.Name))
                    .Where(agent => _dependencyGraph[agent.Name].All(dep => 
                        completedAgents.ContainsKey(dep) || (_agents[dep] is IStreamingAgent)))
                    .ToList();

                if (readyAgents.Count == 0)
                {
                    break;
                }

                Parallel.ForEach(readyAgents, agent =>
                {
                    var stateAgent = _agents[agent.Name] as ComputationalAgent;

                    if (stateAgent is MultiSourceComputationalAgent multiSourceAgent)
                    {
                        var dataSources = GetDependantAgents(multiSourceAgent);
                        foreach (var source in dataSources)
                        {
                            if (agentData.TryGetValue(source.ToString(), out var data))
                            {
                                multiSourceAgent.ToConsumeDataSources[source.GetType()] = data;
                            }
                        }
                    }
                    else
                    {
                        var dataSourceAgent = GetDependantAgent(stateAgent) as ComputationalAgent;
                        if (dataSourceAgent != null)
                        {
                            if (agentData.TryGetValue(dataSourceAgent.ToString(), out var data))
                            {
                                stateAgent.ToConsumeData = data;
                            }
                        }
                    }

                    stateAgent.Execute();

                    if (stateAgent.ProducedData != null)
                    {
                        agentData[agent.Name] = stateAgent.ProducedData;
                    }

                    completedAgents.TryAdd(agent.Name, true);
                    _executedBatchAgents.TryAdd(agent.Name, true);
                });
            }
        }

        private void ExecuteStreamingPipeline()
        {
            _streamingCoordinator.Reset();

            // Execute until all streams are complete
            while (_streamingCoordinator.HasActiveStreams())
            {
                // Execute all stream producers
                Parallel.ForEach(_streamingCoordinator.GetStreamProducers(), producerName =>
                {
                    if (!_agents.TryGetValue(producerName, out var producer))
                    {
                        return;
                    }

                    var streamProducer = producer as IStreamingAgent;
                    if (streamProducer == null || !streamProducer.HasMoreData)
                    {
                        if (streamProducer != null && !_streamingCoordinator.IsProducerComplete(producerName))
                        {
                            _streamingCoordinator.NotifyCompletion(producerName);
                        }
                        return;
                    }

                    // Execute producer
                    producer.Execute();
                    
                    // Propagate data to consumers (thread-safe)
                    lock (_streamingCoordinator)
                    {
                        _streamingCoordinator.PropagateData(producerName);
                    }
                });

                // Execute streaming consumers
                Parallel.ForEach(_streamingCoordinator.GetStreamConsumers(), consumerName =>
                {
                    if (!_agents.TryGetValue(consumerName, out var consumer))
                    {
                        return;
                    }

                    var streamConsumer = consumer as IStreamingAgent;
                    if (streamConsumer == null)
                    {
                        return;
                    }

                    // Execute consumer (processes queued data)
                    consumer.Execute();
                    
                    // If this consumer also produces data, propagate it
                    lock (_streamingCoordinator)
                    {
                        _streamingCoordinator.PropagateData(consumerName);
                    }
                    
                    // Check if complete
                    if (!streamConsumer.HasMoreData && !_streamingCoordinator.IsProducerComplete(consumerName))
                    {
                        lock (_streamingCoordinator)
                        {
                            if (!_streamingCoordinator.IsProducerComplete(consumerName))
                            {
                                _streamingCoordinator.NotifyCompletion(consumerName);
                            }
                        }
                    }
                });
            }
        }

        private void FinalizeAgents()
        {
            Parallel.ForEach(_agentsInExecutionOrder.Where(a => !string.IsNullOrEmpty(a.Name)), agent =>
            {
                _agents[agent.Name].Finish();
            });
        }

        private IEnumerable<TopologicalSort.Item> GetItems()
        {
            var agentItems = _agents.Values.Select(a => new TopologicalSort.Item(a.ToString())).ToDictionary(i => i.Name);
            agentItems.Values.ToList().ForEach(
                agentItem => agentItem.Dependencies = GetDependantAgents(_agents[agentItem.Name]).Select(a => agentItems[a.ToString()]).ToArray());
            return agentItems.Values;
        }

        private IComputationalAgent GetDependantAgent(IStateMachineAgent agent)
        {
            var attribute = agent.GetType().GetCustomAttributes(typeof(ConsumesFrom), true).FirstOrDefault() as ConsumesFrom;
            if (attribute == null)
            {
                return null;
            }

            // If specific instance name is specified, find by name
            if (!string.IsNullOrEmpty(attribute.ProducerName))
            {
                return _agents.Values.FirstOrDefault(a => 
                    a.GetType() == attribute.Producer && 
                    a.ToString() == attribute.ProducerName);
            }

            // Otherwise, find by type (backward compatible)
            return _agents.Values.FirstOrDefault(a => a.GetType() == attribute.Producer);
        }

        private IEnumerable<IComputationalAgent> GetDependantAgents(IStateMachineAgent agent)
        {
            var attributes = agent.GetType().GetCustomAttributes(typeof(ConsumesFrom), true).Cast<ConsumesFrom>();
            foreach (var attribute in attributes)
            {
                IComputationalAgent producer;

                // If specific instance name is specified, find by name
                if (!string.IsNullOrEmpty(attribute.ProducerName))
                {
                    producer = _agents.Values.FirstOrDefault(a => 
                        a.GetType() == attribute.Producer && 
                        a.ToString() == attribute.ProducerName);
                }
                else
                {
                    // Otherwise, find by type (backward compatible)
                    producer = _agents.Values.FirstOrDefault(a => a.GetType() == attribute.Producer);
                }

                if (producer != null)
                {
                    yield return producer;
                }
            }
        }

        private IEnumerable<IComputationalAgent> GetDependingAgents(IStateMachineAgent agent)
        {
            foreach (var agentValue in _agents.Values)
            {
                var attributes = agentValue.GetType().GetCustomAttributes(typeof(ConsumesFrom), true).Cast<ConsumesFrom>();
                if (attributes.Any(attr => attr.Producer == agent.GetType()))
                {
                    yield return agentValue;
                }
            }
        }
    }
}
