using GeneticLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using simcga.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsLib;

namespace simcga
{
    class GeneticOperations : IGeneticOperations<Apl, Dps>
    {
        private readonly string _baseSimcFile;
        private readonly SimcRunner _simcRunner;
        private double _maxDamage;
        readonly Random rnd = StaticRandom.Random;
        private int _populationCount;

        public GeneticOperations(string simcPath
            , string baseSimcFile
            , string apiKeyPath
            )
        {
            this._baseSimcFile = baseSimcFile;
            this._simcRunner = new SimcRunner(baseSimcFile, simcPath, apiKeyPath);
        }

        public IList<Apl> CrossOver(IList<Apl> parents)
        {
            Apl[] ret = new Apl[10];
            for (int i = 0; i < ret.Length; ++i)
            {
                ret[i] = new Apl();
                var max = parents.Select(x => x.Actions.Count).Max();
                for (int j = 0; j < max; ++j)
                {
                    var act = new AplAction();
                    var parentIndex1 = rnd.Next(parents.Count);
                    var parentIndex2 = rnd.Next(parents.Count);
                    var parent1 = parents[parentIndex1];
                    var parent2 = parents[parentIndex2];

                    act.ActionName = parent1.Actions[Math.Min(j, parent1.Actions.Count - 1)].ActionName;
                    act.Options = parent2.Actions[Math.Min(j, parent2.Actions.Count - 1)].Options;
                    ret[i].Actions.Add(act);
                }
            }

            return ret;
        }

        public bool Fitness(IList<Apl> population, IDictionary<Apl, Dps> results, IList<bool> toDelete)
        {
            for (int i = 0; i < population.Count; ++i)
            {
                if (results[population[i]].Damage < 0)
                {
                    toDelete.Add(true);
                }
                else if (results[population[i]].Damage == 0)
                {
                    // It could be a buff, like echo
                    if (population[i].Actions.Count == 1)
                    {
                        toDelete.Add(false);
                    }
                    else
                    {
                        toDelete.Add(true);
                    }
                }
                else if (results[population[i]].Damage < _maxDamage * 0.75)
                {
                    toDelete.Add(true);
                }
                else
                {
                    toDelete.Add(false);
                }
            }

            return true;
        }

        public void History(IList<Apl> population, IDictionary<Apl, Dps> results)
        {
            _maxDamage = results.Select(x => x.Value.Damage).Max();
            File.WriteAllText("results.json", JsonConvert.SerializeObject(results.OrderByDescending(x => x.Value.Damage).Take(10)));
            var maxResult = results.OrderByDescending(x => x.Value.Damage).First();
            File.WriteAllLines($"best{_populationCount++}_{(int)maxResult.Value.Damage}.simc", maxResult.Key.GetContent());
        }

        private static IEnumerable<IList<TSource>> Batch<TSource>(
                  IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count).ToArray();
        }

        public IDictionary<Apl, Dps> Measure(in IList<Apl> items)
        {
            IDictionary<Apl, Dps> ret = new Dictionary<Apl, Dps>();
            foreach (var batch in Batch(items, 10))
            {
                var batchRet = _simcRunner.MeasureCore(batch);
                foreach(var t in batchRet)
                {
                    ret.Add(t.Key, t.Value);
                }
            }

            return ret;
        }

        public Apl Mutate(Apl item)
        {
            var newItem = new Apl();
            foreach (var act in item.Actions)
            {
                if (StaticRandom.Random.Next(0, 3) != 0 || act.Options is EmptyOption)
                {
                    newItem.Actions.Add(act);
                }
                else
                {
                    newItem.Actions.Add(new AplAction { ActionName = act.ActionName, Options = act.Options.Mutate() });
                }
            }

            return newItem;
        }

        public IList<Apl> CreateInitialPopulation(int count)
        {
            SimcParser.ParseActionsAndOptions(_baseSimcFile, out List<string> actions, out List<IOption> options);
            List<Apl> initialApl = new List<Apl>();
            while (initialApl.Count < count)
            {
                var apl = new Apl();
                for (int i = 0; i < 30; ++i)
                {
                    var action = new AplAction();
                    action.ActionName = actions[rnd.Next(0, actions.Count)];
                    var optionsIndex = rnd.Next(-1, options.Count);
                    action.Options = optionsIndex == -1 ? EmptyOption.Option : options[optionsIndex];
                    apl.Actions.Add(action);
                }

                initialApl.Add(apl);
            }

            return initialApl;
        }
    }
}
