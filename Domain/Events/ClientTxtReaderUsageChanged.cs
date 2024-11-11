using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krankenschwester.Domain.Events
{
    public class ClientTxtReaderUsageChanged(bool enabled)
    {
        public bool Enabled { get; } = enabled;
    }
}
