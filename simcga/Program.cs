using GeneticLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;

namespace simcga
{
    class Program
    {
        static void Main(string[] args)
        {
            AplEqualityComparer comparer = new AplEqualityComparer();
            string simcPath = @"D:\Workshop\simc\bin\x64\Release\simc.exe";
            string baseSimcPath = @"d:\Workshop\AutoSimC\input.txt";
            
            string apiKeyPath = @"d:\Workshop\simc\vs\";
            GeneticOperations geneticOperations = new simcga.GeneticOperations(simcPath, baseSimcPath, apiKeyPath);
            BaseGeneticOptions options = new BaseGeneticOptions();
            options.DescendantChance = 33;

            GeneticMain<Apl, Dps> geneticMain = new GeneticMain<Apl, Dps>(geneticOperations
                , comparer
                , options
                );
            IGeneticCallback<Apl, Dps> geneticCallback = new GeneticCallback();
            var results = geneticMain.Run(geneticCallback);
            File.WriteAllText("results.json", JsonConvert.SerializeObject(results));
        }

    }
}
