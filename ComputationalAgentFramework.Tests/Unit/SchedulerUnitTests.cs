using ComputationalAgentFramework.Framework.Scheduler;
using ComputationalAgentFramework.Agent;
using Moq;

namespace ComputationalAgentFramework.Tests.Unit
{
    public class SchedulerUnitTests
    {
        [Fact]
        public void RunOnceScheduler_FirstCall_ShouldReturnTrue()
        {
            // Arrange
            var scheduler = new RunOnceScheduler();

            // Act
            var result = scheduler.HasMoreEpochsToRun();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void RunOnceScheduler_AfterThickEpoch_ShouldReturnFalse()
        {
            // Arrange
            var scheduler = new RunOnceScheduler();

            // Act
            scheduler.ThickEpoch();
            var result = scheduler.HasMoreEpochsToRun();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void RunOnceScheduler_CanRun_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var scheduler = new RunOnceScheduler();

            // Act & Assert
            Assert.True(scheduler.CanRun());
            scheduler.ThickEpoch();
            Assert.True(scheduler.CanRun());
        }

        [Fact]
        public void RunUntilStreamCompleteScheduler_WithNoAgents_ShouldCompleteImmediately()
        {
            // Arrange
            var scheduler = new RunUntilStreamCompleteScheduler();
            var mockAgent = new Mock<IComputationalAgent>();

            // Act
            scheduler.NotifyEpochComplete(new[] { mockAgent.Object });

            // Assert
            Assert.False(scheduler.HasMoreEpochsToRun());
        }

        [Fact]
        public void RunUntilStreamCompleteScheduler_WithActiveStreamingAgent_ShouldContinue()
        {
            // Arrange
            var scheduler = new RunUntilStreamCompleteScheduler();
            var mockStreamingAgent = new Mock<IStreamingAgent>();
            mockStreamingAgent.Setup(a => a.HasMoreData).Returns(true);

            // Act
            scheduler.NotifyEpochComplete(new[] { mockStreamingAgent.Object });

            // Assert
            Assert.True(scheduler.HasMoreEpochsToRun());
        }

        [Fact]
        public void RunUntilStreamCompleteScheduler_WhenStreamCompletes_ShouldStopRunning()
        {
            // Arrange
            var scheduler = new RunUntilStreamCompleteScheduler();
            var mockStreamingAgent = new Mock<IStreamingAgent>();
            
            // Simulate stream completion
            mockStreamingAgent.Setup(a => a.HasMoreData).Returns(false);

            // Act
            scheduler.NotifyEpochComplete(new[] { mockStreamingAgent.Object });

            // Assert
            Assert.False(scheduler.HasMoreEpochsToRun());
        }

        [Fact]
        public void RunUntilStreamCompleteScheduler_WithMixedAgents_ShouldWaitForStreaming()
        {
            // Arrange
            var scheduler = new RunUntilStreamCompleteScheduler();
            var mockBatchAgent = new Mock<IComputationalAgent>();
            var mockStreamingAgent = new Mock<IStreamingAgent>();
            mockStreamingAgent.Setup(a => a.HasMoreData).Returns(true);

            // Act
            scheduler.NotifyEpochComplete(new IComputationalAgent[] { mockBatchAgent.Object, mockStreamingAgent.Object });

            // Assert - Should continue because streaming agent has more data
            Assert.True(scheduler.HasMoreEpochsToRun());
        }

        [Fact]
        public void SchedulerFactory_Create_WithRunOnce_ShouldReturnRunOnceScheduler()
        {
            // Arrange
            var factory = new SchedulerFactory();

            // Act
            var scheduler = factory.Create(Schedule.RunOnce);

            // Assert
            Assert.IsType<RunOnceScheduler>(scheduler);
        }

        [Fact]
        public void SchedulerFactory_Create_WithRunIndefinitely_ShouldReturnRunIndefinitelyScheduler()
        {
            // Arrange
            var factory = new SchedulerFactory();

            // Act
            var scheduler = factory.Create(Schedule.RunIndefinitely);

            // Assert
            Assert.IsType<RunIndefinitelyScheduler>(scheduler);
        }

        [Fact]
        public void SchedulerFactory_Create_WithRunUntilStreamComplete_ShouldReturnRunUntilStreamCompleteScheduler()
        {
            // Arrange
            var factory = new SchedulerFactory();

            // Act
            var scheduler = factory.Create(Schedule.RunUntilStreamComplete);

            // Assert
            Assert.IsType<RunUntilStreamCompleteScheduler>(scheduler);
        }
    }
}
