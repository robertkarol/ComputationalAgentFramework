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

        public ParallelRunner()
        {
            _agents = new Dictionary<string, IComputationalAgent>();
            _dependencyGraph = new Dictionary<string, List<string>>();
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
            BuildDependencyGraph();

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
                        ExecuteAgentsInParallel();
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

        private void ExecuteAgentsInParallel()
        {
            var completedAgents = new ConcurrentDictionary<string, bool>();
            var agentData = new ConcurrentDictionary<string, object>();
            var agentList = _agentsInExecutionOrder.Where(a => !string.IsNullOrEmpty(a.Name)).ToList();

            while (completedAgents.Count < agentList.Count)
            {
                var readyAgents = agentList
                    .Where(agent => !completedAgents.ContainsKey(agent.Name))
                    .Where(agent => _dependencyGraph[agent.Name].All(dep => completedAgents.ContainsKey(dep)))
                    .ToList();

                if (readyAgents.Count == 0)
                {
                    break;
                }

                Parallel.ForEach(readyAgents, agent =>
                {
                    var stateAgent = _agents[agent.Name] as ComputationalAgent;
                    var dataSourceAgent = GetDependantAgent(stateAgent) as ComputationalAgent;

                    if (dataSourceAgent != null)
                    {
                        if (agentData.TryGetValue(dataSourceAgent.ToString(), out var data))
                        {
                            stateAgent.ToConsumeData = data;
                        }
                    }

                    stateAgent.Execute();

                    if (stateAgent.ProducedData != null)
                    {
                        agentData[agent.Name] = stateAgent.ProducedData;
                    }

                    completedAgents.TryAdd(agent.Name, true);
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
            foreach (var agentValue in _agents.Values)
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
