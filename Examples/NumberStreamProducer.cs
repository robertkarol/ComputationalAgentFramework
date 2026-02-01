using ComputationalAgentFramework.Agent;
using System;
using System.Collections.Generic;

namespace Examples
{
    public class NumberStreamProducer : StreamProducerAgent<int>
    {
        private int _maxNumbers;
        private int _currentCount;

        public NumberStreamProducer(string name, int maxNumbers = 10) : base(name)
        {
            _maxNumbers = maxNumbers;
        }

        protected override IEnumerable<int> GenerateStream()
        {
            for (int i = 1; i <= _maxNumbers; i++)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} streaming value: {i}");
                yield return i;
            }
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} stream complete");
        }

        public override int Produce()
        {
            return (int)ProducedData;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} produced: {ProducedData}");
        }

        protected override void InitializeStream()
        {
            _currentCount = 0;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} initialized (will stream {_maxNumbers} numbers)");
        }

        protected override void FinalizeStream()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} finalized");
        }
    }
}
