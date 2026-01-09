using ComputationalAgentFramework.Agent;
using System;

namespace Examples.Agents.Multisource
{
    public class DataSourceA : ComputationalAgent<int, int>
    {
        private int _value;

        public DataSourceA(string name) : base(name) { }

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
            _value = 10;
        }

        public override int Produce()
        {
            return _value;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} executed - producing value: {_value}");
        }
    }
}
