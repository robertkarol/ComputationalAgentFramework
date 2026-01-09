using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Agent
{
    public abstract class MultiSourceComputationalAgent : ComputationalAgent
    {
        public IDictionary<Type, object> ToConsumeDataSources { get; set; }

        protected MultiSourceComputationalAgent()
        {
            ToConsumeDataSources = new Dictionary<Type, object>();
        }
    }

    public abstract class MultiSourceComputationalAgent<TProduced> : MultiSourceComputationalAgent, IProducer<TProduced>
    {
        private string _name;

        public MultiSourceComputationalAgent(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public abstract void ConsumeMultiple(IDictionary<Type, object> consumedData);

        public abstract TProduced Produce();

        protected abstract void ExecuteComputation();

        public override void Execute()
        {
            if (ToConsumeDataSources != null && ToConsumeDataSources.Count > 0)
            {
                ConsumeMultiple(ToConsumeDataSources);
            }
            ExecuteComputation();
            ProducedData = Produce();
        }
    }
}
