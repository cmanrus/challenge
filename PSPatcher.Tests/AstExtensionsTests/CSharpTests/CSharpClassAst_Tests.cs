using System;
using FluentAssertions;
using NUnit.Framework;
using PSPatcher.Core.AstExtensions.CSharp;
using PSPatcher.Core.AstExtensions.PS;

namespace PSPatcher.Tests.AstExtensionsTests.CSharpTests
{
    [TestFixture]
    public class CSharpClassAst_Tests
    {
        private CSharpClassAst csharpClass;

        public CSharpClassAst_Tests() : base()
        {
        } 

        [SetUp]
        public void SetUp()
        {
            csharpClass = TestCSharpParser.Parse(@"
                public class Foo
                {
                    public string First()
                    {
                    }

                    public string Second()
                    {
                    }                    
                }
                ");
        }

        [Test]
        public void GetClassName_should_get_name()
        {
            var name = csharpClass.GetClassName();

            name.Should().Be("Foo");
        }

        [Test]
        public void test()
        {
            csharpClass.AddMethod("public int Mul(int a, int b){return a* b;}");
            
            Console.WriteLine(csharpClass.GetText());
            TestCSharpParser.PsScriptAst.PatchCSharpCode(csharpClass.GetText(), csharpClass.Ast);

            Console.WriteLine(TestCSharpParser.PsScriptAst.GetText());
        }
    }
}