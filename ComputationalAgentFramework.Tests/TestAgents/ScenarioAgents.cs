using ComputationalAgentFramework.Agent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputationalAgentFramework.Tests.TestAgents
{
    // ==================== DATA SOURCE AGENTS ====================
    
    public class DataSourceAgent : ComputationalAgent<int, int>
    {
        private readonly int[] _data;
        public bool ExecuteCalled { get; private set; }

        public DataSourceAgent(string name, int[] data) : base(name)
        {
            _data = data;
        }

        public override void Consume(int consumedData) { }
        public override void Initialize() { }
        public override void Finish() { }

        public override int Produce()
        {
            return _data.Sum();
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }
    }

    // Create unique types for parallel scenarios
    public class DataSourceAgent2 : DataSourceAgent
    {
        public DataSourceAgent2(string name, int[] data) : base(name, data) { }
    }

    public class DataSourceAgent3 : DataSourceAgent
    {
        public DataSourceAgent3(string name, int[] data) : base(name, data) { }
    }

    // ==================== TRANSFORMATION AGENTS ====================
    
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent))]
    public class NormalizerAgent : ComputationalAgent<int, double>
    {
        private readonly double _divisor;
        private int _inputValue;
        public bool ExecuteCalled { get; private set; }

        public NormalizerAgent(string name, double divisor) : base(name)
        {
            _divisor = divisor;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override double Produce()
        {
            return _inputValue / _divisor;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(NormalizerAgent))]
    public class FilterAgent : ComputationalAgent<double, double>
    {
        private readonly double _threshold;
        private double _inputValue;
        public bool ExecuteCalled { get; private set; }

        public FilterAgent(string name, double threshold) : base(name)
        {
            _threshold = threshold;
        }

        public override void Consume(double consumedData)
        {
            _inputValue = consumedData;
        }

        public override double Produce()
        {
            return _inputValue > _threshold ? _inputValue : 0;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(FilterAgent))]
    public class AggregatorAgent : ComputationalAgent<double, double>
    {
        private double _inputValue;
        public bool ExecuteCalled { get; private set; }
        public double Sum { get; private set; }
        public int Count { get; private set; }

        public AggregatorAgent(string name) : base(name) { }

        public override void Consume(double consumedData)
        {
            _inputValue = consumedData;
        }

        public override double Produce()
        {
            if (_inputValue > 0)
            {
                Sum += _inputValue;
                Count++;
            }
            return Sum;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize()
        {
            Sum = 0;
            Count = 0;
        }

        public override void Finish() { }
    }

    // ==================== FAN-OUT PATTERN AGENTS ====================
    
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent))]
    public class MultiplyAgent : ComputationalAgent<int, int>
    {
        private readonly int _multiplier;
        private int _inputValue;
        public bool ExecuteCalled { get; private set; }

        public MultiplyAgent(string name, int multiplier) : base(name)
        {
            _multiplier = multiplier;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            return _inputValue * _multiplier;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    // Create unique multiply agent types for fan-out scenarios
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent))]
    public class MultiplyAgent2 : ComputationalAgent<int, int>
    {
        private readonly int _multiplier;
        private int _inputValue;
        public bool ExecuteCalled { get; private set; }

        public MultiplyAgent2(string name, int multiplier) : base(name)
        {
            _multiplier = multiplier;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            return _inputValue * _multiplier;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent))]
    public class MultiplyAgent3 : ComputationalAgent<int, int>
    {
        private readonly int _multiplier;
        private int _inputValue;
        public bool ExecuteCalled { get; private set; }

        public MultiplyAgent3(string name, int multiplier) : base(name)
        {
            _multiplier = multiplier;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            return _inputValue * _multiplier;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    // Collectors for fan-out scenarios
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MultiplyAgent))]
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MultiplyAgent2))]
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MultiplyAgent3))]
    public class CollectorAgent : MultiSourceComputationalAgent<int>
    {
        public List<int> ReceivedResults { get; private set; }

        public CollectorAgent(string name) : base(name)
        {
            ReceivedResults = new List<int>();
        }

        public override void ConsumeMultiple(IDictionary<Type, object> consumedData)
        {
            ReceivedResults.Clear();
            foreach (var data in consumedData.Values)
            {
                if (data is int value)
                {
                    ReceivedResults.Add(value);
                }
            }
        }

        public override int Produce()
        {
            return ReceivedResults.Sum();
        }

        protected override void ExecuteComputation() { }

        public override void Initialize()
        {
            ReceivedResults = new List<int>();
        }

        public override void Finish() { }
    }

    // ==================== DIAMOND PATTERN AGENTS ====================
    
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MultiplyAgent))]
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MultiplyAgent2))]
    public class MergerAgent : MultiSourceComputationalAgent<int>
    {
        public int MergedResult { get; private set; }
        public bool ExecuteCalled { get; private set; }

        public MergerAgent(string name) : base(name) { }

        public override void ConsumeMultiple(IDictionary<Type, object> consumedData)
        {
            MergedResult = 0;
            foreach (var data in consumedData.Values)
            {
                if (data is int value)
                {
                    MergedResult += value;
                }
            }
        }

        public override int Produce()
        {
            return MergedResult;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { MergedResult = 0; }
        public override void Finish() { }
    }

    // ==================== ANALYTICS AGENTS ====================
    
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent))]
    public class CleanerAgent : ComputationalAgent<int, int>
    {
        private readonly int _minimumValue;
        private int _inputValue;

        public CleanerAgent(string name, int minimumValue) : base(name)
        {
            _minimumValue = minimumValue;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            // Return the full sum, we'll handle filtering in aggregation
            return _inputValue;
        }

        protected override void ExecuteComputation() { }
        public override void Initialize() { }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(CleanerAgent))]
    public class MinAgent : ComputationalAgent<int, int>
    {
        private int _inputValue;
        private int _min = int.MaxValue;

        public MinAgent(string name) : base(name) { }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            // In real scenario, we'd need to pass individual values, not sum
            // For now, just track the value we received
            if (_inputValue > 0 && _inputValue < _min)
            {
                _min = _inputValue;
            }
            return _min == int.MaxValue ? _inputValue : _min;
        }

        protected override void ExecuteComputation() { }
        public override void Initialize() { _min = int.MaxValue; }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(CleanerAgent))]
    public class MaxAgent : ComputationalAgent<int, int>
    {
        private int _inputValue;
        private int _max = int.MinValue;

        public MaxAgent(string name) : base(name) { }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            if (_inputValue > _max)
            {
                _max = _inputValue;
            }
            return _max;
        }

        protected override void ExecuteComputation() { }
        public override void Initialize() { _max = int.MinValue; }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(CleanerAgent))]
    public class AvgAgent : ComputationalAgent<int, double>
    {
        private int _inputValue;
        private int _sum = 0;
        private int _count = 0;

        public AvgAgent(string name) : base(name) { }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override double Produce()
        {
            if (_inputValue > 0)
            {
                _sum += _inputValue;
                _count++;
            }
            return _count > 0 ? (double)_sum / _count : 0;
        }

        protected override void ExecuteComputation() { }
        public override void Initialize() { _sum = 0; _count = 0; }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MinAgent))]
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MaxAgent))]
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(AvgAgent))]
    public class StatisticsReporterAgent : MultiSourceComputationalAgent<string>
    {
        public int Min { get; private set; }
        public int Max { get; private set; }
        public double Average { get; private set; }

        public StatisticsReporterAgent(string name) : base(name) { }

        public override void ConsumeMultiple(IDictionary<Type, object> consumedData)
        {
            if (consumedData.TryGetValue(typeof(MinAgent), out var minData))
            {
                Min = (int)minData;
            }
            if (consumedData.TryGetValue(typeof(MaxAgent), out var maxData))
            {
                Max = (int)maxData;
            }
            if (consumedData.TryGetValue(typeof(AvgAgent), out var avgData))
            {
                Average = (double)avgData;
            }
        }

        public override string Produce()
        {
            return $"Min:{Min},Max:{Max},Avg:{Average:F1}";
        }

        protected override void ExecuteComputation() { }
        public override void Initialize() { }
        public override void Finish() { }
    }

    // ==================== PARALLEL AGGREGATION AGENTS ====================
    
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent))]
    public class MultiplyAgentForSource1 : ComputationalAgent<int, int>
    {
        private readonly int _multiplier;
        private int _inputValue;
        public bool ExecuteCalled { get; private set; }

        public MultiplyAgentForSource1(string name, int multiplier) : base(name)
        {
            _multiplier = multiplier;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            return _inputValue * _multiplier;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent2))]
    public class MultiplyAgentForSource2 : ComputationalAgent<int, int>
    {
        private readonly int _multiplier;
        private int _inputValue;
        public bool ExecuteCalled { get; private set; }

        public MultiplyAgentForSource2(string name, int multiplier) : base(name)
        {
            _multiplier = multiplier;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            return _inputValue * _multiplier;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(DataSourceAgent3))]
    public class MultiplyAgentForSource3 : ComputationalAgent<int, int>
    {
        private readonly int _multiplier;
        private int _inputValue;
        public bool ExecuteCalled { get; private set; }

        public MultiplyAgentForSource3(string name, int multiplier) : base(name)
        {
            _multiplier = multiplier;
        }

        public override void Consume(int consumedData)
        {
            _inputValue = consumedData;
        }

        public override int Produce()
        {
            return _inputValue * _multiplier;
        }

        protected override void ExecuteComputation()
        {
            ExecuteCalled = true;
        }

        public override void Initialize() { }
        public override void Finish() { }
    }

    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MultiplyAgentForSource1))]
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MultiplyAgentForSource2))]
    [ComputationalAgentFramework.Utils.ConsumesFrom(typeof(MultiplyAgentForSource3))]
    public class MultiAggregatorAgent : MultiSourceComputationalAgent<int>
    {
        public int GrandTotal { get; private set; }

        public MultiAggregatorAgent(string name) : base(name) { }

        public override void ConsumeMultiple(IDictionary<Type, object> consumedData)
        {
            GrandTotal = 0;
            foreach (var data in consumedData.Values)
            {
                if (data is int value)
                {
                    GrandTotal += value;
                }
            }
        }

        public override int Produce()
        {
            return GrandTotal;
        }

        protected override void ExecuteComputation() { }
        public override void Initialize() { GrandTotal = 0; }
        public override void Finish() { }
    }
}
