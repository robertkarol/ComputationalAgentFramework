using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;

namespace ComputationalAgentFramework.Tests.TestAgents
{
    [ConsumesFrom(typeof(TestAgent))]
    public class DependentTestAgent : ComputationalAgent<int, int>
    {
        public bool InitializeCalled { get; private set; }
        public bool ExecuteCalled { get; private set; }
        public bool FinishCalled { get; private set; }
        public int ConsumedValue { get; private set; }
        public int ProducedValue { get; private set; }

        public DependentTestAgent(string name, int producedValue = 84) : base(name)
        {
            ProducedValue = producedValue;
        }

        public override void Consume(int consumedData)
        {
            ConsumedValue = consumedData;
        }

        public override void Finish()
        {
            FinishCalled = true;
        }

        public override void Initialize()
        {
            InitializeCalled = true;
        }

        public override int Produce()
        {
            return ConsumedValue * 2;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }
    }
}
