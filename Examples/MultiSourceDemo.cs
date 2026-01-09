using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using Examples.Agents.Multisource;
using System;
using System.Diagnostics;

namespace Examples
{
    public class MultiSourceDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("Multi-Source Agent Demo");
            Console.WriteLine("========================================\n");

            Console.WriteLine("Agent Dependency Graph:");
            Console.WriteLine("  DataSourceA (no dependencies) → produces int");
            Console.WriteLine("  DataSourceB (no dependencies) → produces string");
            Console.WriteLine("  MultiSourceAgent (depends on BOTH DataSourceA and DataSourceB)");
            Console.WriteLine("    └─> Consumes from multiple sources and combines them\n");

            // Test Sequential Runner
            Console.WriteLine("--- Sequential Runner ---");
            var sw1 = Stopwatch.StartNew();

            DataSourceA sourceA_seq = new DataSourceA("DataSourceA");
            DataSourceB sourceB_seq = new DataSourceB("DataSourceB");
            MultiSourceAgent multiAgent_seq = new MultiSourceAgent("MultiSourceAgent");

            Runner sequentialRunner = new Runner();
            sequentialRunner.AddAgent(sourceA_seq);
            sequentialRunner.AddAgent(sourceB_seq);
            sequentialRunner.AddAgent(multiAgent_seq);

            sequentialRunner.Run(Schedule.RunOnce);
            sw1.Stop();

            Console.WriteLine($"\nSequential Runner Total Time: {sw1.ElapsedMilliseconds}ms\n");

            // Small delay between tests
            System.Threading.Thread.Sleep(500);

            // Test Parallel Runner
            Console.WriteLine("\n--- Parallel Runner ---");
            var sw2 = Stopwatch.StartNew();

            DataSourceA sourceA_par = new DataSourceA("DataSourceA");
            DataSourceB sourceB_par = new DataSourceB("DataSourceB");
            MultiSourceAgent multiAgent_par = new MultiSourceAgent("MultiSourceAgent");

            ParallelRunner parallelRunner = new ParallelRunner();
            parallelRunner.AddAgent(sourceA_par);
            parallelRunner.AddAgent(sourceB_par);
            parallelRunner.AddAgent(multiAgent_par);

            parallelRunner.Run(Schedule.RunOnce);
            sw2.Stop();

            Console.WriteLine($"\nParallel Runner Total Time: {sw2.ElapsedMilliseconds}ms");
            Console.WriteLine($"Note: DataSourceA and DataSourceB run in parallel (Wave 1),");
            Console.WriteLine($"      MultiSourceAgent runs after both complete (Wave 2)\n");

            Console.WriteLine("========================================");
            Console.WriteLine("Multi-Source Consumption Summary");
            Console.WriteLine("========================================");
            Console.WriteLine("Agent successfully consumed from multiple sources");
            Console.WriteLine("Both runners support multi-source consumption");
            Console.WriteLine("ParallelRunner executed independent sources in parallel");
            Console.WriteLine("========================================\n");
        }
    }
}
