using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudio.Cake.Helpers
{
    public class SimpleParser
    {
        private static IEnumerable<Task> Parse(FileInfo file)
        {
            if (!file.Exists) yield break;

            var lines = File.ReadAllLines(file.FullName);
            var tasks = lines.Select(x => x.Trim()).Where(x => x.StartsWith("Task(\""));

            var loads = lines.Select(x => x.Trim())
                .Where(x => x.StartsWith("#l"))
                .Select(x =>
                {
                    return x.Replace("#load", String.Empty)
                     .Replace("#l", String.Empty)
                     .Replace("\"", string.Empty)
                     .Trim();
                }).ToList();


            foreach (var newTask in tasks)
            {
                var name = newTask.Split('\"')[1];
                yield return new Task { Name = name };
            }


            foreach (var load in loads)
            {
                var dir = file.Directory.FullName;
                var loadPath = Path.Combine(dir, load);
                var info = new FileInfo(loadPath);
                foreach (var t in Parse(info))
                {
                    yield return t;
                }
            }
        }

        public static IEnumerable<Task> ParseFile(FileInfo file)
        {
            return Parse(file);
        }
    }
}
