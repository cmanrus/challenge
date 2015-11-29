using System;
using System.Management.Automation.Language;
using PSPatcher.Core.AstExtensions.PS;

namespace PSPatcher.Core.Storage
{
    public class StringStorage : IStorage
    {
        public PSScriptAst Open(string text)
        {
            ParseError[] b;
            Token[] a;

            var scriptBlockAst = Parser.ParseInput(text, out a, out b);
            return new PSScriptAst(scriptBlockAst);
        }

        public void Save(string text, string name)
        {
            throw new NotSupportedException();
        }
    }
}