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

            var rs = CakeParser.ParseFile(new System.IO.FileInfo(file));
        }
    }
}
