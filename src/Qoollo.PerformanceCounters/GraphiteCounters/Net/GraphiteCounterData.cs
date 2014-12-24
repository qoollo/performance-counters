using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.GraphiteCounters.Net
{
    internal struct GraphiteCounterData
    {
        private string _fullName;
        private double _value;

        public GraphiteCounterData(string fullName, double value)
        {
            _fullName = fullName;
            _value = value;
        }

        public string FullName { get { return _fullName; } }
        public double Value { get { return _value; } }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", _fullName, _value);
        }
    }
}
