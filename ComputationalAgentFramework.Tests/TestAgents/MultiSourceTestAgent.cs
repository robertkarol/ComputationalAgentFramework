using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;

namespace ComputationalAgentFramework.Tests.TestAgents
{
    [ConsumesFrom(typeof(TestAgent))]
    [ConsumesFrom(typeof(DependentTestAgent))]
    public class MultiSourceTestAgent : MultiSourceComputationalAgent<string>
    {
        public bool InitializeCalled { get; private set; }
        public bool ExecuteCalled { get; private set; }
        public bool FinishCalled { get; private set; }
        public Dictionary<Type, object> ConsumedData { get; private set; }

        public MultiSourceTestAgent(string name) : base(name)
        {
            ConsumedData = new Dictionary<Type, object>();
        }

        public override void ConsumeMultiple(IDictionary<Type, object> consumedData)
        {
            foreach (var kvp in consumedData)
            {
                ConsumedData[kvp.Key] = kvp.Value;
            }
        }

        public override void Finish()
        {
            FinishCalled = true;
        }

        public override void Initialize()
        {
            InitializeCalled = true;
        }

        public override string Produce()
        {
            var values = string.Join(",", ConsumedData.Values);
            return values;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }
    }
}
