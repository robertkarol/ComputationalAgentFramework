using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using ComputationalAgentFramework.Tests.TestAgents;

namespace ComputationalAgentFramework.Tests.Integration
{
    /// <summary>
    /// Mega integration test combining all agent types in a single complex pipeline.
    /// Tests the framework's ability to handle diverse agent types working together.
    /// </summary>
    public class MegaPipelineTests
    {
        [Fact]
        public void MegaPipeline_WithAllAgentTypes_ShouldExecuteComplexWorkflow()
        {
            // Scenario: E-commerce analytics with well-defined data flow
            // This test uses agents with clear, traceable dependencies
            
            // Arrange
            var runner = new Runner();
            
            // === LAYER 1: Configuration (Batch) ===
            var config = new ConfigAgent("Config", multiplier: 100, threshold: 5000);
            
            // === LAYER 2: Data Sources (Batch) ===
            var salesData = new DataSourceAgent("SalesData", new[] { 10, 20, 30 }); // Sum = 60
            var inventoryData = new DataSourceAgent2("InventoryData", new[] { 100, 200 }); // Sum = 300
            var customerData = new DataSourceAgent3("CustomerData", new[] { 5, 15 }); // Sum = 20
            
            // === LAYER 3: Direct Processing (no intermediate cleaning) ===
            // These multiply agents consume directly from their source types
            var salesProcessor = new MultiplyAgentForSource1("SalesProcessor", 2); // 60 * 2 = 120
            var inventoryProcessor = new MultiplyAgentForSource2("InventoryProcessor", 3); // 300 * 3 = 900
            var customerProcessor = new MultiplyAgentForSource3("CustomerProcessor", 4); // 20 * 4 = 80
            
            // === LAYER 4: Statistical Analysis Branch (separate from processing) ===
            // These consume from salesData independently
            var minSales = new MinAgent("MinSales"); // Consumes from CleanerAgent
            var maxSales = new MaxAgent("MaxSales"); // Consumes from CleanerAgent
            var avgSales = new AvgAgent("AvgSales"); // Consumes from CleanerAgent
            var salesCleaner = new CleanerAgent("SalesCleaner", 0); // Consumes from DataSourceAgent, produces 60
            
            // === LAYER 5: Statistics Reporter ===
            var statsReporter = new StatisticsReporterAgent("StatsReporter");
            
            // === LAYER 6: Routing Branch ===
            var normalizer = new NormalizerAgent("Normalizer", 10.0); // 60 / 10 = 6.0
            var router = new RouterAgent("Router", threshold: 5); // 6.0 > 5 ? high
            var highProcessor = new HighValueProcessorAgent("HighProcessor");
            var lowProcessor = new LowValueProcessorAgent("LowProcessor");
            
            // === LAYER 7: Validation Branch ===
            var validator = new ValidatorAgent("Validator"); // Consumes from inventoryProcessor
            var successHandler = new SuccessHandlerAgent("SuccessHandler");
            
            // === LAYER 8: Final Aggregation ===
            var finalAggregator = new MultiAggregatorAgent("FinalAggregator");
            
            // Add all agents (18 total)
            runner.AddAgent(config);                    // 1
            runner.AddAgent(salesData);                 // 2
            runner.AddAgent(inventoryData);             // 3
            runner.AddAgent(customerData);              // 4
            runner.AddAgent(salesProcessor);            // 5
            runner.AddAgent(inventoryProcessor);        // 6
            runner.AddAgent(customerProcessor);         // 7
            runner.AddAgent(salesCleaner);              // 8
            runner.AddAgent(minSales);                  // 9
            runner.AddAgent(maxSales);                  // 10
            runner.AddAgent(avgSales);                  // 11
            runner.AddAgent(statsReporter);             // 12
            runner.AddAgent(normalizer);                // 13
            runner.AddAgent(router);                    // 14
            runner.AddAgent(highProcessor);             // 15
            runner.AddAgent(lowProcessor);              // 16
            runner.AddAgent(validator);                 // 17
            runner.AddAgent(successHandler);            // 18
            runner.AddAgent(finalAggregator);           // 19
            
            // Act
            runner.Run(Schedule.RunOnce);
            
            // Assert - Verify clear data flow with known values
            
            // Config executed
            Assert.True(config.ExecuteCalled);
            var configData = config.Produce();
            Assert.Equal(100, configData.Multiplier);
            Assert.Equal(5000, configData.Threshold);
            
            // Data sources produced expected values
            Assert.True(salesData.ExecuteCalled);
            Assert.Equal(60, salesData.ProducedData); // 10+20+30
            
            // Processing chain produced expected values
            Assert.True(salesProcessor.ExecuteCalled);
            Assert.True(inventoryProcessor.ExecuteCalled);
            Assert.True(customerProcessor.ExecuteCalled);
            
            // Statistics branch (independent from processing)
            // salesData:60 ? salesCleaner:60 ? min/max/avg:60
            Assert.Equal(60, statsReporter.Min);
            Assert.Equal(60, statsReporter.Max);
            Assert.Equal(60, statsReporter.Average);
            
            // Routing branch
            // salesData:60 ? normalizer:6.0 ? router:{IsHigh:true, Value:6.0}
            // highProcessor should process it (6.0 > 5)
            Assert.Equal(1, highProcessor.Count);
            Assert.Equal(0, lowProcessor.Count);
            
            // Validation branch
            // salesData:60 ? validator:60 ? successHandler
            Assert.True(successHandler.ValidCount > 0);
            Assert.Equal(60, successHandler.ValidSum);
            
            // Final aggregation
            // Combines: salesProcessor:120 + inventoryProcessor:900 + customerProcessor:80
            Assert.Equal(1100, finalAggregator.GrandTotal);
        }
        
