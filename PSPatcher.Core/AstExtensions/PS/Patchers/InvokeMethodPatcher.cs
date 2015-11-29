using System.Management.Automation.Language;
using PSPatcher.Core.AstExtensions.PS.Visitors;

namespace PSPatcher.Core.AstExtensions.PS.Patchers
{
    public class InvokeMethodPatcher : CopyAstVisitor
    {
        private readonly string newMethodName;
        private readonly InvokeMemberExpressionAst ast;
        
        public InvokeMethodPatcher(string newMethodName, InvokeMemberExpressionAst ast, PSScriptAst rootAst)
            : base(rootAst)
        {
            this.newMethodName = newMethodName;
            this.ast = ast;
        }

        public override object VisitInvokeMemberExpression(InvokeMemberExpressionAst invokeMemberExpressionAst)
        {
            var newExpression = VisitElement(invokeMemberExpressionAst.Expression);
            CommandElementAst newMethod;
            if (invokeMemberExpressionAst == ast)
            {
                newMethod = new StringConstantExpressionAst(invokeMemberExpressionAst.Member.Extent, newMethodName,
                    ((StringConstantExpressionAst) invokeMemberExpressionAst.Member).StringConstantType);
                rootAst.ModifiedAsts.Add(newMethod);
            }
            else
            {
                newMethod = VisitElement(invokeMemberExpressionAst.Member);
            }
            var newArguments = VisitElements(invokeMemberExpressionAst.Arguments);
            return new InvokeMemberExpressionAst(invokeMemberExpressionAst.Extent, newExpression, newMethod,
                newArguments, invokeMemberExpressionAst.Static);
        }
    }
}