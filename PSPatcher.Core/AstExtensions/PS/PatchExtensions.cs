using System.Management.Automation.Language;
using PSPatcher.Core.AstExtensions.PS.Patchers;

namespace PSPatcher.Core.AstExtensions.PS
{
    public static class PatchExtensions
    {
        public static void PatchCSharpCode(this PSScriptAst psScriptAst, string code, StringConstantExpressionAst ast)
        {
            var patchedAst = psScriptAst.ScriptBlockAst.Visit(new CSharpPatcher(code, ast, psScriptAst)) as ScriptBlockAst;
            psScriptAst.ScriptBlockAst = patchedAst;
        }

        public static void PatchInvokeMethod(this PSScriptAst psScriptAst, string newMethodName, InvokeMemberExpressionAst ast)
        {
            var patchedAst = psScriptAst.ScriptBlockAst.Visit(new InvokeMethodPatcher(newMethodName, ast, psScriptAst)) as ScriptBlockAst;
            psScriptAst.ScriptBlockAst = patchedAst;
        }
    }
}