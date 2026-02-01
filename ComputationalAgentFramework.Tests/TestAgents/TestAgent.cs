using ComputationalAgentFramework.Agent;

namespace ComputationalAgentFramework.Tests.TestAgents
{
    public class TestAgent : ComputationalAgent<int, int>
    {
        public bool InitializeCalled { get; private set; }
        public bool ExecuteCalled { get; private set; }
        public bool FinishCalled { get; private set; }
        public int ConsumedValue { get; private set; }
        public int ProducedValue { get; private set; }

        public TestAgent(string name, int producedValue = 42) : base(name)
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
            return ProducedValue;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }
    }
}
