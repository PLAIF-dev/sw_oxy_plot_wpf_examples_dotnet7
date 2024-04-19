using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraphResearch.Model
{
    public class TreeNode
    {
        public string Name { get; set; } = string.Empty;
        public ObservableCollection<TreeNode> Children { get; set; } = new ObservableCollection<TreeNode>();
    }

    public class PositionNode : TreeNode
    {
        public List<double> PositionsX { get; set; } = new List<double>();
        public List<double> PositionsY { get; set; } = new List<double>();
    }
}
