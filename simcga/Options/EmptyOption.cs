﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simcga.Options
{
    class EmptyOption : IOption
    {
        public static IOption Option = new EmptyOption();

        public IOption Mutate()
        {
            return Option;
        }
    }
}
