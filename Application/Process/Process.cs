using Krankenschwester.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krankenschwester.Application.Process
{
    interface Process
    {
        void Start(ProcessUsageCondition process);

        void Stop();
    }
}
