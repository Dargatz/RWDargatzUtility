using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MRFParser
{
    public abstract record DesignBase
    {

        public DesignBase(FileParser section)
        {
            Console.WriteLine("object properties");
            foreach(PropertyInfo info in this.GetType().GetProperties())
            {
                Console.WriteLine(info.Name);
            }
        }

    }
}
