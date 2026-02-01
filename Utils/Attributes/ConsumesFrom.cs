using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Utils
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConsumesFrom : Attribute
    {
        public Type Producer { get; private set; }
        public string ProducerName { get; private set; }

        public ConsumesFrom(Type producer)
        {
            Producer = producer;
            ProducerName = null;
        }

        public ConsumesFrom(Type producer, string producerName)
        {
            Producer = producer;
            ProducerName = producerName;
        }
    }
}
