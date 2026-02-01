using ComputationalAgentFramework.Agent;
using System.Collections.Generic;

namespace ComputationalAgentFramework.Tests.TestAgents
{
    public class TestStreamProducer : StreamProducerAgent<int>
    {
        private readonly int _count;
        public bool InitializeCalled { get; private set; }
        public bool FinishCalled { get; private set; }
        public int ExecuteCount { get; private set; }

        public TestStreamProducer(string name, int count = 5) : base(name)
        {
            _count = count;
        }

        protected override IEnumerable<int> GenerateStream()
        {
            for (int i = 1; i <= _count; i++)
            {
                yield return i;
            }
        }

        public override int Produce()
        {
            return (int)ProducedData;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCount++;
        }

        protected override void InitializeStream()
        {
            InitializeCalled = true;
        }

        protected override void FinalizeStream()
        {
            FinishCalled = true;
        }
    }
}
