using PSPatcher.Core.AstExtensions.PS;

namespace PSPatcher.Core.Storage
{
    public interface IStorage
    {
        PSScriptAst Open(string name);
        void Save(string text, string name);
    }
}