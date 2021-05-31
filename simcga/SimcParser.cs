using simcga.Actions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace simcga
{
    public static class SimcParser
    {
        public static void ParseActionsAndOptions(string baseSimcPath, out List<IAction> actions, out List<ICondition> conditions)
        {
            var simc = File.ReadAllLines(baseSimcPath).Where(x => !x.StartsWith("#") && !string.IsNullOrEmpty(x.Trim())).ToArray();
            var values = simc.Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
            var talents = values["talents"];
            var covenant = values["covenant"];

            actions = new[]
            {
                new DotAction("flame_shock"),
                new SimpleAction("frost_shock"),
                new SimpleAction("lightning_bolt"),
                new SimpleAction("chain_lightning"),
                covenant == "necrolord" ? new SimpleAction("primordial_wave") : EmptyAction.Action,
                covenant == "nightfae" ? new SimpleAction("fae_transfusion") : EmptyAction.Action,
                covenant == "venthyr" ? new SimpleAction("chain_harvest") : EmptyAction.Action,
                covenant == "kyrian" ? new SimpleAction("vesper_totem") : EmptyAction.Action,
                new SimpleAction("earth_elemental"),
                new SimpleAction("earth_shock"),
                new SimpleAction("earthquake"),
                new SimpleAction("fire_elemental"),
                new SimpleAction("lava_burst"),
            }.Where(x => !(x is EmptyAction) ).ToList();

            IAction[] activeTalents =
            {
                talents[1] == '2' ? new SimpleAction("echoing_shock") : EmptyAction.Action,
                talents[1] == '3' ? new SimpleAction("elemental_blast") : EmptyAction.Action,

                talents[3] == '2' ? new SimpleAction("storm_elemental") : EmptyAction.Action,
                talents[3] == '3' ? new SimpleAction("liquid_magma_totem") : EmptyAction.Action,

                talents[5] == '3' ? new SimpleAction("icefury") : EmptyAction.Action,

                talents[6] == '2' ? new SimpleAction("stormkeeper") : EmptyAction.Action,
                talents[6] == '3' ? new SimpleAction("ascendance") : EmptyAction.Action
            };

            actions.AddRange(activeTalents.Where(x => !(x is EmptyAction)).ToList());

            conditions = new List<ICondition>();
            switch (talents[0])
            {
                case '1':
                    {
                        conditions.Add(new BuffCondition("earthen_rage"));
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
                        conditions.Add(new BuffCondition("echoing_shock"));
                    }
                    break;
                case '3':
                    break;
            }
            switch (talents[3])
            {
                case '1':
                    {
                        conditions.Add(new BuffCondition("master_of_the_elements"));
                    }
                    break;
                case '2':
                    {
                        conditions.Add(new PetCondition("pet.storm_elemental.active"));
                        conditions.AddRange(Enumerable.Range(0, 21).Select(x => new StackBuffCondition("wind_gust", 1, x)).OfType<ICondition>());
                        conditions.AddRange(Enumerable.Range(0, 21).Select(x => new StackBuffCondition("wind_gust", -1, x)).OfType<ICondition>());
                    }
                    break;
                case '3':
                    break;
            }
            switch (talents[5])
            {
                case '1':
                    {
                        conditions.Add(new BuffCondition("surge_of_power"));
                    }
                    break;
                case '2':
                    break;
                case '3':
                    {
                        conditions.AddRange(Enumerable.Range(0, 3).Select(x => new StackBuffCondition("icefury", 1, x)).OfType<ICondition>());
                        conditions.AddRange(Enumerable.Range(1, 5).Select(x => new StackBuffCondition("icefury", -1, x)).OfType<ICondition>());
                    }
                    break;
            }
            switch (talents[6])
            {
                case '1':
                    break;
                case '2':
                    {
                        conditions.AddRange(Enumerable.Range(0,2).Select(x => new StackBuffCondition("stormkeeper", 1, x)).OfType<ICondition>());
                        conditions.AddRange(Enumerable.Range(1,4).Select(x => new StackBuffCondition("stormkeeper", -1, x)).OfType<ICondition>());
                    }
                    break;
                case '3':
                    {
                        conditions.Add(new BuffCondition("ascendance"));
                    }
                    break;
            }

            conditions.AddRange(Enumerable.Range(1, 10).Select(x => new ResourceOption("maelstrom", 1, x * 10)).OfType<ICondition>());
            conditions.AddRange(Enumerable.Range(1, 10).Select(x => new ResourceOption("maelstrom", -1, x * 10)).OfType<ICondition>());
        }
    }
}
