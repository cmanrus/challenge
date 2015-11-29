using System.Management.Automation.Language;
using PSPatcher.Core.AstExtensions.PS.Visitors;

namespace PSPatcher.Core.AstExtensions.PS.Patchers
{
    public class CSharpPatcher : CopyAstVisitor
    {
        private readonly string code;
        private readonly StringConstantExpressionAst ast;

        public CSharpPatcher(string code, StringConstantExpressionAst ast, PSScriptAst rootAst)
            : base(rootAst)
        {
            this.code = code;
            this.ast = ast;
        } 

        public override object VisitStringConstantExpression(StringConstantExpressionAst stringConstantExpressionAst)
        {
            if (stringConstantExpressionAst == ast)
            {
                var newAst = new StringConstantExpressionAst(stringConstantExpressionAst.Extent,
                    code,
                    stringConstantExpressionAst.StringConstantType);

                rootAst.ModifiedAsts.Add(newAst);

                return newAst;
            }
            else
                return base.VisitStringConstantExpression(stringConstantExpressionAst);
        }
    }
}