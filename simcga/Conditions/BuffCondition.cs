using System;
using System.Collections.Generic;

namespace simcga
{
    public class BuffCondition : BaseCondition, IEquatable<BuffCondition>
    {
        private readonly string _buffName;
        private readonly string _state;

        public BuffCondition(string buffName, string state = "up")
        {
            this._buffName = buffName;
            this._state = state;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BuffCondition);
        }

        public bool Equals(BuffCondition other)
        {
            return other != null &&
                   base.Equals(other) &&
                   this._buffName == other._buffName &&
                   this._state == other._state;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), this._buffName, this._state);
        }

        public override string ToString()
        {
            return $"{_negate}buff.{_buffName}.{_state}";
        }

        protected override BaseCondition MutateImpl()
        {
            var newState = "down";
            if(_state == "down")
            {
                newState = "up";
            }

            return new BuffCondition(_buffName, newState);
        }

        public static bool operator ==(BuffCondition left, BuffCondition right)
        {
            return EqualityComparer<BuffCondition>.Default.Equals(left, right);
        }

        public static bool operator !=(BuffCondition left, BuffCondition right)
        {
            return !(left == right);
        }
    }
}
