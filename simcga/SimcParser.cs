using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace simcga
{
    public static class SimcParser
    {
        public static void ParseActionsAndOptions(string baseSimcPath, out List<string> actions, out List<IOption> options)
        {
            var simc = File.ReadAllLines(baseSimcPath).Where(x => !x.StartsWith("#") && !string.IsNullOrEmpty(x.Trim())).ToArray();
            var values = simc.Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
            var talents = values["talents"];
            var covenant = values["covenant"];

            actions = new[]
            {
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

            options = new List<IOption>();
            switch (talents[0])
            {
                case '1':
                    {
                        options.Add(new BuffOption("earthen_rage"));
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
                        options.Add(new BuffOption("echoing_shock"));
                    }
                    break;
                case '3':
                    break;
            }
            switch (talents[3])
            {
                case '1':
                    {
                        options.Add(new BuffOption("master_of_the_elements"));
                    }
                    break;
                case '2':
                    {
                        options.Add(new PetOption("pet.storm_elemental.active"));
                        options.AddRange(Enumerable.Range(0, 21).Select(x => new StackBuffOption("wind_gust", 1, x)).OfType<IOption>());
                        options.AddRange(Enumerable.Range(0, 21).Select(x => new StackBuffOption("wind_gust", -1, x)).OfType<IOption>());
                    }
                    break;
                case '3':
                    break;
            }
            switch (talents[5])
            {
                case '1':
                    {
                        options.Add(new BuffOption("surge_of_power"));
                    }
                    break;
                case '2':
                    break;
                case '3':
                    {
                        options.AddRange(Enumerable.Range(0, 3).Select(x => new StackBuffOption("icefury", 1, x)).OfType<IOption>());
                        options.AddRange(Enumerable.Range(1, 5).Select(x => new StackBuffOption("icefury", -1, x)).OfType<IOption>());
                    }
                    break;
            }
            switch (talents[6])
            {
                case '1':
                    break;
                case '2':
                    {
                        options.AddRange(Enumerable.Range(0,2).Select(x => new StackBuffOption("stormkeeper", 1, x)).OfType<IOption>());
                        options.AddRange(Enumerable.Range(1,4).Select(x => new StackBuffOption("stormkeeper", -1, x)).OfType<IOption>());
                    }
                    break;
                case '3':
                    {
                        options.Add(new BuffOption("ascendance"));
                    }
                    break;
            }

            options.AddRange(Enumerable.Range(1, 10).Select(x => new ResourceOption("maelstrom", 1, x * 10)).OfType<IOption>());
            options.AddRange(Enumerable.Range(1, 10).Select(x => new ResourceOption("maelstrom", -1, x * 10)).OfType<IOption>());
        }
    }
}
