using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Windows.Forms;
using ICSharpCode.NRefactory.CSharp;

namespace PSPatcher.Core.AstExtensions.CSharp
{
    public class CSharpClassAst
    {
        private readonly TypeDeclaration tree;
        public readonly StringConstantExpressionAst Ast;

        public CSharpClassAst(TypeDeclaration tree, StringConstantExpressionAst ast)
        {
            this.tree = tree;
            Ast = ast;
        }

        public string GetText()
        {
            return tree.ToString();
        }

        public string GetClassName()
        {
            return tree.Name;
        }

        public List<string> GetMethodsName()
        {
            var result = new List<string>();

            foreach (var node in tree.Children)
            {
                var methodDeclaration = node as MethodDeclaration;
                if (methodDeclaration != null)
                {
                    result.Add(methodDeclaration.Name);
                }
            }

            return result;
        }
        
        public TreeNode[] GetTree()
        {
            List<TreeNode> result = new List<TreeNode>();

            foreach (var node in tree.Children)
            {
                var methodDeclaration = node as MethodDeclaration;
                if (methodDeclaration != null)
                {
                    result.Add(new TreeNode(String.Format("{0} {1} {2}",
                        methodDeclaration.Modifiers,
                        methodDeclaration.ReturnType,
                        methodDeclaration.Name)) { ToolTipText = node.ToString() });
                }
            }

            return result.ToArray();
        }

        public void AddMethod(string methodCode)
        {
            var parser = new CSharpParser();
            var syntaxTree = parser.Parse("public class Foo{" + methodCode + "}");
            var method = syntaxTree.Children.First().Children.First(x => x is MethodDeclaration).Clone();
            
            tree.AddChild<EntityDeclaration>((EntityDeclaration)method, Roles.TypeMemberRole);
        }

        public override string ToString()
        {
            return GetClassName();
        }
        
    }
}