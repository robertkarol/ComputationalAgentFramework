using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Utils
{
    public class ConsumesFrom: Attribute
    {
        public Type Producer { get; }

        public ConsumesFrom(Type producer)
        {
            Producer = producer;
        }
    }
}
