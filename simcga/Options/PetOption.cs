using System;
using System.Collections.Generic;

namespace simcga
{
    public class PetOption : BaseOption, IEquatable<PetOption>
    {
        private readonly string _petName;
        private readonly string _state;

        public PetOption(string petName, string state = "active")
        {
            this._petName = petName;
            this._state = state;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PetOption);
        }

        public bool Equals(PetOption other)
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

        protected override BaseOption MutateImpl()
        {
            return new PetOption(_petName);
        }

        public static bool operator ==(PetOption left, PetOption right)
        {
            return EqualityComparer<PetOption>.Default.Equals(left, right);
        }

        public static bool operator !=(PetOption left, PetOption right)
        {
            return !(left == right);
        }
    }
}
