using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalAgentFramework.Agent
{
    public interface IStateMachineAgent
    {
        void Initialize();

        void Execute();

        void Finish();
    }
}
