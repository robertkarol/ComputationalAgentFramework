using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Framework.Scheduler
{
    public interface Scheduler
    {
        bool CanRun();

        void ThickEpoch();

        bool HasMoreEpochsToRun();
    }
}
