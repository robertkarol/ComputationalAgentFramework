using ComputationalAgentFramework.Agent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputationalAgentFramework.Framework.Scheduler
{
    public class RunUntilStreamCompleteScheduler : Scheduler
    {
        private bool _streamComplete;
        private bool _hasStreamingAgents;

        public RunUntilStreamCompleteScheduler()
        {
            _streamComplete = false;
            _hasStreamingAgents = false;
        }

        public bool CanRun()
        {
            return true;
        }

        public bool HasMoreEpochsToRun()
        {
            return !_streamComplete;
        }

        public void ThickEpoch()
        {
        }

        public void NotifyAgentExecuted(IComputationalAgent agent)
        {
            if (agent is IStreamingAgent streamingAgent)
            {
                _hasStreamingAgents = true;
                if (!streamingAgent.HasMoreData)
                {
                    // Check if we should mark complete
                    // This will be updated by the runner after all agents execute
                }
            }
        }

        public void NotifyEpochComplete(IEnumerable<IComputationalAgent> agents)
        {
            var streamingAgents = agents.OfType<IStreamingAgent>().ToList();
            
            if (!streamingAgents.Any())
            {
                _streamComplete = true;
                return;
            }

            _hasStreamingAgents = true;
            _streamComplete = streamingAgents.All(agent => !agent.HasMoreData);
        }
    }
}
