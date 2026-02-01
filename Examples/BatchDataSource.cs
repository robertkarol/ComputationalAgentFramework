using ComputationalAgentFramework.Agent;
using System;

namespace Examples
{
    public class BatchDataSource : ComputationalAgent<int, int>
    {
        private int _value;

        public BatchDataSource(string name) : base(name)
        {
        }

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
            _value = 100;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} initialized (batch agent, produces once)");
        }

        public override int Produce()
        {
            return _value;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} executed - producing: {_value}");
        }
    }
}
