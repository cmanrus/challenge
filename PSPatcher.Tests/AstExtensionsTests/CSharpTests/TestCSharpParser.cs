using System;
using System.Linq;
using PSPatcher.Core.AstExtensions.CSharp;
using PSPatcher.Core.AstExtensions.PS;
using PSPatcher.Core.Storage;

namespace PSPatcher.Tests.AstExtensionsTests.CSharpTests
{
    public class TestCSharpParser
    {
        public static PSScriptAst PsScriptAst;

        public static CSharpClassAst Parse(string text)
        {
            var storage = new StringStorage();
            PsScriptAst = storage.Open(String.Format("$someName = '{0}'", text));
            var statementAst = PsScriptAst.ScriptBlockAst.GetLastAssignmentStatementAst("someName", null);

            return statementAst.GetCSharpClasses().First();
        }


    }
}