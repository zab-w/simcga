using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticLibrary
{
    public interface IGeneticOptions
    {
        int DescendantChance { get; }
        int MutateChance { get; }
        int ParentCountForNew { get; }
        int MaxPopulateCount { get; }
        int MinPopulateCount { get; }
        int MinPersonToStop { get; }
    }
}
