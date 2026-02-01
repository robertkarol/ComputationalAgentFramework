using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;

namespace ComputationalAgentFramework.Tests.TestAgents
{
    [ConsumesFrom(typeof(TestStreamProducer))]
    public class TestStreamConsumer : StreamConsumerAgent<int, int>
    {
        public bool InitializeCalled { get; private set; }
        public bool FinishCalled { get; private set; }
        public int ExecuteCount { get; private set; }
        public int Sum { get; private set; }
        public int ItemCount { get; private set; }

        public TestStreamConsumer(string name) : base(name)
        {
        }

        public override void ConsumeStreamItem(int item)
        {
            Sum += item;
            ItemCount++;
        }

        public override int Produce()
        {
            return Sum;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCount++;
        }

        protected override void InitializeStream()
        {
            Sum = 0;
            ItemCount = 0;
            InitializeCalled = true;
        }

        protected override void FinalizeStream()
        {
            FinishCalled = true;
        }
    }
}
