using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krankenschwester.Domain.Events
{
    public class EnteredZone(bool withFlasks)
    {
        public bool WithFlasks { get; } = withFlasks;
    }
}
