using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ComputationalAgentFramework.Agent
{
    public interface IStreamingAgent : IComputationalAgent
    {
        bool HasMoreData { get; }
        void SignalStreamComplete();
    }

    public interface IStreamProducer<TProduced> : IStreamingAgent, IProducer<TProduced>
    {
        IEnumerable<TProduced> ProduceStream();
    }

    public interface IStreamConsumer<TConsumed> : IStreamingAgent, IConsumer<TConsumed>
    {
        void ConsumeStreamItem(TConsumed item);
    }
}
