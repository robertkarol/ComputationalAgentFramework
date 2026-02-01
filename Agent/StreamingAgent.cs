using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ComputationalAgentFramework.Agent
{
    public abstract class StreamingAgent : ComputationalAgent, IStreamingAgent
    {
        private bool _streamComplete;

        public bool HasMoreData => !_streamComplete;

        public void SignalStreamComplete()
        {
            _streamComplete = true;
        }

        protected void ResetStream()
        {
            _streamComplete = false;
        }
    }

    public abstract class StreamProducerAgent<TProduced> : StreamingAgent, IStreamProducer<TProduced>
    {
        private string _name;
        private IEnumerator<TProduced> _streamEnumerator;
        private bool _streamInitialized;

        public StreamProducerAgent(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        protected abstract IEnumerable<TProduced> GenerateStream();

        public IEnumerable<TProduced> ProduceStream()
        {
            return GenerateStream();
        }

        public abstract TProduced Produce();

        protected abstract void ExecuteComputation();

        public override void Execute()
        {
            if (!_streamInitialized)
            {
                _streamEnumerator = GenerateStream().GetEnumerator();
                _streamInitialized = true;
            }

            if (_streamEnumerator.MoveNext())
            {
                ProducedData = _streamEnumerator.Current;
                ExecuteComputation();
            }
            else
            {
                SignalStreamComplete();
            }
        }

        public override void Initialize()
        {
            _streamInitialized = false;
            ResetStream();
            InitializeStream();
        }

        protected abstract void InitializeStream();

        public override void Finish()
        {
            _streamEnumerator?.Dispose();
            FinalizeStream();
        }

        protected abstract void FinalizeStream();
    }

    public abstract class StreamConsumerAgent<TConsumed, TProduced> : StreamingAgent, IStreamConsumer<TConsumed>, IProducer<TProduced>
    {
        private string _name;
        private ConcurrentQueue<TConsumed> _streamQueue;
        private bool _producerComplete;

        public StreamConsumerAgent(string name)
        {
            _name = name;
            _streamQueue = new ConcurrentQueue<TConsumed>();
        }

        public override string ToString()
        {
            return _name;
        }

        public void Consume(TConsumed consumedData)
        {
            _streamQueue.Enqueue(consumedData);
        }

        public void NotifyProducerComplete()
        {
            _producerComplete = true;
        }

        public abstract void ConsumeStreamItem(TConsumed item);

        public abstract TProduced Produce();

        protected abstract void ExecuteComputation();

        public override void Execute()
        {
            if (_streamQueue.TryDequeue(out var item))
            {
                ConsumeStreamItem(item);
                ExecuteComputation();
                ProducedData = Produce();
            }
            else if (_producerComplete && _streamQueue.IsEmpty)
            {
                SignalStreamComplete();
            }
        }

        public override void Initialize()
        {
            _streamQueue = new ConcurrentQueue<TConsumed>();
            _producerComplete = false;
            ResetStream();
            InitializeStream();
        }

        protected abstract void InitializeStream();

        public override void Finish()
        {
            FinalizeStream();
        }

        protected abstract void FinalizeStream();
    }
}
