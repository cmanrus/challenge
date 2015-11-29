using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Windows.Forms;

namespace PSPatcher.Core.AstExtensions.PS
{
    public static class DisplayExtensions
    {
        public static string GetText(this Ast ast)
        {
            return ast.ToString();
        }

        public static TreeNode[] GetTree(this Ast ast)
        {
            var index = new Dictionary<Ast, TreeNode>();
            var tree = new List<TreeNode>();

            foreach (var ast1 in ast.FindAll(_ => true, true).ToList())
            {
                var node = new TreeNode(ast1.GetType().ToString().Split('.').Last());
                node.ToolTipText = ast1.Extent.Text;

                index.Add(ast1, node);

                TreeNode parent;
                if (ast1.Parent != null && index.TryGetValue(ast1.Parent, out parent))
                {
                    parent.Nodes.Add(node);
                }
                else
                {
                    tree.Add(node);
                }
            }

            return tree.ToArray();
        }
    }
}