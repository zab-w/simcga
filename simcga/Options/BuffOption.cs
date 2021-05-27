using System;
using System.Collections.Generic;

namespace simcga
{
    public class BuffOption : BaseOption, IEquatable<BuffOption>
    {
        private readonly string _buffName;
        private readonly string _state;

        public BuffOption(string buffName, string state = "up")
        {
            this._buffName = buffName;
            this._state = state;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BuffOption);
        }

        public bool Equals(BuffOption other)
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

        protected override BaseOption MutateImpl()
        {
            var newState = "down";
            if(_state == "down")
            {
                newState = "up";
            }

            return new BuffOption(_buffName, newState);
        }

        public static bool operator ==(BuffOption left, BuffOption right)
        {
            return EqualityComparer<BuffOption>.Default.Equals(left, right);
        }

        public static bool operator !=(BuffOption left, BuffOption right)
        {
            return !(left == right);
        }
    }
}
