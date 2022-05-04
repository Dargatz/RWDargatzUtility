using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWDargatzUtility.MRFParser;

namespace RWDargatzUtility.GameDesign.GameObjects
{
    public abstract class GameObjectDesign
    {
        public string Ref { get; set; }
        public string[] Tags { get; set; }

        public GameObjectDesign(FileParser parser)
        {
            Ref = parser.FindRequiredString("REF");
            Tags = parser.FindRequiredStringArray("TAGS");
        }
    }

}
