using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using ComputationalAgentFramework.Utils;
using System;
using System.Linq;

namespace Examples
{
    public static class MultipleInstancesDemo
    {
        // Data source
        public class DataSource : ComputationalAgent<int, int[]>
        {
            private readonly int[] _data;

            public DataSource(string name, int[] data) : base(name)
            {
                _data = data;
            }

            public override void Consume(int consumedData) { }

            public override int[] Produce()
            {
                return _data;
            }

            protected override void ExecuteComputation()
            {
                Console.WriteLine($"[{ToString()}] Generated data: {string.Join(", ", _data)}");
            }

            public override void Initialize() { }
            public override void Finish() { }
        }

        // Processor that can be instantiated multiple times
        [ConsumesFrom(typeof(DataSource))]
        public class DataProcessor : ComputationalAgent<int[], int>
        {
            private readonly string _operation;
            private readonly Func<int[], int> _processFunc;
            private int[] _inputData;

            public DataProcessor(string name, string operation, Func<int[], int> processFunc) : base(name)
            {
                _operation = operation;
                _processFunc = processFunc;
            }

            public override void Consume(int[] consumedData)
            {
                _inputData = consumedData;
            }

            public override int Produce()
            {
                return _inputData != null ? _processFunc(_inputData) : 0;
            }

            protected override void ExecuteComputation()
            {
                var result = Produce();
                Console.WriteLine($"[{ToString()}] {_operation}: {result}");
            }

            public override void Initialize() { }
            public override void Finish() { }
        }

        // Named consumer that uses a specific processor instance
        [ConsumesFrom(typeof(DataProcessor), "SumProcessor")]
        public class SumConsumer : ComputationalAgent<int, string>
        {
            private int _value;

            public SumConsumer(string name) : base(name) { }

            public override void Consume(int consumedData)
            {
                _value = consumedData;
            }

            public override string Produce()
            {
                return $"Sum Result: {_value}";
            }

            protected override void ExecuteComputation()
            {
                Console.WriteLine($"[{ToString()}] {Produce()}");
            }

            public override void Initialize() { }
            public override void Finish() { }
        }

        [ConsumesFrom(typeof(DataProcessor), "MaxProcessor")]
        public class MaxConsumer : ComputationalAgent<int, string>
        {
            private int _value;

            public MaxConsumer(string name) : base(name) { }

            public override void Consume(int consumedData)
            {
                _value = consumedData;
            }

            public override string Produce()
            {
                return $"Max Result: {_value}";
            }

            protected override void ExecuteComputation()
            {
                Console.WriteLine($"[{ToString()}] {Produce()}");
            }

            public override void Initialize() { }
            public override void Finish() { }
        }

        public static void RunDemo()
        {
            Console.WriteLine("\n=== Multiple Instances Demo ===\n");
            Console.WriteLine("Creating a pipeline with multiple instances of DataProcessor:");
            Console.WriteLine("1. DataSource produces array [1, 2, 3, 4, 5]");
            Console.WriteLine("2. Three DataProcessor instances process the array:");
            Console.WriteLine("   - SumProcessor: calculates sum");
            Console.WriteLine("   - MaxProcessor: finds maximum");
            Console.WriteLine("   - MinProcessor: finds minimum");
            Console.WriteLine("3. Named consumers consume from specific processor instances\n");

            var runner = new Runner();

            // Add data source
            var source = new DataSource("DataSource", new[] { 1, 2, 3, 4, 5 });
            runner.AddAgent(source);

            // Add multiple instances of the same processor type
            var sumProcessor = new DataProcessor("SumProcessor", "Sum", arr => arr.Sum());
            var maxProcessor = new DataProcessor("MaxProcessor", "Max", arr => arr.Max());
            var minProcessor = new DataProcessor("MinProcessor", "Min", arr => arr.Min());

            runner.AddAgent(sumProcessor);
            runner.AddAgent(maxProcessor);
            runner.AddAgent(minProcessor);

            // Add consumers that depend on specific named instances
            var sumConsumer = new SumConsumer("SumConsumer");
            var maxConsumer = new MaxConsumer("MaxConsumer");

            runner.AddAgent(sumConsumer);
            runner.AddAgent(maxConsumer);

            // Run the pipeline
            Console.WriteLine("Executing pipeline...\n");
            runner.Run(Schedule.RunOnce);

            Console.WriteLine("\n=== Parallel Runner Demo ===\n");
            Console.WriteLine("Same pipeline but with parallel execution:\n");

            var parallelRunner = new ParallelRunner();
            
            // Re-create agents for parallel runner
            var source2 = new DataSource("DataSource", new[] { 10, 20, 30, 40, 50 });
            var sumProcessor2 = new DataProcessor("SumProcessor", "Sum", arr => arr.Sum());
            var maxProcessor2 = new DataProcessor("MaxProcessor", "Max", arr => arr.Max());
            var minProcessor2 = new DataProcessor("MinProcessor", "Min", arr => arr.Min());
            var sumConsumer2 = new SumConsumer("SumConsumer");
            var maxConsumer2 = new MaxConsumer("MaxConsumer");

            parallelRunner.AddAgent(source2);
            parallelRunner.AddAgent(sumProcessor2);
            parallelRunner.AddAgent(maxProcessor2);
            parallelRunner.AddAgent(minProcessor2);
            parallelRunner.AddAgent(sumConsumer2);
            parallelRunner.AddAgent(maxConsumer2);

            Console.WriteLine("Executing parallel pipeline...\n");
            parallelRunner.Run(Schedule.RunOnce);

            Console.WriteLine("\nKey Features:");
            Console.WriteLine("- Multiple instances of DataProcessor type allowed");
            Console.WriteLine("- Each instance has unique name for identification");
            Console.WriteLine("- Consumers use [ConsumesFrom(typeof(Type), \"InstanceName\")] for specific dependencies");
            Console.WriteLine("- Backward compatible: unnamed dependencies still work with first matching type");
            Console.WriteLine("- Works with both Runner and ParallelRunner");
        }
    }
}
