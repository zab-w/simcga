using GeneticLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace simcga
{
    internal class GeneticCallback : IGeneticCallback<Apl, Dps>
    {
        public void OnCreateNewPopulate(int prevPopulateCount, int nextPopulateCount)
        {
            Console.WriteLine("OnCreateNewPopulate");
        }

        public void OnDump(IList<Apl> population, IDictionary<Apl, Dps> results)
        {
            Console.WriteLine($"Average dmg: {results.Select(x => x.Value.Damage).Average()}");
        }

        public void OnError(Exception ex)
        {
            Console.WriteLine("OnError");
        }

        public void OnFinish(object genetic)
        {
            Console.WriteLine("OnFinish");
        }

        public void OnFitness()
        {
            Console.WriteLine("OnFitness");
        }

        public void OnMeasuring(Apl individual)
        {
            Console.WriteLine($"OnMeasuring {individual.ToString()}");
        }

        public void OnMeasured(Apl individual, TimeSpan elapsed)
        {
            Console.WriteLine($"OnMeasured {individual.ToString()}. Elapsed: {elapsed}");
        }

        public void OnRemoveDuplicate(int firstCount, int secondCount)
        {
            Console.WriteLine($"OnRemoveDuplicate. Source {firstCount}, Left {secondCount}");
        }

        public void OnStart(object genetic)
        {
            Console.WriteLine("OnStart");
        }
    }
}