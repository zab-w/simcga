using GeneticLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly string _simcPath;
        private readonly string _baseSimcFile;
        private readonly string _apiKeyPath;
        private double _averageDamage;
        private int _maximumActionCount;
        private Dictionary<Apl, Dps> _results;
        private double _maxDamage;
        readonly Random rnd = StaticRandom.Random;
        private int _populationCount;

        public GeneticOperations(string simcPath
            , string baseSimcFile
            , string apiKeyPath
            )
        {
            this._simcPath = simcPath;
            this._baseSimcFile = baseSimcFile;
            this._apiKeyPath = apiKeyPath;
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

                    act.ActionName = parents[rnd.Next(parents.Count)].Actions[j].ActionName;
                    act.Options = parents[rnd.Next(parents.Count)].Actions[j].Options;
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
                // 64 is a max for sequence. If it's less than average - it's totally useless.
                else if (population[i].Actions.Count >= _maximumActionCount && results[population[i]].Damage < _maxDamage * 0.75)
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
            _maximumActionCount = results.Select(x => x.Key.Actions.Count).Max();
            _averageDamage = results.Where(x => x.Key.Actions.Count == _maximumActionCount).Select(x => x.Value.Damage).Average();
            _results = results.ToDictionary(x => x.Key, x => x.Value);
            _maxDamage = results.Where(x => x.Key.ToString() != string.Empty).Select(x => x.Value.Damage).Max();
            File.WriteAllText("results.json", JsonConvert.SerializeObject(_results.OrderByDescending(x => x.Value.Damage)));
            var maxResult = results.OrderByDescending(x => x.Value.Damage).First();
            File.WriteAllLines($"best{_populationCount++}.simc", maxResult.Key.GetContent());
        }

        public Dps Measure(in Apl singleItem)
        {
            var guid = Guid.NewGuid().ToString();
            var jsonPath = Path.Combine(Path.GetTempPath(), guid + ".json");
            var inputSimcPath = Path.Combine(Path.GetTempPath(), guid + ".simc");
            Process proc;
            try
            {
                File.Copy(_baseSimcFile, inputSimcPath);
                File.AppendAllText(inputSimcPath, Environment.NewLine);
                File.AppendAllLines(inputSimcPath, singleItem.GetContent());
                var psiBase = new ProcessStartInfo();
                psiBase.FileName = _simcPath;
                psiBase.WorkingDirectory = _apiKeyPath;
                psiBase.WindowStyle = ProcessWindowStyle.Hidden;
                psiBase.CreateNoWindow = true;
                psiBase.Arguments = $@"{inputSimcPath} json={jsonPath}";
                proc = Process.Start(psiBase);
                proc.PriorityClass = ProcessPriorityClass.Idle;
                if (!proc.WaitForExit(10000))
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch(InvalidOperationException)
                    {
                        if(!proc.HasExited)
                        {
                            throw;
                        }
                    }
                    catch (Win32Exception ex)
                    {
                        if (ex.NativeErrorCode != 5)
                        {
                            throw;
                        }
                    }
                }

                double dps = 0.0;
                if (!File.Exists(jsonPath))
                {
                    dps = -1;
                }
                else
                {
                    var content = "";
                    bool read = false;
                    for (int i = 0; i < 5; ++i)
                    {
                        try
                        {
                            content = File.ReadAllText(jsonPath);
                            read = true;
                        }
                        catch (IOException)
                        {
                            Thread.Yield();
                            continue;
                        }
                    }
                    if (!read)
                    {
                        throw new InvalidOperationException($"Unable to read file {jsonPath}");
                    }
                    var obj = JsonConvert.DeserializeObject(content) as JObject;
                    var raid_dps = obj["sim"]["statistics"]["raid_dps"];
                    if (raid_dps != null)
                    {
                        dps = raid_dps["mean"].Value<double>();
                    }
                }
                return new Dps
                {
                    Damage = dps
                };
            }
            catch
            {
                return new Dps
                {
                    Damage = -1
                };
            }
            finally
            {
                if (File.Exists(jsonPath))
                {
                    File.Delete(jsonPath);
                }
                if (File.Exists(inputSimcPath))
                {
                    File.Delete(inputSimcPath);
                }
            }
        }

        public Apl Mutate(Apl item)
        {
            var newItem = new Apl();
            foreach (var act in item.Actions)
            {
                if (StaticRandom.Random.Next(0, 3) != 0 || string.IsNullOrEmpty(act.Options))
                {
                    newItem.Actions.Add(act);
                }
                else
                {
                    newItem.Actions.Add(new AplAction { ActionName = act.ActionName, Options = "!" + act.Options });
                }
            }

            return newItem;
        }
    }
}
