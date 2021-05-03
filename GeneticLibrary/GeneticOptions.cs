﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GeneticLibrary
{
    
    public class BaseGeneticOptions : IGeneticOptions
    {
        public int DescendantChance { get; set; } = 0;
        
        public int MutateChance { get; set; } = 10;

        public int ParentCountForNew { get; set; } = 2;

        public int MaxPopulateCount { get; set; } = 100;

        public int MinPersonToStop { get; set; } = 3;

        public int UniqueParentCount { get; set; } = 2;

        public int MinPopulateCount { get; set; } = 10;
    }
}