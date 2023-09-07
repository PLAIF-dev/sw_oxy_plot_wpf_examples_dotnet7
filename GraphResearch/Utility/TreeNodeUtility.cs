using GraphResearch.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphResearch.Utility
{
    public static class TreeNodeUtility
    {
        public static List<PositionNode> TravelNode(TreeNode node)
        {
            List<PositionNode> positions = new List<PositionNode>();

            try
            {
                TravelNode(node, ref positions);

                return positions;
            }
            catch
            {
                return positions;
            }
        }

        private static void TravelNode(TreeNode node, ref List<PositionNode> positions)
        {
            try
            {
                if(node is PositionNode positionNode)
                {
                    positions.Add(positionNode);
                }
                else
                {
                    foreach (TreeNode child in node.Children)
                    {
                        TravelNode(child, ref positions);
                    }
                }

                //foreach (TreeNode child in node.Children)
                //{
                //    if (child is PositionNode positionNode)
                //    {
                //        positions.Add(positionNode);
                //    }

                //    TravelNode(child, ref positions);
                //}
            }
            catch
            {

            }
        }
    }
}
