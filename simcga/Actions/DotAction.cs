using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simcga.Actions
{
    class DotAction : IAction, IEquatable<DotAction>
    {
        private readonly string _actionName;

        public DotAction(string actionName)
        {
            this._actionName = actionName;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DotAction);
        }

        public bool Equals(DotAction other)
        {
            return other != null &&
                   this._actionName == other._actionName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this._actionName);
        }

        public override string ToString()
        {
            return $"{_actionName},target_if=refreshable";
        }

        public static bool operator ==(DotAction left, DotAction right)
        {
            return EqualityComparer<DotAction>.Default.Equals(left, right);
        }

        public static bool operator !=(DotAction left, DotAction right)
        {
            return !(left == right);
        }
    }
}
