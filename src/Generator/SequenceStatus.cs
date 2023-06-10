using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public enum SequenceStatus
    {
        Valid,
        Homopolymer,
        GCContent,
        Folding,
        Repeats
    }
}
