using System;
using System.Collections.Generic;
using UtilsLib;

namespace simcga
{
    public abstract class BaseOption : IOption, IEquatable<BaseOption>
    {
        protected string _negate = string.Empty;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BaseOption);
        }

        public bool Equals(BaseOption other)
        {
            return other != null &&
                   this._negate == other._negate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this._negate);
        }

        public IOption Mutate()
        {
            var ret = this.MutateImpl();
            int rand = StaticRandom.Random.Next(2);
            if (rand == 1)
            {
                if (ret._negate == "!")
                {
                    ret._negate = "";
                }
                else
                {
                    ret._negate = "!";
                }
            }

            return ret;
        }

        protected abstract BaseOption MutateImpl();

        public static bool operator ==(BaseOption left, BaseOption right)
        {
            return EqualityComparer<BaseOption>.Default.Equals(left, right);
        }

        public static bool operator !=(BaseOption left, BaseOption right)
        {
            return !(left == right);
        }
    }
}
