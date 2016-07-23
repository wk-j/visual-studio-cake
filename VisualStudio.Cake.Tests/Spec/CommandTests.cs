using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualStudio.Cake.Helpers;
using Xunit;

namespace VisualStudio.Cake.Tests.Spec
{
    public class CommandTests
    {
        public void ShouldExecuteCmd()
        {

        }

        [Fact]
        public void ShouldExecutePs()
        {
            var path = @"Z:\Source\project\extension\VisualStudio.Cake";
            CakeHelper.ExecutePs("hello", path);
        }
    }
}
