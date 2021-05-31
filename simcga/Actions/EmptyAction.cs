using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simcga.Actions
{
    class EmptyAction : IAction
    {
        public static IAction Action = new EmptyAction();

        public override string ToString()
        {
            return "";
        }
    }
}
