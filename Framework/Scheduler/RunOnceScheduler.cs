using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Framework.Scheduler
{
    public class RunOnceScheduler : Scheduler
    {
        private bool _done;

        public RunOnceScheduler()
        {
            _done = false;
        }

        public bool CanRun() => true;

        public bool HasMoreEpochsToRun() => !_done;

        public void ThickEpoch() => _done = true;
    }
}
