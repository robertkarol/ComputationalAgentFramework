using ComputationalAgentFramework;
using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;
using System;

namespace Examples.Agents.Simple
{
    [ConsumesFrom(typeof(Agent2))]
    public class Agent3 : ComputationalAgent<int, int>
    {
        private int _data;

        public Agent3(string name) : base(name)
        {
        }

        public override void Consume(int consumedData)
        {
            _data = consumedData;
        }

        public override void Finish()
        {
            Console.WriteLine("Agent 3 was finished");
        }

        public override void Initialize()
        {
            Console.WriteLine("Agent 3 was initialized");
        }

        public override int Produce()
        {
            return _data * 4;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine(_data);
            Console.WriteLine("Agent 3 was executed");
        }
    }
}
