using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simcga.Actions
{
    class SimpleAction : IAction, IEquatable<SimpleAction>
    {
        private readonly string _actionName;

        public SimpleAction(string actionName)
        {
            this._actionName = actionName;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SimpleAction);
        }

        public bool Equals(SimpleAction other)
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
            return $"{_actionName}";
        }

        public static bool operator ==(SimpleAction left, SimpleAction right)
        {
            return EqualityComparer<SimpleAction>.Default.Equals(left, right);
        }

        public static bool operator !=(SimpleAction left, SimpleAction right)
        {
            return !(left == right);
        }
    }
}
