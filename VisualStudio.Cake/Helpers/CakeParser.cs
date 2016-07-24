using Sprache;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualStudio.Cake.Helpers
{
    public class Task
    {
        public string Name { set; get; }
    }

    public class CakeParser
    {
        static readonly Parser<string> TaskName =
            (from _ in Parse.Char('(')
             from open in Parse.Char('"')
             from content in Parse.CharExcept('"').Many().Text()
             from close in Parse.Char('"')
             from __ in Parse.Char(')')
             select content).Token();

        static readonly Parser<string> Any =
            (from x in Parse.Letter.AtLeastOnce()
             select x).Text();

        static readonly Parser<Task> Task =
            from _ in Any
            from taskName in TaskName
            select new Task { Name = taskName };


        public static IEnumerable<Task> ParseFile(FileInfo file)
        {
            if (!file.Exists) return Enumerable.Empty<Task>();
            var content = File.ReadAllText(file.FullName);
            var lines = content.Split('\n').Select(x => x.Trim()).Where(x => x.StartsWith("Task(\""));
            var task = lines.Select(Task.Parse);
            return task;
        }
    }
}
