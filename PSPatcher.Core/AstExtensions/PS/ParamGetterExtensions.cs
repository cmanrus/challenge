using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using ICSharpCode.NRefactory.CSharp;
using PSPatcher.Core.AstExtensions.CSharp;
using PSPatcher.Core.AstExtensions.PS.Visitors;

namespace PSPatcher.Core.AstExtensions.PS
{
    public static class ParamGetterExtensions
    {
        public static List<CommandAst> GetAddTypeAsts(this Ast ast)
        {
            var result = new List<CommandAst>();

            foreach (var commandAst in ast.FindAll(x => x is CommandAst, true))
            {
                if (((CommandAst)commandAst).IsAddTypeCommand())
                {
                    result.Add((CommandAst)commandAst);
                }
            }

            return result;
        }

        public static string GetTypeDefinitionVarNames(this CommandAst commandAst)
        {
            var visitor = new FindTypeDefinitionVisitor();
            commandAst.Visit(visitor);

            return visitor.IsTypeDefinitionAst ? visitor.VarName : null;
        }

        public static AssignmentStatementAst GetLastAssignmentStatementAst(this Ast ast, string varName, CommandAst usedAst)
        {
            var visitor = new FindLastAssignmentStatementVisitor(varName, usedAst);
            ast.Visit(visitor);

            return visitor.LastAssignmentStatementAst;
        }

        public static List<CSharpClassAst> GetCSharpClasses(this AssignmentStatementAst ast)
        {
            var result = new List<CSharpClassAst>();

            var parser = new CSharpParser();

            foreach (var str in ast.Right.FindAll(x => x is StringConstantExpressionAst,true))
            {
                var value = ((StringConstantExpressionAst)str).Value;
                var syntaxTree = parser.Parse(value);

                if (!syntaxTree.Errors.Any())
                    foreach (var child in syntaxTree.Children)
                    {
                        var tree = child as TypeDeclaration;
                        if (tree != null)
                        {
                            if (tree.ClassType == ClassType.Class)
                                result.Add(new CSharpClassAst(tree, (StringConstantExpressionAst)str));
                        }
                    }
            }

            return result;
        }

        public static List<InvokeMember> GetInvokeMemberExpressionAst(this PSScriptAst ast)
        {
            var result = new List<InvokeMember>();

            foreach (var invokeAst in ast.ScriptBlockAst.FindAll(x => x is InvokeMemberExpressionAst, true))
            {
                result.Add(new InvokeMember(ast.GetText(((InvokeMemberExpressionAst)invokeAst).Member), (InvokeMemberExpressionAst)invokeAst));
            }

            return result;
        }

        private static bool IsAddTypeCommand(this CommandAst ast)
        {
            return ast.GetCommandName().ToLower() == "add-type";
        }
    }
}