using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;
using System;
using System.Threading;

namespace Examples.Agents
{
    [ConsumesFrom(typeof(SlowAgent1))]
    public class SlowAgent2 : ComputationalAgent<int, int>
    {
        private int _data;

        public SlowAgent2(string name) : base(name) { }

        public override void Consume(int consumedData)
        {
            _data = consumedData;
        }

        public override void Finish()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} finished");
        }

        public override void Initialize()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} initialized");
        }

        public override int Produce()
        {
            return _data + 5;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} started execution (data: {_data})");
            Thread.Sleep(1000); // Simulate 1 second of work
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} completed execution");
        }
    }
}
