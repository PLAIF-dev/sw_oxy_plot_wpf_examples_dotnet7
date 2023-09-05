using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCtrlLib
{
    public class GraphModel
    {
        public class GraphDataSet
        {
            public int ID { get; set; } = 0;
            public string LineName { get; set; } = string.Empty;
            public List<double> DataX { get; set; } = new List<double>();
            public List<double> DataY { get; set; } = new List<double>();
        }

    }
}
