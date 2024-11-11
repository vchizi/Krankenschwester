using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krankenschwester.Domain.Events
{
    class ProcessDeactivated(string message)
    {
        public string Message { get; private set; } = message;
    }
}