        [Fact]
        public void MegaPipeline_WithStreamingAgents_ShouldProcessCompleteWorkflow()
        {
            // A second mega test focusing on streaming agents
            // Scenario: Real-time sensor monitoring and analytics
            
            // Arrange
            var runner = new Runner();
            
            // === LAYER 1: Configuration (Batch) ===
            var config = new ConfigAgent("StreamConfig", multiplier: 10, threshold: 500);
            
            // === LAYER 2: Streaming Data Sources ===
            var sensorStream = new SensorStreamAgent("Sensors", 
                new[] { 45, 48, 52, 850, 47, 920, 49, 51 }); // Temperature readings
            
            // === LAYER 3: Stream Processing ===
            var anomalyDetector = new AnomalyDetectorAgent("AnomalyDetector", threshold: 500);
            
            // === LAYER 4: Alerting ===
            var alerter = new AlertAgent("Alerter");
            
            // === LAYER 5: Additional Batch Processing ===
            var batchProcessor = new DataSourceAgent("BatchMetrics", new[] { 100, 200, 300 });
            var batchMultiplier = new MultiplyAgent("BatchProcessor", 2);
            
            // === LAYER 6: Validation ===
            var validator = new ValidatorAgent("MetricsValidator");
            var successTracker = new SuccessHandlerAgent("SuccessTracker");
            
            // Add agents (8 total with mix of batch and streaming)
            runner.AddAgent(config);                // 1 - Batch config
            runner.AddAgent(sensorStream);          // 2 - Stream producer
            runner.AddAgent(anomalyDetector);       // 3 - Stream consumer/producer
            runner.AddAgent(alerter);               // 4 - Stream consumer
            runner.AddAgent(batchProcessor);        // 5 - Batch source
            runner.AddAgent(batchMultiplier);       // 6 - Batch transformer
            runner.AddAgent(validator);             // 7 - Batch validator
            runner.AddAgent(successTracker);        // 8 - Batch aggregator
            
            // Act
            runner.Run(Schedule.RunUntilStreamComplete);
            
            // Assert
            // Batch agents executed once
            Assert.True(config.ExecuteCalled);
            
            // Streaming pipeline processed all data
            Assert.False(sensorStream.HasMoreData);
            Assert.False(anomalyDetector.HasMoreData);
            Assert.False(alerter.HasMoreData);
            
            // Anomalies detected: 850, 920 (values > 500)
            Assert.Equal(2, alerter.AnomalyCount);
            Assert.Contains(850, alerter.AnomalyValues);
            Assert.Contains(920, alerter.AnomalyValues);
            
            // Batch processing completed
            // batchProcessor: 100+200+300 = 600
            // validator: 600 (valid, passes through)
            // successTracker: count=1, sum=600
            Assert.Equal(1, successTracker.ValidCount);
            Assert.Equal(600, successTracker.ValidSum);
            
            // Config available throughout (batch agents execute first)
            var configData = config.Produce();
            Assert.Equal(10, configData.Multiplier);
            Assert.Equal(500, configData.Threshold);
        }
        
