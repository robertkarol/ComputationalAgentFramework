using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Framework.Scheduler
{
    public class RunIndefinitelyScheduler : Scheduler
    {
        public bool CanRun() => true;

        public bool HasMoreEpochsToRun() => true;

        public void ThickEpoch() { }
    }
}
