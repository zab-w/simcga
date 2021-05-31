using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GeneticLibrary
{
    
    public class BaseGeneticOptions : IGeneticOptions
    {
        public int DescendantChance { get; set; } = 0;
        
        public int MutateChance { get; set; } = 33;

        public int ParentCountForNew { get; set; } = 10;

        public int MaxPopulateCount { get; set; } = 6;

        public int MinPersonToStop { get; set; } = 3;

        public int MinPopulateCount { get; set; } = 20;
    }
}
