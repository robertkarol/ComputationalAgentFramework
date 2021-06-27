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

        IEnumerable<TopologicalSort.Item> _agentsInExecutionOrder;

        public Runner()
        {
            _agents = new Dictionary<string, IComputationalAgent>();
        }

        public void AddAgent(IComputationalAgent agent)
        {
            foreach (var agentValue in _agents.Values)
            {
                if (agent.GetType() == agentValue.GetType())
                {
                    throw new ArgumentException("Only one agent instance is currently supported");
                }
            }
            _agents.Add(agent.ToString(), agent);
        }

        public void Run(Schedule schedule)
        {
            var scheduler = new SchedulerFactory().Create(schedule);

            _agentsInExecutionOrder = GetAgentExecutionOrder();

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
            => TopologicalSort.Sort(GetItems(), x => x.Dependencies, new TopologicalSort.ItemEqualityComparer()).Reverse();

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
            foreach (var agent in _agentsInExecutionOrder)
            {
                if (string.IsNullOrEmpty(agent.Name))
                {
                    continue;
                }
                var stateAgent = _agents[agent.Name] as ComputationalAgent;
                var dataSourceAgent = GetDependantAgent(stateAgent) as ComputationalAgent;
                stateAgent.ToConsumeData = dataSourceAgent?.ProducedData;
                stateAgent.Execute();
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
                agentItem => agentItem.Dependencies = GetDependingAgents(_agents[agentItem.Name]).Select(a => agentItems[a.ToString()]).ToArray());
            return agentItems.Values;
        }

        private IComputationalAgent GetDependantAgent(IStateMachineAgent agent)
        {
            var attribute = agent.GetType().GetCustomAttributes(typeof(ConsumesFrom), true).FirstOrDefault() as ConsumesFrom;
            return _agents.Values.Where(a => a.GetType() == attribute?.Producer).FirstOrDefault();
        }

        private IEnumerable<IComputationalAgent> GetDependingAgents(IStateMachineAgent agent)
        {
            foreach(var agentValue in _agents.Values)
            {
                var attribute = agentValue.GetType().GetCustomAttributes(typeof(ConsumesFrom), true).FirstOrDefault() as ConsumesFrom;
                if (attribute?.Producer == agent.GetType())
                {
                    yield return agentValue;
                }
            }
        }
    }
}
