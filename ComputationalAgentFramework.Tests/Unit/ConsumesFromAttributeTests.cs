using ComputationalAgentFramework.Utils;
using ComputationalAgentFramework.Tests.TestAgents;

namespace ComputationalAgentFramework.Tests.Unit
{
    public class ConsumesFromAttributeTests
    {
        [Fact]
        public void ConsumesFrom_ShouldStoreProducerType()
        {
            var attribute = new ConsumesFrom(typeof(TestAgent));
            
            Assert.Equal(typeof(TestAgent), attribute.Producer);
        }

        [Fact]
        public void ConsumesFrom_CanBeAppliedMultipleTimes()
        {
            var type = typeof(MultiSourceTestAgent);
            var attributes = type.GetCustomAttributes(typeof(ConsumesFrom), true);
            
            Assert.Equal(2, attributes.Length);
        }

        [Fact]
        public void ConsumesFrom_SingleAttribute_ShouldBeRetrievable()
        {
            var type = typeof(DependentTestAgent);
            var attributes = type.GetCustomAttributes(typeof(ConsumesFrom), true);
            
            Assert.Single(attributes);
            var consumesFrom = attributes[0] as ConsumesFrom;
            Assert.NotNull(consumesFrom);
            Assert.Equal(typeof(TestAgent), consumesFrom.Producer);
        }

        [Fact]
        public void ConsumesFrom_MultipleAttributes_ShouldAllBeRetrievable()
        {
            var type = typeof(MultiSourceTestAgent);
            var attributes = type.GetCustomAttributes(typeof(ConsumesFrom), true)
                .Cast<ConsumesFrom>()
                .ToList();
            
            Assert.Equal(2, attributes.Count);
            Assert.Contains(attributes, a => a.Producer == typeof(TestAgent));
            Assert.Contains(attributes, a => a.Producer == typeof(DependentTestAgent));
        }

        [Fact]
        public void ConsumesFrom_NoAttribute_ShouldReturnEmpty()
        {
            var type = typeof(TestAgent);
            var attributes = type.GetCustomAttributes(typeof(ConsumesFrom), true);
            
            Assert.Empty(attributes);
        }
    }
}
