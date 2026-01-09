using ComputationalAgentFramework;
using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;
using System;

namespace Examples
{
    [ConsumesFrom(typeof(Agent1))]
    public class Agent2 : ComputationalAgent<int, int>
    {
        private int _data;

        public Agent2(string name) : base(name)
        {
        }

        public override void Consume(int consumedData)
        {
            _data = consumedData;
        }

        public override void Finish()
        {
            Console.WriteLine("Agent 2 was finished");
        }

        public override void Initialize()
        {
            Console.WriteLine("Agent 2 was initialized");
        }

        public override int Produce()
        {
            return _data + 1;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine(_data);
            Console.WriteLine("Agent 2 was executed");
        }
    }
}
