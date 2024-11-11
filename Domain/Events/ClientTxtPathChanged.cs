using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krankenschwester.Domain.Events
{
    public class ClientTxtPathChanged(string path)
    {
        public string Path { get; private set; } = path;
    }
}
