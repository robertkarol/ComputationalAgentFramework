using ComputationalAgentFramework.Tests.TestAgents;

namespace ComputationalAgentFramework.Tests.Unit
{
    public class ComputationalAgentTests
    {
        [Fact]
        public void Agent_ShouldHaveName()
        {
            var agent = new TestAgent("TestAgent");
            
            Assert.Equal("TestAgent", agent.ToString());
        }

        [Fact]
        public void Agent_Execute_ShouldCallLifecycleMethods()
        {
            var agent = new TestAgent("TestAgent", 42);
            
            agent.Initialize();
            agent.Execute();
            agent.Finish();
            
            Assert.True(agent.InitializeCalled);
            Assert.True(agent.ExecuteCalled);
            Assert.True(agent.FinishCalled);
        }

        [Fact]
        public void Agent_Execute_ShouldConsumeData()
        {
            var agent = new TestAgent("TestAgent");
            agent.ToConsumeData = 10;
            
            agent.Execute();
            
            Assert.Equal(10, agent.ConsumedValue);
        }

        [Fact]
        public void Agent_Execute_ShouldProduceData()
        {
            var agent = new TestAgent("TestAgent", 42);
            
            agent.Execute();
            
            Assert.Equal(42, agent.ProducedData);
        }

        [Fact]
        public void Agent_Execute_WithNullData_ShouldNotThrow()
        {
            var agent = new TestAgent("TestAgent");
            agent.ToConsumeData = null;
            
            var exception = Record.Exception(() => agent.Execute());
            
            Assert.Null(exception);
        }

        [Fact]
        public void Agent_Execute_ShouldCallExecuteComputation()
        {
            var agent = new TestAgent("TestAgent");
            
            agent.Execute();
            
            Assert.True(agent.ExecuteCalled);
        }

        [Fact]
        public void DependentAgent_ShouldConsumeFromProducer()
        {
            var producer = new TestAgent("Producer", 21);
            var consumer = new DependentTestAgent("Consumer");
            
            producer.Execute();
            consumer.ToConsumeData = producer.ProducedData;
            consumer.Execute();
            
            Assert.Equal(21, consumer.ConsumedValue);
            Assert.Equal(42, consumer.ProducedData);
        }
    }
}
