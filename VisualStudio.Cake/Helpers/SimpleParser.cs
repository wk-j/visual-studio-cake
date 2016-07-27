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
        public static IEnumerable<Task> ParseFile(FileInfo file)
        {
            if (!file.Exists) return Enumerable.Empty<Task>();

            var lines = File.ReadAllLines(file.FullName);
            var tasks = lines.Select(x => x.Trim()).Where(x => x.StartsWith("Task(\""));

            return tasks.Select(x => {
                var name = x.Split('\"')[1];
                return new Task { Name = name };
            });
        }
    }
}
