using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWDargatzUtility.MRFParser.Utility;

namespace RWDargatzUtility.MRFParser.ValueTypes
{
    public struct MRFInteger
    {
        private IVariableInteger integer;
        public MRFInteger(string raw)
        {
            switch (ValueUtility.ParseGenerationType(raw))
            {
                case ValueGenerationType.Single:
                    integer = new SingleInteger(raw);
                    break;
                case ValueGenerationType.Ranged:
                    integer = new RangedInteger(raw);
                    break;
                case ValueGenerationType.List:
                    integer = new RangedInteger(raw);
                    break;
                default:
                    integer = new SingleInteger(raw);
                    break;
            }
        }

        public int Dispense(Random generator) => integer.Dispense(generator);

        private interface IVariableInteger
        {
            int Dispense(Random generator);
            float CalculateAverage();
        }

        private struct SingleInteger : IVariableInteger
        {
            private int Value;
            public SingleInteger(string raw)
            {
                Value = Int32.Parse(raw);
            }
            public float CalculateAverage() => Value;
            public int Dispense(Random generator) => Value;
            public override string ToString() => $"{Value}";
        }

        private struct RangedInteger : IVariableInteger
        {
            private int RangeStart;
            private int RangeEnd;
            private int Length;
            public RangedInteger(string raw)
            {
                int refIndex = raw.IndexOf('.');
                RangeStart = Int32.Parse(raw.Substring(0, refIndex));
                RangeEnd = Int32.Parse(raw.Substring(refIndex + 3));
                Length = RangeEnd - RangeStart;
            }
            public float CalculateAverage() => (((float)Length) / 2) + RangeStart;
            public int Dispense(Random generator) => generator.Next(Length + 1) + RangeStart;
            public override string ToString() => $"{RangeStart}...{RangeEnd}";
        }
    }
}
