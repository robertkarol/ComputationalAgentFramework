using ComputationalAgentFramework.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Framework.Scheduler
{
    public class SchedulerFactory
    {
        public SchedulerFactory()
        {
        }

        public Scheduler Create(Schedule schedule)
        {
            switch (schedule)
            {
                case Schedule.RunOnce:
                    return new RunOnceScheduler();
                case Schedule.RunIndefinitely:
                    return new RunIndefinitelyScheduler();
                case Schedule.RunUntilStreamComplete:
                    return new RunUntilStreamCompleteScheduler();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
