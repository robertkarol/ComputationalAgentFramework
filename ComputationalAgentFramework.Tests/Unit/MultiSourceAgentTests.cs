using ComputationalAgentFramework.Tests.TestAgents;

namespace ComputationalAgentFramework.Tests.Unit
{
    public class MultiSourceAgentTests
    {
        [Fact]
        public void MultiSourceAgent_ShouldConsumeFromMultipleSources()
        {
            var source1 = new TestAgent("Source1", 10);
            var source2 = new DependentTestAgent("Source2", 20);
            var multiAgent = new MultiSourceTestAgent("MultiAgent");
            
            source1.Execute();
            source2.ToConsumeData = 5;
            source2.Execute();
            
            multiAgent.ToConsumeDataSources[typeof(TestAgent)] = source1.ProducedData;
            multiAgent.ToConsumeDataSources[typeof(DependentTestAgent)] = source2.ProducedData;
            multiAgent.Execute();
            
            Assert.Equal(2, multiAgent.ConsumedData.Count);
            Assert.Equal(10, multiAgent.ConsumedData[typeof(TestAgent)]);
            Assert.Equal(10, multiAgent.ConsumedData[typeof(DependentTestAgent)]);
        }

        [Fact]
        public void MultiSourceAgent_ShouldInitializeEmptyDictionary()
        {
            var agent = new MultiSourceTestAgent("MultiAgent");
            
            Assert.NotNull(agent.ToConsumeDataSources);
            Assert.Empty(agent.ToConsumeDataSources);
        }

        [Fact]
        public void MultiSourceAgent_Execute_ShouldCallLifecycleMethods()
        {
            var agent = new MultiSourceTestAgent("MultiAgent");
            
            agent.Initialize();
            agent.Execute();
            agent.Finish();
            
            Assert.True(agent.InitializeCalled);
            Assert.True(agent.ExecuteCalled);
            Assert.True(agent.FinishCalled);
        }

        [Fact]
        public void MultiSourceAgent_ShouldProduceAggregatedData()
        {
            var source1 = new TestAgent("Source1", 10);
            var source2 = new DependentTestAgent("Source2", 20);
            var multiAgent = new MultiSourceTestAgent("MultiAgent");
            
            source1.Execute();
            source2.ToConsumeData = 10; // Dependent needs to consume to produce correctly
            source2.Execute();
            
            multiAgent.ToConsumeDataSources[typeof(TestAgent)] = source1.ProducedData;
            multiAgent.ToConsumeDataSources[typeof(DependentTestAgent)] = source2.ProducedData;
            multiAgent.Execute();
            
            var result = (string)multiAgent.ProducedData;
            Assert.Contains("10", result);
            Assert.Contains("20", result); // DependentTestAgent produces 10*2=20
        }

        [Fact]
        public void MultiSourceAgent_WithEmptySources_ShouldExecute()
        {
            var agent = new MultiSourceTestAgent("MultiAgent");
            
            var exception = Record.Exception(() => agent.Execute());
            
            Assert.Null(exception);
        }
    }
}
