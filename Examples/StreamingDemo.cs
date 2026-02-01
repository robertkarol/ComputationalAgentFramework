using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using System;
using System.Diagnostics;

namespace Examples
{
    public class StreamingDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("Streaming Agents Demo");
            Console.WriteLine("========================================\n");

            Console.WriteLine("Agent Graph:");
            Console.WriteLine("  NumberStreamProducer (streams 10 numbers)");
            Console.WriteLine("    -> NumberStreamConsumer (consumes stream and calculates sum)\n");

            Console.WriteLine("--- Sequential Runner with Streaming ---");
            var sw = Stopwatch.StartNew();

            NumberStreamProducer producer = new NumberStreamProducer("StreamProducer", 10);
            NumberStreamConsumer consumer = new NumberStreamConsumer("StreamConsumer");

            Runner runner = new Runner();
            runner.AddAgent(producer);
            runner.AddAgent(consumer);

            runner.Run(Schedule.RunUntilStreamComplete);
            sw.Stop();

            Console.WriteLine($"\nStreaming execution completed in {sw.ElapsedMilliseconds}ms");
            Console.WriteLine("\n========================================");
            Console.WriteLine("Streaming Summary");
            Console.WriteLine("========================================");
            Console.WriteLine("? Producer streamed data incrementally");
            Console.WriteLine("? Consumer processed each item as it arrived");
            Console.WriteLine("? Execution continued until stream completed");
            Console.WriteLine("========================================\n");
        }
    }
}
