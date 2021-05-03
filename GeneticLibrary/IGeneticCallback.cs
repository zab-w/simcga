using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticLibrary
{
    public interface IGeneticCallback<IndividualType, MeasureType>
    {
        void OnCreateNewPopulate(int prevPopulateCount, int nextPopulateCount);
        void OnRemoveDuplicate(int firstCount, int secondCount);
        void OnFitness();
        void OnFinish(object genetic);
        void OnError(Exception ex);
        void OnDump(IList<IndividualType> population, IDictionary<IndividualType, MeasureType> results);
		void OnMeasuring(IndividualType individual);
		void OnMeasured(IndividualType individual, TimeSpan elapsed);
        void OnStart(object genetic);
    }
}
