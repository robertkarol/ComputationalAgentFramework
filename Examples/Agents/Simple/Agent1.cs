using ComputationalAgentFramework;
using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;
using System;

namespace Examples.Agents.Simple
{
    //[ConsumesFrom(typeof(Agent3))] // Uncomment this for a cyclic dependency
    public class Agent1 : ComputationalAgent<int, int>
    {
        private int _valueToProduce;

        public Agent1(string name) : base(name)
        {
        }

        public override void Consume(int consumedData)
        {
        }

        public override void Finish()
        {
            Console.WriteLine("Agent 1 was finished");
        }

        public override void Initialize()
        {
            _valueToProduce = 0;
            Console.WriteLine("Agent 1 was initialized");
        }

        public override int Produce()
        {
            return _valueToProduce;
        }

        protected override void ExecuteComputation()
        {
            Console.WriteLine("Enter a number:");
            _valueToProduce = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Agent 1 was executed");
        }
    }
}
