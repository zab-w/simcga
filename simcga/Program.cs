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

            ParseActionsAndOptions(baseSimcPath, out List<string> actions, out List<string> conditions);

            string apiKeyPath = @"d:\Workshop\simc\vs\";
            GeneticOperations geneticOperations = new simcga.GeneticOperations(simcPath, baseSimcPath, apiKeyPath);
            BaseGeneticOptions options = new BaseGeneticOptions();
            options.DescendantChance = 33;
            List<Apl> initialApl = new List<Apl>();
            var rnd = StaticRandom.Random;
            while (initialApl.Count < 300)
            {
                var apl = new Apl();
                for (int i = 0; i < 30; ++i)
                {
                    var action = new AplAction();
                    action.ActionName = actions[rnd.Next(0, actions.Count)];
                    var optionsIndex = rnd.Next(-1, conditions.Count);
                    action.Options = optionsIndex == -1 ? "" : conditions[optionsIndex];
                    apl.Actions.Add(action);
                }

                initialApl.Add(apl);
            }

            GeneticMain<Apl, Dps> geneticMain = new GeneticMain<Apl, Dps>(geneticOperations
                , comparer
                , initialApl
                , options
                );
            IGeneticCallback<Apl, Dps> geneticCallback = new GeneticCallback();
            var results = geneticMain.Run(geneticCallback);
            File.WriteAllText("results.json", JsonConvert.SerializeObject(results));
        }

        private static void ParseActionsAndOptions(string baseSimcPath, out List<string> actions, out List<string> conditions)
        {
            var simc = File.ReadAllLines(baseSimcPath).Where(x => !x.StartsWith("#") && !string.IsNullOrEmpty(x.Trim())).ToArray();
            var values = simc.Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
            var talents = values["talents"];
            var covenant = values["covenant"];

            actions = new[]
            {
                "bloodlust",
                "flame_shock",
                "frost_shock",
                "lightning_bolt",
                "chain_lightning",
                covenant == "necrolord" ? "primordial_wave" : "",
                covenant == "nightfae" ? "fae_transfusion" : "",
                covenant == "venthyr" ? "chain_harvest" : "",
                covenant == "kyrian" ? "vesper_totem" : "",
                "earth_elemental",
                "earth_shock",
                "earthquake",
                "fire_elemental",
                "lava_beam",
                "lava_burst",
            }.Where(x => !string.IsNullOrEmpty(x)).ToList();

            string[] activeTalents =
            {
                talents[1] == '2' ? "echoing_shock" : "",
                talents[1] == '3' ? "elemental_blast" : "",

                talents[3] == '2' ? "storm_elemental" : "",
                talents[3] == '3' ? "liquid_magma_totem" : "",

                talents[5] == '3' ? "icefury" : "",

                talents[6] == '2' ? "stormkeeper" : "",
                talents[6] == '3' ? "ascendance" : ""
            };

            actions.AddRange(activeTalents.Where(x => !string.IsNullOrEmpty(x)).ToList());

            conditions = new List<string>();
            switch (talents[0])
            {
                case '1':
                    {
                        conditions.Add("buff.earthen_rage.up");
                    }
                    break;
                case '2':
                    break;
                case '3':
                    break;
            }
            switch (talents[1])
            {
                case '1':
                    break;
                case '2':
                    {
                        conditions.Add("buff.echoing_shock.up");
                    }
                    break;
                case '3':
                    break;
            }
            switch (talents[3])
            {
                case '1':
                    {
                        conditions.Add("buff.master_of_the_elements.up");
                    }
                    break;
                case '2':
                    {
                        conditions.Add("pet.storm_elemental.active");
                        conditions.AddRange(Enumerable.Range(0, 21).Select(x => "buff.wind_gust.stack>" + x));
                        conditions.AddRange(Enumerable.Range(0, 21).Select(x => "buff.wind_gust.stack<" + x));
                    }
                    break;
                case '3':
                    break;
            }
            switch (talents[5])
            {
                case '1':
                    {
                        conditions.Add("buff.surge_of_power.up");
                    }
                    break;
                case '2':
                    break;
                case '3':
                    {
                        conditions.AddRange(Enumerable.Range(0, 4).Select(x => "buff.icefury.stack>" + x));
                        conditions.AddRange(Enumerable.Range(0, 4).Select(x => "buff.icefury.stack<" + x));
                    }
                    break;
            }
            switch (talents[6])
            {
                case '1':
                    break;
                case '2':
                    {
                        conditions.AddRange(Enumerable.Range(0, 2).Select(x => "buff.stormkeeper.stack>" + x));
                        conditions.AddRange(Enumerable.Range(0, 2).Select(x => "buff.stormkeeper.stack<" + x));
                    }
                    break;
                case '3':
                    {
                        conditions.Add("buff.ascendance.up");
                    }
                    break;
            }

            conditions.AddRange(Enumerable.Range(1, 10).Select(x => "maelstrom<" + x + "0"));
            conditions.AddRange(Enumerable.Range(1, 10).Select(x => "maelstrom>=" + x + "0"));
        }
    }
}
