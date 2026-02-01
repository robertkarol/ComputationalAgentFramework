using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;
using System;

namespace Examples
{
    [ConsumesFrom(typeof(BatchDataSource))]
    [ConsumesFrom(typeof(NumberStreamProducer))]
    public class HybridConsumer : MultiSourceComputationalAgent<string>
    {
        private int _batchValue;
        private int _streamSum;
        private int _streamCount;

        public HybridConsumer(string name) : base(name)
        {
        }

        public override void ConsumeMultiple(System.Collections.Generic.IDictionary<Type, object> consumedData)
        {
            if (consumedData.TryGetValue(typeof(BatchDataSource), out var batchData))
            {
                _batchValue = (int)batchData;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} consumed from BatchDataSource: {_batchValue}");
            }

            if (consumedData.TryGetValue(typeof(NumberStreamProducer), out var streamData))
            {
                int value = (int)streamData;
                _streamSum += value;
                _streamCount++;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} consumed from NumberStreamProducer: {value}, total streamed: {_streamSum}");
            }
        }

        public override void Finish()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} finished - Batch value: {_batchValue}, Stream sum: {_streamSum}, Stream count: {_streamCount}");
        }

        public override void Initialize()
        {
            _batchValue = 0;
            _streamSum = 0;
            _streamCount = 0;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} initialized");
        }

        public override string Produce()
        {
            return $"Batch:{_batchValue},StreamSum:{_streamSum},StreamCount:{_streamCount}";
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this} computed - Combined result: {Produce()}");
        }
    }
}
