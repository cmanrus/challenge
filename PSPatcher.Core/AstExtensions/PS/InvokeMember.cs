using System.Management.Automation.Language;

namespace PSPatcher.Core.AstExtensions.PS
{
    public class InvokeMember
    {
        public readonly string MethodName;
        public readonly InvokeMemberExpressionAst Ast;

        public InvokeMember(string methodName, InvokeMemberExpressionAst ast)
        {
            MethodName = methodName;
            Ast = ast;
        }
        
        public override string ToString()
        {
            return MethodName;
        }
    }
}