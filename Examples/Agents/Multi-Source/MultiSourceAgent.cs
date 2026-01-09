using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;
using System;
using System.Collections.Generic;

namespace Examples.Agents.Multisource
{
    [ConsumesFrom(typeof(DataSourceA))]
    [ConsumesFrom(typeof(DataSourceB))]
    public class MultiSourceAgent : MultiSourceComputationalAgent<string>
    {
        private int _intValue;
        private string _stringValue;

        public MultiSourceAgent(string name) : base(name) { }

        public override void ConsumeMultiple(IDictionary<Type, object> consumedData)
        {
            if (consumedData.TryGetValue(typeof(DataSourceA), out var intData))
            {
                _intValue = (int)intData;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} consumed from DataSourceA: {_intValue}");
            }

            if (consumedData.TryGetValue(typeof(DataSourceB), out var stringData))
            {
                _stringValue = (string)stringData;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} consumed from DataSourceB: {_stringValue}");
            }
        }

        public override void Finish()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} finished");
        }

        public override void Initialize()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} initialized");
        }

        public override string Produce()
        {
            return $"{_stringValue}-{_intValue}";
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} executed - combining data: {_stringValue} + {_intValue}");
        }
    }
}
