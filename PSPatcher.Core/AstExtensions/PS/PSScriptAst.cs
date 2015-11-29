using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Windows.Forms;
using PSPatcher.Core.AstExtensions.PS.Visitors;

namespace PSPatcher.Core.AstExtensions.PS
{
    public class PSScriptAst
    {
        public readonly HashSet<Ast> ModifiedAsts ;
        public ScriptBlockAst ScriptBlockAst;

        public PSScriptAst(ScriptBlockAst scriptBlockAst)
        {
            ModifiedAsts = new HashSet<Ast>();
            ScriptBlockAst = scriptBlockAst;
        }

        public string GetText()
        {
            var visitor = new GetTextAstVisitor(this);
            ScriptBlockAst.Visit(visitor);

            return visitor.Text;
        }
        
        public string GetText(Ast ast)
        {
            var visitor = new GetTextAstVisitor(this);
            ast.Visit(visitor);

            return visitor.Text;
        }

        public TreeNode[] GetTree()
        {
            var index = new Dictionary<Ast, TreeNode>();
            var tree = new List<TreeNode>();

            foreach (var ast1 in ScriptBlockAst.FindAll(_ => true, true).ToList())
            {
                var node = new TreeNode(ast1.GetType().ToString().Split('.').Last());
                node.ToolTipText = GetText(ast1);

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