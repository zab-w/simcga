using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simcga.Options
{
    class EmptyCondition : ICondition
    {
        public static ICondition Condition = new EmptyCondition();

        public ICondition Mutate()
        {
            return Condition;
        }
    }
}
