using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UtilsLib
{
    public static class StaticRandom
    {
        private static int _seed;
        static StaticRandom()
        {
            _seed = unchecked((int)DateTime.Now.Ticks);
        }
        
        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        public static Random Random
        {
            get
            {
                return random.Value;
            }
        }
    }
}
