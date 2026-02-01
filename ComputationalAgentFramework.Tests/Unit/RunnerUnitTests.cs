using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Tests.TestAgents;

namespace ComputationalAgentFramework.Tests.Unit
{
    public class RunnerUnitTests
    {
        [Fact]
        public void AddAgent_WithValidAgent_ShouldAddSuccessfully()
        {
            // Arrange
            var runner = new Runner();
            var agent = new TestAgent("MockAgent", 42);

            // Act
            var exception = Record.Exception(() => runner.AddAgent(agent));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void AddAgent_WithMultipleInstancesOfSameType_ShouldAllowThem()
        {
            // Arrange
            var runner = new Runner();
            var agent1 = new TestAgent("Agent1", 10);
            var agent2 = new TestAgent("Agent2", 20);

            // Act
            runner.AddAgent(agent1);
            var exception = Record.Exception(() => runner.AddAgent(agent2));

            // Assert - Should now allow multiple instances
            Assert.Null(exception);
        }

        [Fact]
        public void Run_WithRunOnceSchedule_ShouldInitializeExecuteAndFinishAgent()
        {
            // Arrange
            var runner = new Runner();
            var agent = new TestAgent("TestAgent", 42);

            runner.AddAgent(agent);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            Assert.True(agent.InitializeCalled);
            Assert.True(agent.ExecuteCalled);
            Assert.True(agent.FinishCalled);
            Assert.Equal(42, agent.ProducedData);
        }

        [Fact]
        public void Run_WithNoAgents_ShouldNotThrow()
        {
            // Arrange
            var runner = new Runner();

            // Act
            var exception = Record.Exception(() => runner.Run(Schedule.RunOnce));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Run_WithMultipleIndependentAgents_ShouldExecuteAllAgents()
        {
            // Arrange
            var runner = new Runner();
            var agent1 = new TestAgent("Agent1", 10);
            var agent2 = new TestAgent2("Agent2", 20);

            runner.AddAgent(agent1);
            runner.AddAgent(agent2);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            Assert.True(agent1.ExecuteCalled);
            Assert.True(agent2.ExecuteCalled);
            Assert.Equal(10, agent1.ProducedData);
            Assert.Equal(20, agent2.ProducedData);
        }

        [Fact]
        public void Run_WithDependentAgents_ShouldRespectExecutionOrder()
        {
            // Arrange
            var runner = new Runner();
            var producer = new TestAgent("Producer", 21);
            var consumer = new DependentTestAgent("Consumer");

            runner.AddAgent(producer);
            runner.AddAgent(consumer);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            Assert.True(producer.ExecuteCalled);
            Assert.True(consumer.ExecuteCalled);
            Assert.Equal(21, consumer.ConsumedValue);
            Assert.Equal(42, consumer.ProducedData); // DependentTestAgent produces ConsumedValue * 2
        }
    }
}
