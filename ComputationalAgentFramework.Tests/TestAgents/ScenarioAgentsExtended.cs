using ComputationalAgentFramework.Agent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputationalAgentFramework.Tests.TestAgents
{
    // ==================== STREAMING SCENARIO AGENTS ====================
    
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(TestStreamProducer))]
    public class StreamTransformerAgent : StreamConsumerAgent<int, int>
    {
        private readonly int _addValue;

        public StreamTransformerAgent(string name, int addValue) : base(name)
        {
            _addValue = addValue;
        }

        public override void ConsumeStreamItem(int item)
        {
            // Transform is done in Produce
        }

        public override int Produce()
        {
            if (ToConsumeData != null && ToConsumeData is int value)
            {
                return value + _addValue;
            }
            return 0;
        }

        protected override void ExecuteComputation() { }
        protected override void InitializeStream() { }
        protected override void FinalizeStream() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(StreamTransformerAgent))]
    public class StreamAccumulatorAgent : StreamConsumerAgent<int, int>
    {
        public int Total { get; private set; }
        public int ItemCount { get; private set; }

        public StreamAccumulatorAgent(string name) : base(name) { }

        public override void ConsumeStreamItem(int item)
        {
            Total += item;
            ItemCount++;
        }

        public override int Produce()
        {
            return Total;
        }

        protected override void ExecuteComputation() { }

        protected override void InitializeStream()
        {
            Total = 0;
            ItemCount = 0;
        }

        protected override void FinalizeStream() { }
    }

    // ==================== HYBRID BATCH/STREAM AGENTS ====================
    
    public class ConfigAgent : ComputationalAgent<int, ConfigData>
    {
        private readonly int _multiplier;
        private readonly int _threshold;
        public bool ExecuteCalled { get; private set; }

        public ConfigAgent(string name, int multiplier, int threshold) : base(name)
        {
            _multiplier = multiplier;
            _threshold = threshold;
        }

        public override void Consume(int consumedData) { }

        public override ConfigData Produce()
        {
            return new ConfigData { Multiplier = _multiplier, Threshold = _threshold };
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    public class ConfigData
    {
        public int Multiplier { get; set; }
        public int Threshold { get; set; }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(ConfigAgent))]
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(TestStreamProducer))]
    public class ConfigurableProcessorAgent : MultiSourceComputationalAgent<int>
    {
        private ConfigData _config;
        public int ProcessedCount { get; private set; }

        public ConfigurableProcessorAgent(string name) : base(name) { }

        public override void ConsumeMultiple(IDictionary<Type, object> consumedData)
        {
            if (consumedData.TryGetValue(typeof(ConfigAgent), out var configData))
            {
                _config = (ConfigData)configData;
            }

            if (consumedData.TryGetValue(typeof(TestStreamProducer), out var streamData) && streamData is int value)
            {
                if (_config != null)
                {
                    int processed = value * _config.Multiplier;
                    if (processed > _config.Threshold)
                    {
                        ProcessedCount++;
                    }
                }
            }
        }

        public override int Produce()
        {
            return ProcessedCount;
        }

        protected override void ExecuteComputation() { }

        public override void Initialize()
        {
            ProcessedCount = 0;
        }

        public override void Finish() { }
    }

    // ==================== ERROR HANDLING AGENTS ====================
    
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent))]
    public class ValidatorAgent : ComputationalAgent<int, int>
    {
        private int _inputValue;

        public ValidatorAgent(string name) : base(name) { }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            // Only pass through positive values
            return _inputValue > 0 ? _inputValue : 0;
        }

        protected override void ExecuteComputation() { }
        public override void Initialize() { }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(ValidatorAgent))]
    public class SuccessHandlerAgent : ComputationalAgent<int, int>
    {
        private int _inputValue;
        public int ValidCount { get; private set; }
        public int ValidSum { get; private set; }

        public SuccessHandlerAgent(string name) : base(name) { }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            if (_inputValue > 0)
            {
                ValidCount++;
                ValidSum += _inputValue;
            }
            return ValidSum;
        }

        protected override void ExecuteComputation() { }

        public override void Initialize()
        {
            ValidCount = 0;
            ValidSum = 0;
        }

        public override void Finish() { }
    }

    // ==================== PARALLEL AGGREGATION AGENTS ====================
    
    // ==================== BRANCHING LOGIC AGENTS ====================
    
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent))]
    public class RouterAgent : ComputationalAgent<int, RoutedData>
    {
        private readonly int _threshold;
        private int _inputValue;

        public RouterAgent(string name, int threshold) : base(name)
        {
            _threshold = threshold;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override RoutedData Produce()
        {
            return new RoutedData
            {
                IsHigh = _inputValue > _threshold,
                Value = _inputValue
            };
        }

        protected override void ExecuteComputation() { }
        public override void Initialize() { }
        public override void Finish() { }
    }

    public class RoutedData
    {
        public bool IsHigh { get; set; }
        public int Value { get; set; }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(RouterAgent))]
    public class HighValueProcessorAgent : ComputationalAgent<RoutedData, int>
    {
        private RoutedData _inputData;
        public int Count { get; private set; }
        private int _sum;

        public HighValueProcessorAgent(string name) : base(name) { }

        public override void Consume(RoutedData consumedData)
        {
            _inputData = consumedData;
        }

        public override int Produce()
        {
            if (_inputData != null && _inputData.IsHigh)
            {
                Count++;
                _sum += _inputData.Value;
            }
            return Count > 0 ? _sum / Count : 0;
        }

        protected override void ExecuteComputation() { }

        public override void Initialize()
        {
            Count = 0;
            _sum = 0;
        }

        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(RouterAgent))]
    public class LowValueProcessorAgent : ComputationalAgent<RoutedData, int>
    {
        private RoutedData _inputData;
        public int Count { get; private set; }
        private int _sum;

        public LowValueProcessorAgent(string name) : base(name) { }

        public override void Consume(RoutedData consumedData)
        {
            _inputData = consumedData;
        }

        public override int Produce()
        {
            if (_inputData != null && !_inputData.IsHigh)
            {
                Count++;
                _sum += _inputData.Value;
            }
            return Count > 0 ? _sum / Count : 0;
        }

        protected override void ExecuteComputation() { }

        public override void Initialize()
        {
            Count = 0;
            _sum = 0;
        }

        public override void Finish() { }
    }

    // ==================== MONITORING AGENTS ====================
    
    public class SensorStreamAgent : StreamProducerAgent<int>
    {
        private readonly int[] _readings;

        public SensorStreamAgent(string name, int[] readings) : base(name)
        {
            _readings = readings;
        }

        protected override IEnumerable<int> GenerateStream()
        {
            foreach (var reading in _readings)
            {
                yield return reading;
            }
        }

        public override int Produce()
        {
            return ProducedData != null ? (int)ProducedData : 0;
        }

        protected override void ExecuteComputation() { }
        protected override void InitializeStream() { }
        protected override void FinalizeStream() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(SensorStreamAgent))]
    public class AnomalyDetectorAgent : StreamConsumerAgent<int, AnomalyData>
    {
        private readonly int _threshold;
        private int _currentValue;

        public AnomalyDetectorAgent(string name, int threshold) : base(name)
        {
            _threshold = threshold;
        }

        public override void ConsumeStreamItem(int item)
        {
            _currentValue = item;
        }

        public override AnomalyData Produce()
        {
            return new AnomalyData
            {
                Value = _currentValue,
                IsAnomaly = _currentValue > _threshold
            };
        }

        protected override void ExecuteComputation() { }
        protected override void InitializeStream() { }
        protected override void FinalizeStream() { }
    }

    public class AnomalyData
    {
        public int Value { get; set; }
        public bool IsAnomaly { get; set; }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(AnomalyDetectorAgent))]
    public class AlertAgent : StreamConsumerAgent<AnomalyData, int>
    {
        public int AnomalyCount { get; private set; }
        public List<int> AnomalyValues { get; private set; } = new List<int>();
        private AnomalyData _currentData;

        public AlertAgent(string name) : base(name) { }

        public override void ConsumeStreamItem(AnomalyData item)
        {
            _currentData = item;
        }

        public override int Produce()
        {
            if (_currentData != null && _currentData.IsAnomaly)
            {
                AnomalyCount++;
                AnomalyValues.Add(_currentData.Value);
            }
            return AnomalyCount;
        }

        protected override void ExecuteComputation() { }

        protected override void InitializeStream()
        {
            AnomalyCount = 0;
            AnomalyValues = new List<int>();
        }

        protected override void FinalizeStream() { }
    }
}
