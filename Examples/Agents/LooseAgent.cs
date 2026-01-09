using ComputationalAgentFramework.Agent;
using System;
using System.Threading;

namespace Examples.Agents
{
    public class LooseAgent : ComputationalAgent<int, int>
    {
        private int _value;

        public LooseAgent(string name) : base(name) { }

        public override void Consume(int consumedData)
        {
            _value = consumedData;
        }

        public override void Finish()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} finished");
        }

        public override void Initialize()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} initialized");
            _value = 999;
        }

        public override int Produce()
        {
            return _value;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} started execution (LOOSE AGENT - NO DEPENDENCIES)");
            Thread.Sleep(1000); // Simulate 1 second of work
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} completed execution");
        }
    }
}
