using ComputationalAgentFramework.Framework;
using ComputationalAgentFramework.Framework.Scheduler;
using ComputationalAgentFramework.Agent;
using ComputationalAgentFramework.Utils;
using System.Collections.Generic;
using System.Linq;

namespace ComputationalAgentFramework.Tests.Unit
{
    /// <summary>
    /// Tests to verify the framework works correctly with custom domain models and complex types
    /// </summary>
    public class CustomDomainModelTests
    {
        // Custom domain models
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Email { get; set; }
        }

        public class Order
        {
            public int OrderId { get; set; }
            public List<string> Items { get; set; }
            public decimal Total { get; set; }
        }

        public class ProcessedOrder
        {
            public int OrderId { get; set; }
            public int ItemCount { get; set; }
            public decimal Total { get; set; }
            public decimal Tax { get; set; }
            public decimal GrandTotal { get; set; }
        }

        public class SensorReading
        {
            public string SensorId { get; set; }
            public double Temperature { get; set; }
            public double Humidity { get; set; }
            public System.DateTime Timestamp { get; set; }
        }

        public class AggregatedData
        {
            public double AverageTemperature { get; set; }
            public double AverageHumidity { get; set; }
            public int ReadingCount { get; set; }
        }

        // Test agents using custom models
        public class PersonProducerAgent : ComputationalAgent<int, Person>
        {
            private readonly Person _person;

            public PersonProducerAgent(string name, Person person) : base(name)
            {
                _person = person;
            }

            public override void Consume(int consumedData) { }
            public override Person Produce() => _person;
            protected override void ExecuteComputation() { }
            public override void Initialize() { }
            public override void Finish() { }
        }

        [ConsumesFrom(typeof(PersonProducerAgent))]
        public class PersonValidatorAgent : ComputationalAgent<Person, bool>
        {
            private Person _person;

            public PersonValidatorAgent(string name) : base(name) { }

            public override void Consume(Person consumedData)
            {
                _person = consumedData;
            }

            public override bool Produce()
            {
                return _person != null && 
                       !string.IsNullOrEmpty(_person.Name) && 
                       _person.Age > 0 &&
                       !string.IsNullOrEmpty(_person.Email);
            }

            protected override void ExecuteComputation() { }
            public override void Initialize() { }
            public override void Finish() { }
        }

        public class OrderProducerAgent : ComputationalAgent<int, Order>
        {
            private readonly Order _order;

            public OrderProducerAgent(string name, Order order) : base(name)
            {
                _order = order;
            }

            public override void Consume(int consumedData) { }
            public override Order Produce() => _order;
            protected override void ExecuteComputation() { }
            public override void Initialize() { }
            public override void Finish() { }
        }

        [ConsumesFrom(typeof(OrderProducerAgent))]
        public class OrderProcessorAgent : ComputationalAgent<Order, ProcessedOrder>
        {
            private Order _order;
            private readonly decimal _taxRate;

            public OrderProcessorAgent(string name, decimal taxRate) : base(name)
            {
                _taxRate = taxRate;
            }

            public override void Consume(Order consumedData)
            {
                _order = consumedData;
            }

            public override ProcessedOrder Produce()
            {
                if (_order == null) return null;

                var tax = _order.Total * _taxRate;
                return new ProcessedOrder
                {
                    OrderId = _order.OrderId,
                    ItemCount = _order.Items?.Count ?? 0,
                    Total = _order.Total,
                    Tax = tax,
                    GrandTotal = _order.Total + tax
                };
            }

            protected override void ExecuteComputation() { }
            public override void Initialize() { }
            public override void Finish() { }
        }

        public class SensorProducerAgent : ComputationalAgent<int, SensorReading>
        {
            private readonly SensorReading _reading;

            public SensorProducerAgent(string name, SensorReading reading) : base(name)
            {
                _reading = reading;
            }

            public override void Consume(int consumedData) { }
            public override SensorReading Produce() => _reading;
            protected override void ExecuteComputation() { }
            public override void Initialize() { }
            public override void Finish() { }
        }

        public class SensorProducerAgent2 : ComputationalAgent<int, SensorReading>
        {
            private readonly SensorReading _reading;

