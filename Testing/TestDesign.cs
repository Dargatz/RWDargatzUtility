using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MRFParser;

namespace Testing
{
    public record TestDesign : DesignBase
    {

        public string Name;
        public int Number;

        public TestDesign(FileParser section) : base(section)
        {

        }
    }
}
