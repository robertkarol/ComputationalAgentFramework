using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using System;
using System.Diagnostics;

namespace Examples
{
    public class HybridStreamingDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("Hybrid Streaming & Batch Demo");
            Console.WriteLine("========================================\n");

            Console.WriteLine("Agent Graph:");
            Console.WriteLine("  BatchDataSource (batch agent, produces once: 100)");
            Console.WriteLine("  NumberStreamProducer (streaming agent, produces 5 numbers)");
            Console.WriteLine("    -> HybridConsumer (multi-source: consumes batch + stream)\n");

            Console.WriteLine("Expected behavior:");
            Console.WriteLine("  1. BatchDataSource executes once, produces 100");
            Console.WriteLine("  2. NumberStreamProducer streams 1, 2, 3, 4, 5");
            Console.WriteLine("  3. HybridConsumer receives batch value once");
            Console.WriteLine("  4. HybridConsumer receives each streamed value");
            Console.WriteLine("  5. Execution continues until stream completes\n");

            Console.WriteLine("--- Running Hybrid Demo ---");
            var sw = Stopwatch.StartNew();

            BatchDataSource batchSource = new BatchDataSource("BatchSource");
            NumberStreamProducer streamProducer = new NumberStreamProducer("StreamProducer", 5);
            HybridConsumer hybridConsumer = new HybridConsumer("HybridConsumer");

            Runner runner = new Runner();
            runner.AddAgent(batchSource);
            runner.AddAgent(streamProducer);
            runner.AddAgent(hybridConsumer);

            runner.Run(Schedule.RunUntilStreamComplete);
            sw.Stop();

            Console.WriteLine($"\nHybrid execution completed in {sw.ElapsedMilliseconds}ms");
            Console.WriteLine("\n========================================");
            Console.WriteLine("Hybrid Streaming Summary");
            Console.WriteLine("========================================");
            Console.WriteLine("? Batch agent executed once");
            Console.WriteLine("? Streaming agent produced incremental data");
            Console.WriteLine("? Multi-source consumer received both batch and stream data");
            Console.WriteLine("? Execution continued until streaming complete");
            Console.WriteLine("========================================\n");
        }
    }
}
