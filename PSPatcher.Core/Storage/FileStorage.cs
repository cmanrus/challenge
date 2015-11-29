using System.IO;
using System.Management.Automation.Language;
using PSPatcher.Core.AstExtensions.PS;

namespace PSPatcher.Core.Storage
{
    public class FileStorage : IStorage
    {
        public PSScriptAst Open(string name)
        {
            Token[] tokens;
            ParseError[] errors;
            var scriptBlockAst = Parser.ParseFile(name, out tokens, out errors);

            return new PSScriptAst(scriptBlockAst);
        }

        public void Save(string text, string name)
        {
            File.WriteAllText(name, text);
        }
    }
}