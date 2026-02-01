using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;
using System;

namespace Examples
{
    [ConsumesFrom(typeof(NumberStreamProducer))]
    public class NumberStreamConsumer : StreamConsumerAgent<int, int>
    {
        private int _sum;
        private int _count;

        public NumberStreamConsumer(string name) : base(name)
        {
        }

        public override void ConsumeStreamItem(int item)
        {
            _sum += item;
            _count++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} consumed: {item}, running sum: {_sum}");
        }

        public override int Produce()
        {
            return _sum;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} processed item #{_count}");
        }

        protected override void InitializeStream()
        {
            _sum = 0;
            _count = 0;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} initialized");
        }

        protected override void FinalizeStream()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} finalized - Total sum: {_sum}, Count: {_count}");
        }
    }
}
