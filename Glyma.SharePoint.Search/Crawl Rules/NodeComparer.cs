using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Compares two GlymaNodeReference objects using their node type and name.
   /// </summary>
   public class NodeComparer : System.Collections.Generic.IComparer<GlymaNodeReference>
   {
      private static Dictionary<string, int> _nodeTypeRanks;
      private static int _unspecifiedRank = 100;


      static NodeComparer()
      {
         // Define the desired order of node types.  Any node type that is not explicitly defined is assigned a value that places it after all explicitly defined node types.
         _nodeTypeRanks = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
         _nodeTypeRanks.Add(GlymaNodeTypes.Map, 0);
         _nodeTypeRanks.Add(GlymaNodeTypes.Question, 1);
         _nodeTypeRanks.Add(GlymaNodeTypes.Decision, 2);
         _nodeTypeRanks.Add(GlymaNodeTypes.Idea, 3);
         _nodeTypeRanks.Add(GlymaNodeTypes.Pro, 4);
         _nodeTypeRanks.Add(GlymaNodeTypes.Con, 5);
      }


      public int Compare(GlymaNodeReference x, GlymaNodeReference y)
      {
         int result = 0;

         if (x == null)
         {
            if (y != null)
            {
               result = -1;
            }
         }
         else if (y == null)
         {
            result = 1;
         }
         else if (x.NodeType.Equals(y.NodeType, StringComparison.OrdinalIgnoreCase))
         {
            result = string.Compare(x.Name, y.Name);
         }
         else
         {
            int xRank = _nodeTypeRanks.ContainsKey(x.NodeType) ? _nodeTypeRanks[x.NodeType] : _unspecifiedRank;
            int yRank = _nodeTypeRanks.ContainsKey(y.NodeType) ? _nodeTypeRanks[y.NodeType] : _unspecifiedRank;

            if (xRank < yRank)
            {
               result = -1;
            }
            else if (xRank > yRank)
            {
               result = 1;
            }
            else
            {
               result = string.Compare(x.Name, y.Name);
            }
         }

         return result;
      }
   }
}
