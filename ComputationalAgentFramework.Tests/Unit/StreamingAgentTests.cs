using ComputationalAgentFramework.Tests.TestAgents;

namespace ComputationalAgentFramework.Tests.Unit
{
    public class StreamingAgentTests
    {
        [Fact]
        public void StreamProducer_ShouldGenerateStream()
        {
            var producer = new TestStreamProducer("StreamProducer", 3);
            
            producer.Initialize();
            Assert.True(producer.HasMoreData);
            
            // First item
            producer.Execute();
            Assert.Equal(1, producer.ProducedData);
            Assert.True(producer.HasMoreData);
            
            // Second item
            producer.Execute();
            Assert.Equal(2, producer.ProducedData);
            Assert.True(producer.HasMoreData);
            
            // Third item
            producer.Execute();
            Assert.Equal(3, producer.ProducedData);
            Assert.True(producer.HasMoreData);
            
            // Stream complete
            producer.Execute();
            Assert.False(producer.HasMoreData);
        }

        [Fact]
        public void StreamProducer_ShouldCallLifecycleMethods()
        {
            var producer = new TestStreamProducer("StreamProducer", 2);
            
            producer.Initialize();
            producer.Execute(); // Item 1
            producer.Execute(); // Item 2
            producer.Execute(); // Complete (no ExecuteComputation call)
            producer.Finish();
            
            Assert.True(producer.InitializeCalled);
            Assert.True(producer.FinishCalled);
            Assert.Equal(2, producer.ExecuteCount); // ExecuteComputation called for each item, not for completion
        }

        [Fact]
        public void StreamConsumer_ShouldConsumeStreamItems()
        {
            var consumer = new TestStreamConsumer("StreamConsumer");
            
            consumer.Initialize();
            
            consumer.Consume(1);
            consumer.Execute();
            Assert.Equal(1, consumer.Sum);
            
            consumer.Consume(2);
            consumer.Execute();
            Assert.Equal(3, consumer.Sum);
            
            consumer.Consume(3);
            consumer.Execute();
            Assert.Equal(6, consumer.Sum);
            
            Assert.Equal(3, consumer.ItemCount);
        }

        [Fact]
        public void StreamConsumer_ShouldCallLifecycleMethods()
        {
            var consumer = new TestStreamConsumer("StreamConsumer");
            
            consumer.Initialize();
            consumer.Consume(5);
            consumer.Execute();
            consumer.Finish();
            
            Assert.True(consumer.InitializeCalled);
            Assert.True(consumer.FinishCalled);
            Assert.Equal(1, consumer.ExecuteCount);
        }

        [Fact]
        public void StreamConsumer_WithEmptyQueue_ShouldNotCrash()
        {
            var consumer = new TestStreamConsumer("StreamConsumer");
            
            consumer.Initialize();
            
            var exception = Record.Exception(() => consumer.Execute());
            
            Assert.Null(exception);
        }

        [Fact]
        public void StreamProducer_Initialize_ShouldResetStream()
        {
            var producer = new TestStreamProducer("StreamProducer", 2);
            
            producer.Initialize();
            producer.Execute();
            producer.Execute();
            producer.Execute(); // Complete
            
            Assert.False(producer.HasMoreData);
            
            // Re-initialize should reset
            producer.Initialize();
            Assert.True(producer.HasMoreData);
        }

        [Fact]
        public void StreamConsumer_ProducerComplete_ShouldSignalCompletion()
        {
            var consumer = new TestStreamConsumer("StreamConsumer");
            
            consumer.Initialize();
            consumer.Consume(1);
            consumer.Execute();
            
            Assert.True(consumer.HasMoreData);
            
            consumer.NotifyProducerComplete();
            consumer.Execute(); // Empty queue + producer complete
            
            Assert.False(consumer.HasMoreData);
        }
    }
}
