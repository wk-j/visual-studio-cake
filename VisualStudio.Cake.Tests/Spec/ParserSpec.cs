using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualStudio.Cake.Helpers;
using Xunit;

namespace VisualStudio.Cake.Tests.Spec
{
    public class ParserSpec
    {
        [Fact]
        public void ShouldParseFile()
        {
            var file = @"Z:\Source\project\extension\VisualStudio.Cake\build.cake";
            var rs = SimpleParser.ParseFile(new System.IO.FileInfo(file));
        }


        [Fact]
        public void ShouldParseNestedFile()
        {
            var file = @"Z:\source\project\extension\VisualStudio.Cake\VisualStudio.Cake.Tests\Tasks\build.cake";
            var rs = SimpleParser.ParseFile(new System.IO.FileInfo(file));
        }
    }
}