            public SensorProducerAgent2(string name, SensorReading reading) : base(name)
            {
                _reading = reading;
            }

            public override void Consume(int consumedData) { }
            public override SensorReading Produce() => _reading;
            protected override void ExecuteComputation() { }
            public override void Initialize() { }
            public override void Finish() { }
        }

        [ConsumesFrom(typeof(SensorProducerAgent))]
        [ConsumesFrom(typeof(SensorProducerAgent2))]
        public class SensorAggregatorAgent : MultiSourceComputationalAgent<AggregatedData>
        {
            private List<SensorReading> _readings = new List<SensorReading>();

            public SensorAggregatorAgent(string name) : base(name) { }

            public override void ConsumeMultiple(IDictionary<System.Type, object> consumedData)
            {
                _readings.Clear();
                foreach (var data in consumedData.Values)
                {
                    if (data is SensorReading reading)
                    {
                        _readings.Add(reading);
                    }
                }
            }

            public override AggregatedData Produce()
            {
                if (_readings.Count == 0) return null;

                return new AggregatedData
                {
                    AverageTemperature = _readings.Average(r => r.Temperature),
                    AverageHumidity = _readings.Average(r => r.Humidity),
                    ReadingCount = _readings.Count
                };
            }

            protected override void ExecuteComputation() { }
            public override void Initialize() { _readings = new List<SensorReading>(); }
            public override void Finish() { }
        }

        // Unit Tests
        [Fact]
        public void Runner_WithCustomPersonModel_ShouldPassDataCorrectly()
        {
            // Arrange
            var runner = new Runner();
            var person = new Person { Name = "John Doe", Age = 30, Email = "john@example.com" };
            var producer = new PersonProducerAgent("Producer", person);
            var validator = new PersonValidatorAgent("Validator");

            runner.AddAgent(producer);
            runner.AddAgent(validator);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            Assert.True((bool)validator.ProducedData);
        }

        [Fact]
        public void ParallelRunner_WithCustomPersonModel_ShouldPassDataCorrectly()
        {
            // Arrange
            var runner = new ParallelRunner();
            var person = new Person { Name = "Jane Smith", Age = 25, Email = "jane@example.com" };
            var producer = new PersonProducerAgent("Producer", person);
            var validator = new PersonValidatorAgent("Validator");

            runner.AddAgent(producer);
            runner.AddAgent(validator);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            Assert.True((bool)validator.ProducedData);
        }

        [Fact]
        public void Runner_WithInvalidPerson_ShouldProduceFalse()
        {
            // Arrange
            var runner = new Runner();
            var person = new Person { Name = "", Age = 0, Email = "" }; // Invalid
            var producer = new PersonProducerAgent("Producer", person);
            var validator = new PersonValidatorAgent("Validator");

            runner.AddAgent(producer);
            runner.AddAgent(validator);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            Assert.False((bool)validator.ProducedData);
        }

        [Fact]
        public void Runner_WithCustomOrderModel_ShouldCalculateTax()
        {
            // Arrange
            var runner = new Runner();
            var order = new Order 
            { 
                OrderId = 123, 
                Items = new List<string> { "Item1", "Item2", "Item3" },
                Total = 100.00m
            };
            var producer = new OrderProducerAgent("Producer", order);
            var processor = new OrderProcessorAgent("Processor", 0.08m); // 8% tax

            runner.AddAgent(producer);
            runner.AddAgent(processor);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            var result = (ProcessedOrder)processor.ProducedData;
            Assert.NotNull(result);
            Assert.Equal(123, result.OrderId);
            Assert.Equal(3, result.ItemCount);
            Assert.Equal(100.00m, result.Total);
            Assert.Equal(8.00m, result.Tax);
            Assert.Equal(108.00m, result.GrandTotal);
        }

