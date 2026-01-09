using ComputationalAgentFramework;
using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using Examples.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            // Simple Runner example
            Agent1 agent1 = new Agent1("a");
            Agent2 agent2 = new Agent2("b");
            Agent3 agent3 = new Agent3("c");
            Runner application = new Runner();
            application.AddAgent(agent1);
            application.AddAgent(agent2);
            application.AddAgent(agent3);
            application.Run(Schedule.RunOnce);

            // Parallel Runner comparison demo
            ParallelRunnerDemo.CompareRunners();
        }
    }
}
