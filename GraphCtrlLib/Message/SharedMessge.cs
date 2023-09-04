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

    public class SharedSplitMessage
    {
        public int GraphID { get; set; }
        public List<string> LineName { get; set; } = new List<string>();
    }

    public class SharedDeleteMessage
    {
        public int GraphID { get; set; }
        public string GraphName { get; set; } = string.Empty;
    }

    public class SharedNewWindowMessage
    {
        public int GraphID { get; set; }
        public string GraphName { get; set; } = string.Empty;
    }
}
