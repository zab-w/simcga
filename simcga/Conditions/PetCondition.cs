using System;
using System.Collections.Generic;

namespace simcga
{
    public class PetCondition : BaseCondition, IEquatable<PetCondition>
    {
        private readonly string _petName;
        private readonly string _state;

        public PetCondition(string petName, string state = "active")
        {
            this._petName = petName;
            this._state = state;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PetCondition);
        }

        public bool Equals(PetCondition other)
        {
            return other != null &&
                   base.Equals(other) &&
                   this._petName == other._petName &&
                   this._state == other._state;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), this._petName, this._state);
        }

        public override string ToString()
        {
            return $"{_negate}pet.{_petName}.{_state}";
        }

        protected override BaseCondition MutateImpl()
        {
            return new PetCondition(_petName);
        }

        public static bool operator ==(PetCondition left, PetCondition right)
        {
            return EqualityComparer<PetCondition>.Default.Equals(left, right);
        }

        public static bool operator !=(PetCondition left, PetCondition right)
        {
            return !(left == right);
        }
    }
}
