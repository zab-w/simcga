using System;
using System.Collections.Generic;
using UtilsLib;

namespace simcga
{
    public class ResourceOption : BaseCondition, IEquatable<ResourceOption>
    {
        private readonly string _resourceName;
        private readonly int _moreEqualLess;
        private readonly int _value;

        public ResourceOption(string resourceName, int moreEqualLess, int value)
        {
            this._resourceName = resourceName;
            this._moreEqualLess = moreEqualLess;
            this._value = value;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ResourceOption);
        }

        public bool Equals(ResourceOption other)
        {
            return other != null &&
                   base.Equals(other) &&
                   this._resourceName == other._resourceName &&
                   this._moreEqualLess == other._moreEqualLess &&
                   this._value == other._value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), this._resourceName, this._moreEqualLess, this._value);
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

            return $"{_negate}{_resourceName}{compare}{_value}";
        }

        protected override BaseCondition MutateImpl()
        {
            var value = _value;
            int moreEqualLess = _moreEqualLess;
            int rand = StaticRandom.Random.Next(2);
            if (rand == 0) // Change stacks
            {
                rand = StaticRandom.Random.Next(2); // 0 - multiple, 1 - divide
                if (rand == 0)
                {
                    value *= 2;
                }
                else if (rand == 1)
                {
                    value /= 2;
                }
            }
            else if (rand == 1) // change operator
            {
                moreEqualLess = StaticRandom.Random.Next(3) - 1;
            }

            return new ResourceOption(_resourceName, moreEqualLess, value);
        }

        public static bool operator ==(ResourceOption left, ResourceOption right)
        {
            return EqualityComparer<ResourceOption>.Default.Equals(left, right);
        }

        public static bool operator !=(ResourceOption left, ResourceOption right)
        {
            return !(left == right);
        }
    }
}
