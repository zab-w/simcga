using simcga.Actions;
using simcga.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace simcga
{
    public class AplAction : IEquatable<AplAction>
    {
        public IAction ActionName { get; set; }

        public ICondition Options { get; set; }

        public AplAction Clone()
        {
            return new AplAction
            {
                ActionName = this.ActionName,
                Options = this.Options
            };
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ActionName, Options);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AplAction)obj);
        }

        public bool Equals(AplAction other)
        {
            return ActionName == other.ActionName && Options == other.Options;
        }

        public override string ToString()
        {
            return $"{ActionName}:{Options}";
        }
    }

    public class Apl
    {
        public Apl()
        {
            Actions = new List<AplAction>();
        }

        public Apl(IEnumerable<AplAction> x)
            : this()
        {
            Actions.AddRange(x);
            List<AplAction> results = new List<AplAction>();
            foreach (var element in Actions)
            {
                if (results.Count == 0 || !results.Last().Equals(element))
                {
                    results.Add(element);
                }
            }

            Actions = results;
        }

        public List<AplAction> Actions { get; }

        public Apl Clone()
        {
            var apl = new Apl();
            apl.Actions.AddRange(Actions.Select(x => x.Clone()));
            return apl;
        }
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var act in this.Actions)
            {
                hash.Add(act.GetHashCode());
            }

            return hash.ToHashCode();
        }

        internal IEnumerable<string> GetContent()
        {
            var precombat = new string[]
            {
                "actions.precombat=\"flask\"",
"actions.precombat+=\"/food\"",
"actions.precombat+=\"/augmentation\"",
"actions.precombat+=\"/earth_elemental,if=!talent.primal_elementalist.enabled\"",
"actions.precombat+=\"/stormkeeper,if=talent.stormkeeper.enabled&(raid_event.adds.count<3|raid_event.adds.in>50)\"",
"actions.precombat+=\"/fire_elemental\"",
"actions.precombat+=\"/elemental_blast,if=talent.elemental_blast.enabled&spell_targets.chain_lightning<4\"",
"actions.precombat+=\"/lava_burst,if=!talent.elemental_blast.enabled&spell_targets.chain_lightning<4|talent.master_of_the_elements.enabled\"",
"actions.precombat+=\"/chain_lightning,if=!talent.elemental_blast.enabled&spell_targets.chain_lightning>=4\"",
"actions.precombat+=\"/snapshot_stats\"",
"actions.precombat+=\"/potion\"",
            };

            foreach(var pre in precombat)
            {
                yield return pre;
            }

            yield return "actions=/potion";
            yield return "actions+=/use_items";
            yield return "actions+=/blood_fury";
            yield return "actions+=/berserking";
            yield return "actions+=/fireblood";
            yield return "actions+=/ancestral_call";
            yield return "actions+=/bag_of_tricks";

            foreach (var act in Actions)
            {
                var temp = !(act.Options is EmptyCondition) ? string.Join(",", act.ActionName, $"if={act.Options}") : act.ActionName.ToString();
                yield return "actions+=/" + temp;
            }
        }

        public override string ToString()
        {
            return string.Join(";", Actions.Select(x => x.ActionName));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Apl)obj);
        }

        protected bool Equals(Apl other)
        {
            return this.Actions.SequenceEqual(other.Actions);
        }
    }
}
