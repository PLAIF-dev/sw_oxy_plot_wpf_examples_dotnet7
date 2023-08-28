using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCtrlLib.Message
{
    public class SharedMessge
    {
        public double DataX { get; set; }
        public double DataY { get; set; }
        public double sDataX { get; set; }
        public double sDataY { get; set; }
        public int DataIndex { get; set; }
        public object? e { get; set; }
    }
}
