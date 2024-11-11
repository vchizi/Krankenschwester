using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krankenschwester.Domain.Events
{
    public class ProcessStateChanged(bool activated)
    {
        public bool Activated { get; private set; } = activated;
    }
}