        [Fact]
        public void ParallelRunner_WithCustomOrderModel_ShouldCalculateTax()
        {
            // Arrange
            var runner = new ParallelRunner();
            var order = new Order 
            { 
                OrderId = 456, 
                Items = new List<string> { "Item1", "Item2" },
                Total = 200.00m
            };
            var producer = new OrderProducerAgent("Producer", order);
            var processor = new OrderProcessorAgent("Processor", 0.10m); // 10% tax

            runner.AddAgent(producer);
            runner.AddAgent(processor);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            var result = (ProcessedOrder)processor.ProducedData;
            Assert.NotNull(result);
            Assert.Equal(456, result.OrderId);
            Assert.Equal(2, result.ItemCount);
            Assert.Equal(200.00m, result.Total);
            Assert.Equal(20.00m, result.Tax);
            Assert.Equal(220.00m, result.GrandTotal);
        }

        [Fact]
        public void Runner_WithMultipleCustomModels_ShouldAggregateData()
        {
            // Arrange
            var runner = new Runner();
            var reading1 = new SensorReading 
            { 
                SensorId = "Sensor1", 
                Temperature = 22.5, 
                Humidity = 45.0,
                Timestamp = System.DateTime.Now
            };
            var reading2 = new SensorReading 
            { 
                SensorId = "Sensor2", 
                Temperature = 23.5, 
                Humidity = 55.0,
                Timestamp = System.DateTime.Now
            };

            var producer1 = new SensorProducerAgent("Producer1", reading1);
            var producer2 = new SensorProducerAgent2("Producer2", reading2);
            var aggregator = new SensorAggregatorAgent("Aggregator");

            runner.AddAgent(producer1);
            runner.AddAgent(producer2);
            runner.AddAgent(aggregator);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            var result = (AggregatedData)aggregator.ProducedData;
            Assert.NotNull(result);
            Assert.Equal(23.0, result.AverageTemperature); // (22.5 + 23.5) / 2
            Assert.Equal(50.0, result.AverageHumidity); // (45 + 55) / 2
            Assert.Equal(2, result.ReadingCount);
        }

        [Fact]
        public void ParallelRunner_WithMultipleCustomModels_ShouldAggregateData()
        {
            // Arrange
            var runner = new ParallelRunner();
            var reading1 = new SensorReading 
            { 
                SensorId = "Sensor1", 
                Temperature = 20.0, 
                Humidity = 40.0,
                Timestamp = System.DateTime.Now
            };
            var reading2 = new SensorReading 
            { 
                SensorId = "Sensor2", 
                Temperature = 24.0, 
                Humidity = 60.0,
                Timestamp = System.DateTime.Now
            };

            var producer1 = new SensorProducerAgent("Producer1", reading1);
            var producer2 = new SensorProducerAgent2("Producer2", reading2);
            var aggregator = new SensorAggregatorAgent("Aggregator");

            runner.AddAgent(producer1);
            runner.AddAgent(producer2);
            runner.AddAgent(aggregator);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            var result = (AggregatedData)aggregator.ProducedData;
            Assert.NotNull(result);
            Assert.Equal(22.0, result.AverageTemperature); // (20 + 24) / 2
            Assert.Equal(50.0, result.AverageHumidity); // (40 + 60) / 2
            Assert.Equal(2, result.ReadingCount);
        }

        [Fact]
        public void Runner_WithNullCustomModel_ShouldHandleGracefully()
        {
            // Arrange
            var runner = new Runner();
            var producer = new PersonProducerAgent("Producer", null);
            var validator = new PersonValidatorAgent("Validator");

            runner.AddAgent(producer);
            runner.AddAgent(validator);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            Assert.False((bool)validator.ProducedData);
        }

        [Fact]
        public void Runner_WithComplexNestedModel_ShouldPreserveStructure()
        {
            // Arrange
            var runner = new Runner();
            var order = new Order 
            { 
                OrderId = 789, 
                Items = new List<string> { "A", "B", "C", "D", "E" },
                Total = 500.00m
            };
            var producer = new OrderProducerAgent("Producer", order);
            var processor = new OrderProcessorAgent("Processor", 0.05m);

            runner.AddAgent(producer);
            runner.AddAgent(processor);

            // Act
            runner.Run(Schedule.RunOnce);

            // Assert
            var result = (ProcessedOrder)processor.ProducedData;
            Assert.NotNull(result);
            Assert.Equal(5, result.ItemCount);
            Assert.Equal(789, result.OrderId);
        }
    }
}
