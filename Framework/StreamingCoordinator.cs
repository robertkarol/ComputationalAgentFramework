using ComputationalAgentFramework.Agent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputationalAgentFramework.Framework
{
    /// <summary>
    /// Coordinates streaming data flow between producer and consumer agents.
    /// Handles automatic data propagation, completion signaling, and multi-stage pipelines.
    /// </summary>
    internal class StreamingCoordinator
    {
        private readonly IDictionary<string, IComputationalAgent> _agents;
        private readonly HashSet<string> _streamProducers;
        private readonly HashSet<string> _streamConsumers;
        private readonly Dictionary<string, HashSet<string>> _producerToConsumers;
        private readonly HashSet<string> _completedProducers;

        public StreamingCoordinator(IDictionary<string, IComputationalAgent> agents)
        {
            _agents = agents;
            _streamProducers = new HashSet<string>();
            _streamConsumers = new HashSet<string>();
            _producerToConsumers = new Dictionary<string, HashSet<string>>();
            _completedProducers = new HashSet<string>();
            
            AnalyzeStreamingTopology();
        }

        private void AnalyzeStreamingTopology()
        {
            // Identify streaming producers (no streaming dependencies)
            foreach (var agent in _agents.Values.OfType<IStreamingAgent>())
            {
                var agentName = agent.ToString();
                var hasStreamingUpstream = HasStreamingUpstream(agent);
                
                if (!hasStreamingUpstream)
                {
                    _streamProducers.Add(agentName);
                }
                else
                {
                    _streamConsumers.Add(agentName);
                }
            }

            // Build producer->consumer mapping
            foreach (var producerName in _streamProducers)
            {
                var consumers = GetDownstreamConsumers(producerName);
                _producerToConsumers[producerName] = new HashSet<string>(consumers);
            }

            // Add intermediate streaming agents as producers for their consumers
            foreach (var consumerName in _streamConsumers.ToList())
            {
                var consumers = GetDownstreamConsumers(consumerName);
                if (consumers.Any())
                {
                    _producerToConsumers[consumerName] = new HashSet<string>(consumers);
                }
            }
        }

        private bool HasStreamingUpstream(IComputationalAgent agent)
        {
            var attributes = agent.GetType()
                .GetCustomAttributes(typeof(Utils.ConsumesFrom), true)
                .Cast<Utils.ConsumesFrom>();

            foreach (var attr in attributes)
            {
                IComputationalAgent producer;

                // If specific instance name is specified, find by name
                if (!string.IsNullOrEmpty(attr.ProducerName))
                {
                    producer = _agents.Values.FirstOrDefault(a => 
                        a.GetType() == attr.Producer && 
                        a.ToString() == attr.ProducerName);
                }
                else
                {
                    // Otherwise, find by type
                    producer = _agents.Values.FirstOrDefault(a => a.GetType() == attr.Producer);
                }

                if (producer is IStreamingAgent)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<string> GetDownstreamConsumers(string producerName)
        {
            if (!_agents.TryGetValue(producerName, out var producer))
            {
                yield break;
            }

            var producerType = producer.GetType();
            var producerInstanceName = producer.ToString();

            foreach (var agent in _agents.Values)
            {
                var attributes = agent.GetType()
                    .GetCustomAttributes(typeof(Utils.ConsumesFrom), true)
                    .Cast<Utils.ConsumesFrom>();

                foreach (var attr in attributes)
                {
                    // Check if this attribute matches our producer
                    if (attr.Producer == producerType)
                    {
                        // If ProducerName is specified, must match
                        if (!string.IsNullOrEmpty(attr.ProducerName))
                        {
                            if (attr.ProducerName == producerInstanceName)
                            {
                                yield return agent.ToString();
                            }
                        }
                        else
                        {
                            // No ProducerName specified, match by type (backward compatible)
                            yield return agent.ToString();
                        }
                    }
                }
            }
        }

        public bool HasActiveStreams()
        {
            // Check if any producer still has data
            foreach (var producerName in _streamProducers)
            {
                if (_agents.TryGetValue(producerName, out var agent) && 
                    agent is IStreamingAgent streamAgent && 
                    streamAgent.HasMoreData)
                {
                    return true;
                }
            }

            // Check if any intermediate consumer still has data to process
            foreach (var consumerName in _streamConsumers)
            {
                if (_agents.TryGetValue(consumerName, out var agent) && 
                    agent is IStreamingAgent streamAgent && 
                    streamAgent.HasMoreData)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> GetStreamProducers()
        {
            return _streamProducers;
        }

        public IEnumerable<string> GetStreamConsumers()
        {
            return _streamConsumers;
        }

        public void PropagateData(string producerName)
        {
            if (!_agents.TryGetValue(producerName, out var producer))
            {
                return;
            }

            if (producer is not ComputationalAgent computationalProducer)
            {
                return;
            }

            var producedData = computationalProducer.ProducedData;
            if (producedData == null)
            {
                return;
            }

            // Get consumers for this producer
            if (!_producerToConsumers.TryGetValue(producerName, out var consumerNames))
            {
                return;
            }

            // Push data to all consumers
            foreach (var consumerName in consumerNames)
            {
                if (_agents.TryGetValue(consumerName, out var consumer))
                {
                    // Use reflection to call the generic Consume method
                    var consumeMethod = consumer.GetType().GetMethod("Consume");
                    consumeMethod?.Invoke(consumer, new[] { producedData });
                }
            }
        }

        public void NotifyCompletion(string producerName)
        {
            if (_completedProducers.Contains(producerName))
            {
                return;
            }

            _completedProducers.Add(producerName);

            // Notify all downstream consumers
            if (!_producerToConsumers.TryGetValue(producerName, out var consumerNames))
            {
                return;
            }

            foreach (var consumerName in consumerNames)
            {
                if (_agents.TryGetValue(consumerName, out var consumer))
                {
                    var notifyMethod = consumer.GetType().GetMethod("NotifyProducerComplete");
                    notifyMethod?.Invoke(consumer, null);
                }
            }
        }

        public bool IsProducerComplete(string agentName)
        {
            return _completedProducers.Contains(agentName);
        }

        public void Reset()
        {
            _completedProducers.Clear();
        }
    }
}
