using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Agent
{
    public interface IComputationalAgent : IStateMachineAgent
    {
    }

    public interface IComputationalAgent<TConsumed, TProduced> : IComputationalAgent, IConsumer<TConsumed>, IProducer<TProduced>
    {
    }
}
