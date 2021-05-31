using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UtilsLib;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace GeneticLibrary
{
    public interface IGeneticOperations<IndividualType, MeasureType>
    {
        IList<IndividualType> CreateInitialPopulation(int count);

        IDictionary<IndividualType, MeasureType> Measure(in IList<IndividualType> items);

        IList<IndividualType> CrossOver(IList<IndividualType> parents);

        IList<IndividualType> Mutate(IndividualType item);

        bool Fitness(IList<IndividualType> population, IDictionary<IndividualType, MeasureType> results, IList<bool> toDelete);

        void History(IList<IndividualType> population, IDictionary<IndividualType, MeasureType> results);
    }

    public class GeneticMain<IndividualType, MeasureType>
    {
        readonly IList<IndividualType> initialPopulation = new List<IndividualType>();
        readonly Dictionary<IndividualType, MeasureType> results = new Dictionary<IndividualType, MeasureType>();
        readonly IEqualityComparer<IndividualType> equalityComparer;
        private readonly IGeneticOperations<IndividualType, MeasureType> geneticOperations;
        readonly IGeneticOptions options;
        readonly Random rnd = StaticRandom.Random;


        public GeneticMain(IGeneticOperations<IndividualType, MeasureType> geneticOperations
            , IEqualityComparer<IndividualType> equalityComparer
            , IGeneticOptions options)
        {
            this.geneticOperations = geneticOperations;
            this.options = options;
            this.equalityComparer = equalityComparer;
            this.initialPopulation = geneticOperations.CreateInitialPopulation(100);

            if (!typeof(IndividualType).IsValueType && equalityComparer == null)
            {
                throw new ArgumentException("Must be specified", nameof(equalityComparer));
            }

            if (equalityComparer != null)
            {
                results = new Dictionary<IndividualType, MeasureType>(equalityComparer);
            }
            else
            {
                results = new Dictionary<IndividualType, MeasureType>();
            }
        }

        private List<IndividualType> RemoveDuplicates(IList<IndividualType> populate, IGeneticCallback<IndividualType, MeasureType> callback)
        {
            var uniques = populate.Select(x => x).Distinct(equalityComparer).ToList();
            callback?.OnRemoveDuplicate(populate.Count, uniques.Count);
            return uniques;
        }

        List<IndividualType> CreateNewPopulate(IList<IndividualType> parentPopulate, IGeneticCallback<IndividualType, MeasureType> callback)
        {
            if (parentPopulate.Count == 0)
            {
                throw new ArgumentException("Initial populate must be specified", nameof(parentPopulate));
            }

            List<IndividualType> sons = new List<IndividualType>();
            foreach (IndividualType person in parentPopulate)
            {
                int k = rnd.Next(100);
                if (k <= options.MutateChance)
                {
                    sons.AddRange(this.geneticOperations.Mutate(person));
                }
            }

            List<IndividualType> parentsForCrossOver = new List<IndividualType>();
            foreach (IndividualType person in parentPopulate)
            {
                int k = rnd.Next(100);
                if (k < options.DescendantChance)
                {
                    parentsForCrossOver.Add(person);
                }
            }

            while (parentsForCrossOver.Count > 0)
            {
                List<IndividualType> temp = new List<IndividualType>();
                for (int i = 0; i < options.ParentCountForNew; ++i)
                {
                    if (parentsForCrossOver.Count == 0)
                    {
                        break;
                    }

                    temp.Add(parentsForCrossOver[0]);
                    parentsForCrossOver.Remove(parentsForCrossOver[0]);
                }

                if (temp.Count == options.ParentCountForNew)
                {
                    sons.AddRange(geneticOperations.CrossOver(temp));
                }
            }

            sons.AddRange(parentPopulate);
            callback?.OnCreateNewPopulate(parentPopulate.Count, sons.Count);

            return sons;
        }

        public IDictionary<IndividualType, MeasureType> Run(IGeneticCallback<IndividualType, MeasureType> callback)
        {
            callback?.OnStart(this);
            int populateIndex = 0;
            var populate = this.initialPopulation;
            do
            {
                populate = CreateNewPopulate(populate, callback);
                foreach (var n in geneticOperations.CreateInitialPopulation((int)(populate.Count * 0.1)))
                {
                    populate.Add(n);
                }

                populate = RemoveDuplicates(populate, callback);

                var toMeasure = populate.Where(x => !results.ContainsKey(x)).ToList();

                var measured = geneticOperations.Measure(toMeasure);
                foreach (var individual in measured)
                {
                    results.Add(individual.Key, individual.Value);
                }

                geneticOperations.History(populate, results);
                callback.OnDump(populate, results);

                List<bool> toDelete = new List<bool>();
                callback?.OnFitness();
                bool contGenerate = geneticOperations.Fitness(populate, results, toDelete);
                IList<IndividualType> toDeleteRefs = GetIndividualsToDelete(populate, toDelete);

                if (populate.Count - toDeleteRefs.Count <= options.MinPersonToStop || !contGenerate)
                {
                    break;
                }
                DeleteIndividuals(populate, toDeleteRefs);
                if (populateIndex++ > options.MaxPopulateCount)
                {
                    break;
                }
            }
            while (true);
            callback?.OnFinish(this);
            return results;
        }

        private static void DeleteIndividuals(IList<IndividualType> populate, IList<IndividualType> toDeleteRefs)
        {
            foreach (IndividualType item in toDeleteRefs)
            {
                populate.Remove(item);
            }
        }

        private static IList<IndividualType> GetIndividualsToDelete(IList<IndividualType> populate, List<bool> toDelete)
        {
            List<IndividualType> toDeleteRefs = new List<IndividualType>();
            if (toDelete.Count != 0)
            {
                for (int i = 0; i < populate.Count; ++i)
                {
                    if (toDelete[i])
                    {
                        toDeleteRefs.Add(populate[i]);
                    }
                }
            }

            return toDeleteRefs;
        }

        public IDictionary<IndividualType, MeasureType> GetAllResults()
        {
            return results;
        }

        public Dictionary<IndividualType, MeasureType> GetPopulateResults()
        {
            return results.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
