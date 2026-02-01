using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Framework.Scheduler;
using ComputationalAgentFramework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ComputationalAgentFramework.Framework
{
    public class Runner
    {
        private IDictionary<string, IComputationalAgent> _agents;
        private HashSet<string> _executedBatchAgents;
        private IEnumerable<TopologicalSort.Item> _agentsInExecutionOrder;
        private StreamingCoordinator _streamingCoordinator;

        public Runner()
        {
            _agents = new Dictionary<string, IComputationalAgent>();
            _executedBatchAgents = new HashSet<string>();
        }

        public void AddAgent(IComputationalAgent agent)
        {
            _agents.Add(agent.ToString(), agent);
        }

        public void Run(Schedule schedule)
        {
            var scheduler = new SchedulerFactory().Create(schedule);

            _agentsInExecutionOrder = GetAgentExecutionOrder();
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
                        ExecuteAgents();
                        
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

        private void InitializeAgents()
        {
            foreach (var agent in _agentsInExecutionOrder)
            {
                if (string.IsNullOrEmpty(agent.Name))
                {
                    continue;
                }
                _agents[agent.Name].Initialize();
            }
        }

        private void ExecuteAgents()
        {
            // Phase 1: Execute all batch agents first
            ExecuteBatchAgents();
            
            // Phase 2: Execute streaming pipeline if there are streaming agents
            if (_streamingCoordinator.HasActiveStreams())
            {
                ExecuteStreamingPipeline();
            }
        }

        private void ExecuteBatchAgents()
        {
            foreach (var agent in _agentsInExecutionOrder)
            {
                if (string.IsNullOrEmpty(agent.Name))
                {
                    continue;
                }
                
                var stateAgent = _agents[agent.Name] as ComputationalAgent;
                if (stateAgent == null)
                {
                    continue;
                }
                
                // Skip streaming agents in batch phase
                if (stateAgent is IStreamingAgent)
                {
                    continue;
                }
                
                // Skip batch agents that have already executed
                if (_executedBatchAgents.Contains(agent.Name))
                {
                    continue;
                }
                
                // Handle multi-source agents
                if (stateAgent is MultiSourceComputationalAgent multiSourceAgent)
                {
                    var dataSources = GetDependantAgents(multiSourceAgent);
                    foreach (var source in dataSources)
                    {
                        var sourceAgent = source as ComputationalAgent;
                        if (sourceAgent?.ProducedData != null)
                        {
                            multiSourceAgent.ToConsumeDataSources[source.GetType()] = sourceAgent.ProducedData;
                        }
                    }
                }
                // Handle regular single-source agents
                else
                {
                    var dataSourceAgent = GetDependantAgent(stateAgent) as ComputationalAgent;
                    stateAgent.ToConsumeData = dataSourceAgent?.ProducedData;
                }
                
                stateAgent.Execute();
                _executedBatchAgents.Add(agent.Name);
            }
        }

        private void ExecuteStreamingPipeline()
        {
            _streamingCoordinator.Reset();

            // Execute until all streams are complete
            while (_streamingCoordinator.HasActiveStreams())
            {
                // Execute all stream producers
                foreach (var producerName in _streamingCoordinator.GetStreamProducers())
                {
                    if (!_agents.TryGetValue(producerName, out var producer))
                    {
                        continue;
                    }

                    var streamProducer = producer as IStreamingAgent;
                    if (streamProducer == null)
                    {
                        continue;
                    }
                    
                    // Check completion before execution
                    if (!streamProducer.HasMoreData)
                    {
                        if (!_streamingCoordinator.IsProducerComplete(producerName))
                        {
                            _streamingCoordinator.NotifyCompletion(producerName);
                        }
                        continue;
                    }

                    // Execute producer
                    producer.Execute();
                    
                    // Only propagate if we actually produced data and still have more
                    if (streamProducer.HasMoreData)
                    {
                        // Propagate data to consumers
                        _streamingCoordinator.PropagateData(producerName);
                        
                        // Execute consumers
                        ExecuteStreamConsumersFor(producerName);
                    }
                }

                // Execute intermediate streaming consumers that may also be producers
                foreach (var consumerName in _streamingCoordinator.GetStreamConsumers())
                {
                    if (!_agents.TryGetValue(consumerName, out var consumer))
                    {
                        continue;
                    }

                    var streamConsumer = consumer as IStreamingAgent;
                    if (streamConsumer == null)
                    {
                        continue;
                    }

                    // Execute consumer (processes queued data)
                    consumer.Execute();
                    
                    // If this consumer also produces data and has more, propagate it
                    if (streamConsumer.HasMoreData)
                    {
                        _streamingCoordinator.PropagateData(consumerName);
                        ExecuteStreamConsumersFor(consumerName);
                    }
                    
                    // Check if complete
                    if (!streamConsumer.HasMoreData && !_streamingCoordinator.IsProducerComplete(consumerName))
                    {
                        _streamingCoordinator.NotifyCompletion(consumerName);
                    }
                }
            }
        }

        private void ExecuteStreamConsumersFor(string producerName)
        {
            // Get direct consumers of this producer
            var producerType = _agents[producerName]?.GetType();
            if (producerType == null)
            {
                return;
            }

            foreach (var agent in _agentsInExecutionOrder)
            {
                if (string.IsNullOrEmpty(agent.Name))
                {
                    continue;
                }

                var consumer = _agents[agent.Name];
                if (consumer is not IStreamingAgent)
                {
                    continue;
                }

                // Check if this consumer depends on the producer
                var attributes = consumer.GetType()
                    .GetCustomAttributes(typeof(ConsumesFrom), true)
                    .Cast<ConsumesFrom>();

                if (attributes.Any(attr => attr.Producer == producerType))
                {
                    consumer.Execute();
                }
            }
        }

        private void FinalizeAgents()
        {
            foreach (var agent in _agentsInExecutionOrder)
            {
                if (string.IsNullOrEmpty(agent.Name))
                {
                    continue;
                }
                _agents[agent.Name].Finish();
            }
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
            foreach(var agentValue in _agents.Values)
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
