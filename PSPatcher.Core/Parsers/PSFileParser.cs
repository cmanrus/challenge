using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using PSPatcher.Core.AstExtensions.PS;

namespace PSPatcher.Core.Parsers
{
    public static class PSFileParser
    {
        public static PSParseResult Parse(PSScriptAst psScriptAst)
        {
            var addTypeAsts = psScriptAst.ScriptBlockAst.GetAddTypeAsts();

            var varsWithCode = addTypeAsts
                .Select(ast => new Tuple<string, CommandAst>(ast.GetTypeDefinitionVarNames(), ast))
                .Where(x => x.Item1 != null).ToList();

            var defineVarAsts = varsWithCode
                .Select(x => psScriptAst.ScriptBlockAst.GetLastAssignmentStatementAst(x.Item1, x.Item2))
                .Where(x => x != null).ToList();

            var csharpClassAsts = defineVarAsts.SelectMany(x => x.GetCSharpClasses()).ToList();

            var invokeMembers = psScriptAst.GetInvokeMemberExpressionAst();

            return new PSParseResult(csharpClassAsts, invokeMembers, new List<string>());
        }
    }
}