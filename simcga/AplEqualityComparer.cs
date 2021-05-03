using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simcga
{
    class AplEqualityComparer : EqualityComparer<Apl>
    {
        public override bool Equals(Apl x, Apl y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(Apl obj)
        {
            return obj.GetHashCode();
        }
    }
}
