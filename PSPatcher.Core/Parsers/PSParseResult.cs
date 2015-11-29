using System.Collections.Generic;
using PSPatcher.Core.AstExtensions.CSharp;
using PSPatcher.Core.AstExtensions.PS;

namespace PSPatcher.Core.Parsers
{
    public struct PSParseResult
    {
        public readonly List<CSharpClassAst> CSharpClasses;
        public readonly List<InvokeMember> InvokeMembers;
        public readonly List<string> Errors;

        public PSParseResult(List<CSharpClassAst> cSharpClasses, List<InvokeMember> invokeMembers, List<string> errors)
        {
            CSharpClasses = cSharpClasses;
            InvokeMembers = invokeMembers;
            Errors = errors;
        }
    }
}