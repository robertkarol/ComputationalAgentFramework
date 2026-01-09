using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using Examples.Agents;
using System;
using System.Diagnostics;

namespace Examples
{
    public class ParallelRunnerDemo
    {
        public static void CompareRunners()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("Parallel Runner vs Sequential Runner Demo");
            Console.WriteLine("========================================\n");

            Console.WriteLine("Agent Dependency Graph:");
            Console.WriteLine("  SlowAgent1 (no dependencies)");
            Console.WriteLine("    ├─> SlowAgent2 (depends on SlowAgent1)");
            Console.WriteLine("    └─> SlowAgent3 (depends on SlowAgent1)");
            Console.WriteLine("  LooseAgent (no dependencies, no dependents) ← INDEPENDENT");
            Console.WriteLine("\nEach agent simulates 1 second of work.\n");

            // Test Sequential Runner
            Console.WriteLine("--- Sequential Runner ---");
            var sw1 = Stopwatch.StartNew();
            
            SlowAgent1 agent1_seq = new SlowAgent1("SlowAgent1");
            SlowAgent2 agent2_seq = new SlowAgent2("SlowAgent2");
            SlowAgent3 agent3_seq = new SlowAgent3("SlowAgent3");
            LooseAgent loose_seq = new LooseAgent("LooseAgent");
            
            Runner sequentialRunner = new Runner();
            sequentialRunner.AddAgent(agent1_seq);
            sequentialRunner.AddAgent(agent2_seq);
            sequentialRunner.AddAgent(agent3_seq);
            sequentialRunner.AddAgent(loose_seq);
            
            sequentialRunner.Run(Schedule.RunOnce);
            sw1.Stop();
            
            Console.WriteLine($"\nSequential Runner Total Time: {sw1.ElapsedMilliseconds}ms");
            Console.WriteLine($"Expected: ~4000ms (4 agents × 1000ms each)\n");

            // Small delay between tests
            System.Threading.Thread.Sleep(500);

            // Test Parallel Runner
            Console.WriteLine("\n--- Parallel Runner ---");
            var sw2 = Stopwatch.StartNew();
            
            SlowAgent1 agent1_par = new SlowAgent1("SlowAgent1");
            SlowAgent2 agent2_par = new SlowAgent2("SlowAgent2");
            SlowAgent3 agent3_par = new SlowAgent3("SlowAgent3");
            LooseAgent loose_par = new LooseAgent("LooseAgent");
            
            ParallelRunner parallelRunner = new ParallelRunner();
            parallelRunner.AddAgent(agent1_par);
            parallelRunner.AddAgent(agent2_par);
            parallelRunner.AddAgent(agent3_par);
            parallelRunner.AddAgent(loose_par);
            
            parallelRunner.Run(Schedule.RunOnce);
            sw2.Stop();
            
            Console.WriteLine($"\nParallel Runner Total Time: {sw2.ElapsedMilliseconds}ms");
            Console.WriteLine($"Expected: ~2000ms (Wave 1: SlowAgent1+LooseAgent parallel=1000ms, Wave 2: SlowAgent2+SlowAgent3 parallel=1000ms)\n");

            // Performance Summary
            Console.WriteLine("========================================");
            Console.WriteLine("Performance Summary");
            Console.WriteLine("========================================");
            Console.WriteLine($"Sequential Runner: {sw1.ElapsedMilliseconds}ms");
            Console.WriteLine($"Parallel Runner:   {sw2.ElapsedMilliseconds}ms");
            
            if (sw1.ElapsedMilliseconds > 0)
            {
                double speedup = (double)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds;
                double improvement = ((sw1.ElapsedMilliseconds - sw2.ElapsedMilliseconds) / (double)sw1.ElapsedMilliseconds) * 100;
                Console.WriteLine($"Speedup:           {speedup:F2}x");
                Console.WriteLine($"Time Saved:        {improvement:F1}%");
            }
            
            Console.WriteLine("\nNote: LooseAgent has no dependencies and no dependents.");
            Console.WriteLine("   It runs in Wave 1 alongside SlowAgent1 in ParallelRunner!");
            Console.WriteLine("========================================\n");
        }
    }
}
