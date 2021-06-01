using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace simcga
{
    class SimcRunner
    {
        private readonly string _baseSimcFile;
        private readonly string _simcPath;
        private readonly string _apiKeyPath;

        public SimcRunner(string baseSimcFile, string simcPath, string apiKeyPath)
        {
            this._baseSimcFile = baseSimcFile;
            this._simcPath = simcPath;
            this._apiKeyPath = apiKeyPath;
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

        public IDictionary<Apl, Dps> MeasureCore(in IList<Apl> itemsIn)
        {
            var items = Sanitize(itemsIn).ToList();

            var guid = Guid.NewGuid().ToString();
            var jsonPath = Path.Combine(Path.GetTempPath(), guid + ".json");
            var inputSimcPath = Path.Combine(Path.GetTempPath(), guid + ".simc");
            var baseContent = File.ReadAllText(_baseSimcFile);
            foreach (var singleItem in items)
            {
                StringBuilder sb = new StringBuilder(baseContent);
                sb.AppendLine();
                sb.Append(string.Join(Environment.NewLine, singleItem.GetContent()));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                File.AppendAllText(inputSimcPath, sb.ToString());
            }

            IDictionary<Apl, Dps> ret = new Dictionary<Apl, Dps>();
            Process proc;
            try
            {
                var psiBase = new ProcessStartInfo();
                psiBase.FileName = _simcPath;
                psiBase.WorkingDirectory = _apiKeyPath;
                psiBase.WindowStyle = ProcessWindowStyle.Hidden;
                psiBase.CreateNoWindow = true;
                psiBase.Arguments = $@"{inputSimcPath} json={jsonPath}";
                proc = Process.Start(psiBase);
                proc.PriorityClass = ProcessPriorityClass.Idle;
                proc.WaitForExit();

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
                for (int i = 0; i < items.Count; ++i)
                {
                    var apl = items[i];
                    var dps = obj["sim"]["players"][i]["collected_data"]["dps"]["mean"].Value<double>();
                    ret.Add(apl, new Dps { Damage = dps });
                }

                return ret;
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

        private IEnumerable<Apl> Sanitize(IList<Apl> itemsIn)
        {
            foreach (var singleItem in itemsIn)
            {
                var guid = Guid.NewGuid().ToString();
                var jsonPath = Path.Combine(Path.GetTempPath(), guid + ".json");
                var inputSimcPath = Path.Combine(Path.GetTempPath(), guid + ".simc");
                var baseContent = File.ReadAllText(_baseSimcFile);

                try
                {
                    StringBuilder sb = new StringBuilder(baseContent);
                    sb.AppendLine();
                    sb.Append(string.Join(Environment.NewLine, singleItem.GetContent()));
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine();
                    File.AppendAllText(inputSimcPath, sb.ToString());

                    IDictionary<Apl, Dps> ret = new Dictionary<Apl, Dps>();
                    Process proc;

                    var psiBase = new ProcessStartInfo();
                    psiBase.FileName = _simcPath;
                    psiBase.WorkingDirectory = _apiKeyPath;
                    psiBase.WindowStyle = ProcessWindowStyle.Hidden;
                    psiBase.CreateNoWindow = true;
                    psiBase.Arguments = $@"{inputSimcPath} strict_parsing=1 save_profiles=1";
                    proc = Process.Start(psiBase);
                    proc.PriorityClass = ProcessPriorityClass.Idle;
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        yield return singleItem;
                    }
                    else
                    {
                        File.Copy(inputSimcPath, Path.Combine($"malformed_{guid}.simc"));
                    }
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
        }

        internal Dps Measure(Apl apl)
        {
            return MeasureCore(new[] { apl }).First().Value;
        }
    }
}
