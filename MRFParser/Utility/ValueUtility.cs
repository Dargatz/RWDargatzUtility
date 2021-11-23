using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRFParser.Utility
{
    public static class ValueUtility
    {

        public static ValueGenerationType ParseGenerationType(string raw)
        {
            if (raw.Contains("...")) return ValueGenerationType.Ranged;
            if (raw.Contains(',')) return ValueGenerationType.List;
            else return ValueGenerationType.Single;
        }

    }

    public enum ValueGenerationType
    {
        Single,
        Ranged,
        List
    }
}