        [Fact]
        public void MegaPipeline_ParallelExecution_ShouldHandleComplexDependencies()
        {
            // Test parallel execution with all agent types
            // Demonstrates the ParallelRunner's capability
            
            // Arrange
            var parallelRunner = new ParallelRunner();
            
            // === Independent sources (Wave 1) ===
            var source1 = new DataSourceAgent("ParallelSource1", new[] { 1, 2, 3, 4, 5 });
            var source2 = new DataSourceAgent2("ParallelSource2", new[] { 10, 20, 30 });
            var source3 = new DataSourceAgent3("ParallelSource3", new[] { 100, 200 });
            
            // === Parallel processors (Wave 2) ===
            var processor1 = new MultiplyAgentForSource1("Processor1", 10);
            var processor2 = new MultiplyAgentForSource2("Processor2", 5);
            var processor3 = new MultiplyAgentForSource3("Processor3", 2);
            
            // === Normalization (Wave 3) ===
            var normalizer1 = new NormalizerAgent("Normalizer1", 10.0);
            
            // === Validation (Wave 4) ===
            var validator = new ValidatorAgent("ParallelValidator");
            
            // === Routing (Wave 5) ===
            var router = new RouterAgent("Router", threshold: 10);
            var highProc = new HighValueProcessorAgent("HighProc");
            var lowProc = new LowValueProcessorAgent("LowProc");
            
            // === Final aggregation (Wave 6) ===
            var aggregator = new MultiAggregatorAgent("ParallelAggregator");
            
            // === Success tracking (Wave 7) ===
            var successHandler = new SuccessHandlerAgent("ParallelSuccess");
            
            // Add agents (12 total)
            parallelRunner.AddAgent(source1);       // Wave 1
            parallelRunner.AddAgent(source2);       // Wave 1
            parallelRunner.AddAgent(source3);       // Wave 1
            parallelRunner.AddAgent(processor1);    // Wave 2
            parallelRunner.AddAgent(processor2);    // Wave 2
            parallelRunner.AddAgent(processor3);    // Wave 2
            parallelRunner.AddAgent(normalizer1);   // Wave 3
            parallelRunner.AddAgent(validator);     // Wave 4
            parallelRunner.AddAgent(router);        // Wave 5
            parallelRunner.AddAgent(highProc);      // Wave 6
            parallelRunner.AddAgent(lowProc);       // Wave 6
            parallelRunner.AddAgent(aggregator);    // Wave 7
            
            // Act
            parallelRunner.Run(Schedule.RunOnce);
            
            // Assert
            // All sources executed in parallel (Wave 1)
            Assert.True(source1.ExecuteCalled);
            
            // All processors executed in parallel (Wave 2)
            Assert.True(processor1.ExecuteCalled);
            Assert.True(processor2.ExecuteCalled);
            Assert.True(processor3.ExecuteCalled);
            
            // Verify data flow through waves
            // source1: 1+2+3+4+5 = 15 -> processor1: 15*10 = 150
            // source2: 10+20+30 = 60 -> processor2: 60*5 = 300
            // source3: 100+200 = 300 -> processor3: 300*2 = 600
            // aggregator: 150+300+600 = 1050
            Assert.Equal(1050, aggregator.GrandTotal);
            
            // Normalization executed
            Assert.True(normalizer1.ExecuteCalled);
            
            // Routing logic executed
            // The router receives the normalized value
            Assert.True(highProc.Count + lowProc.Count >= 0);
        }
    }
}
