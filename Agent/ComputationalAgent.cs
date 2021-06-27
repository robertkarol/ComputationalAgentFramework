using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Agent
{
    public abstract class ComputationalAgent : IComputationalAgent
    {
        public object ToConsumeData { get; set; }

        public object ProducedData { get; protected set; }

        public abstract void Execute();

        public abstract void Finish();

        public abstract void Initialize();
    }

    public abstract class ComputationalAgent<TConsumed, TProduced> : ComputationalAgent, IComputationalAgent<TConsumed, TProduced>
    {
        private string _name;

        public ComputationalAgent(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public abstract void Consume(TConsumed consumedData);

        public abstract TProduced Produce();

        protected abstract void ExecuteComputation();

        public override void Execute()
        {
            if (ToConsumeData != null)
            {
                Consume((TConsumed)ToConsumeData);
            }
            ExecuteComputation();
            ProducedData = Produce();
        }

    }
}
