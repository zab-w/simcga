using System;
using System.Collections.Generic;
using UtilsLib;

namespace simcga
{
    public abstract class BaseCondition : ICondition, IEquatable<BaseCondition>
    {
        protected string _negate = string.Empty;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BaseCondition);
        }

        public bool Equals(BaseCondition other)
        {
            return other != null &&
                   this._negate == other._negate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this._negate);
        }

        public ICondition Mutate()
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

        protected abstract BaseCondition MutateImpl();

        public static bool operator ==(BaseCondition left, BaseCondition right)
        {
            return EqualityComparer<BaseCondition>.Default.Equals(left, right);
        }

        public static bool operator !=(BaseCondition left, BaseCondition right)
        {
            return !(left == right);
        }
    }
}
