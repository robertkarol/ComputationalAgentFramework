using ComputationalAgentFramework.Agent;
using System;

namespace Examples.Agents.Multisource
{
    public class DataSourceB : ComputationalAgent<int, string>
    {
        private string _value;

        public DataSourceB(string name) : base(name) { }

        public override void Consume(int consumedData)
        {
            _value = consumedData.ToString();
        }

        public override void Finish()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} finished");
        }

        public override void Initialize()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} initialized");
            _value = "Hello";
        }

        public override string Produce()
        {
            return _value;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} executed - producing value: {_value}");
        }
    }
}
