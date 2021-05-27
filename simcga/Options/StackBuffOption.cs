using System;
using System.Collections.Generic;
using UtilsLib;

namespace simcga
{
    public class StackBuffOption : BaseOption, IEquatable<StackBuffOption>
    {
        private readonly string _buffName;
        private readonly int _moreEqualLess;
        private readonly int _stacks;

        public StackBuffOption(string buffName, int moreEqualLess, int stacks)
        {
            this._buffName = buffName;
            this._moreEqualLess = moreEqualLess;
            this._stacks = stacks;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as StackBuffOption);
        }

        public bool Equals(StackBuffOption other)
        {
            return other != null &&
                   base.Equals(other) &&
                   this._buffName == other._buffName &&
                   this._moreEqualLess == other._moreEqualLess &&
                   this._stacks == other._stacks;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), this._buffName, this._moreEqualLess, this._stacks);
        }

        public override string ToString()
        {
            string compare = "=";
            if (_moreEqualLess > 0)
            {
                compare = ">";
            }
            else if (_moreEqualLess < 0)
            {
                compare = "<";
            }

            return $"{_negate}buff.{_buffName}.stack{compare}{_stacks}";
        }

        protected override BaseOption MutateImpl()
        {
            var stacks = _stacks;
            int moreEqualLess = _moreEqualLess;
            int rand = StaticRandom.Random.Next(2);
            if (rand == 0) // Change stacks
            {
                rand = StaticRandom.Random.Next(2); // 0 - multiple, 1 - divide
                if (rand == 0)
                {
                    stacks *= 2;
                }
                else if(rand == 1)
                {
                    stacks /= 2;
                }
            }
            else if(rand == 1) // change operator
            {
                moreEqualLess = StaticRandom.Random.Next(3) - 1;
            }

            return new StackBuffOption(_buffName, moreEqualLess, stacks);
        }

        public static bool operator ==(StackBuffOption left, StackBuffOption right)
        {
            return EqualityComparer<StackBuffOption>.Default.Equals(left, right);
        }

        public static bool operator !=(StackBuffOption left, StackBuffOption right)
        {
            return !(left == right);
        }
    }
}
