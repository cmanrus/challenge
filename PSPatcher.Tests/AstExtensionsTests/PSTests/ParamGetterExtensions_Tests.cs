using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PSPatcher.Core.AstExtensions.PS;
using PSPatcher.Core.Storage;

namespace PSPatcher.Tests.AstExtensionsTests.PSTests
{
    [TestFixture]
    public class ParamGetterExtensions_Tests
    {
        private StringStorage storage;

        [SetUp]
        public void SetUp()
        {
            storage = new StringStorage();
        }

        [Test]
        public void GetAddTypeAsts_should_get_ast()
        {
            var ast = storage.Open("Add-Type smdstr").ScriptBlockAst;

            var commandAsts = ast.GetAddTypeAsts();

            commandAsts.Count.Should().Be(1);
        }

        [Test]
        public void GetTypeDefinitionVarNames_should_get_var_name_for_commandAst()
        {
            var ast = storage.Open("Add-Type -TypeDefinition $correctName").ScriptBlockAst;
            var commandAsts = ast.GetAddTypeAsts();

            var name = commandAsts.First().GetTypeDefinitionVarNames();
            
            name.Should().Be("correctName");
        }

        [Test]
        public void GetTypeDefinitionVarNames_should_get_null_for_bad_commandAst()
        {
            var ast = storage.Open("Add-Type -TypeDefinition 'some code'").ScriptBlockAst;
            var commandAsts = ast.GetAddTypeAsts();

            var name = commandAsts.First().GetTypeDefinitionVarNames();

            name.Should().BeNull();
        }

        [Test]
        public void GetLastAssignmentStatementAst_should_get_ast()
        {
            var ast = storage.Open("$someName = 'some code'").ScriptBlockAst;

            var result = ast.GetLastAssignmentStatementAst("someName", null);

            result.Should().NotBeNull();
        }

        [Test]
        public void GetLastAssignmentStatementAst_should_get_null_when_var_not_def()
        {
            var ast = storage.Open("$otherName = 'some code'").ScriptBlockAst;

            var result = ast.GetLastAssignmentStatementAst("someName", null);

            result.Should().BeNull();
        }

        [Test]
        public void GetCSharpClasses_should_get_all_classes()
        {
            var ast = storage.Open(String.Format("$someName = '{0} {1}'",
                "public class First{}",
                "public class Second{}")).ScriptBlockAst;
            var statementAst = ast.GetLastAssignmentStatementAst("someName", null);

            var cSharpClassAsts = statementAst.GetCSharpClasses();

            cSharpClassAsts.Count.Should().Be(2);
            cSharpClassAsts.Select(x => x.GetClassName()).Should().BeEquivalentTo("First", "Second");
        }

        [Test]
        public void GetInvokeMemberExpressionAst_shold_get_all_invoke_members()
        {
            var ast = storage.Open(String.Format("{0}\n{1}\n{2}'",
                "$first.FFunc()",
                "$second.SFunc()",
                "$first.SFunc()"));

            var result = ast.GetInvokeMemberExpressionAst();

            result.Count.Should().Be(3);
            result.Select(x => x.MethodName).Should().BeEquivalentTo("FFunc", "SFunc", "SFunc");
        }
    }
}