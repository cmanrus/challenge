using System.Management.Automation.Language;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PSPatcher.Core.AstExtensions.PS.Visitors;

namespace PSPatcher.Tests.AstExtensionsTests.PSTests.VisitorsTests
{
    [TestFixture]
    public class FindTypeDefinitionVisitor_Tests
    {
        private FindTypeDefinitionVisitor visitor;
        private IScriptExtent extent;
        private VariableExpressionAst argument;

        [SetUp]
        public void SetUp()
        {
            extent = Substitute.For<IScriptExtent>();
            argument = new VariableExpressionAst(extent, "other", false);

            visitor = new FindTypeDefinitionVisitor();
        }

        [Test]
        public void IsTypeDefinitionAst_should_be_false_by_default()
        {
            visitor.IsTypeDefinitionAst.Should().BeFalse();
        }

        [Test]
        public void IsTypeDefinitionAst_should_be_true_when_visit_typedefinition_ast()
        {
            var commandParameterAst = new CommandParameterAst(extent, "TypeDefinition", argument, extent);

            visitor.VisitCommandParameter(commandParameterAst);

            visitor.IsTypeDefinitionAst.Should().BeTrue();
        }

        [Test]
        public void IsTypeDefinitionAst_should_be_false_when_visit_not_typedefinition_CommandParameterAst()
        {
            var commandParameterAst = new CommandParameterAst(extent, "Other", argument, extent);

            visitor.VisitCommandParameter(commandParameterAst);

            visitor.IsTypeDefinitionAst.Should().BeFalse();
        }

        [Test]
        public void VarName_should_return_name_from_argument()
        {
            var commandParameterAst = new CommandParameterAst(extent, "TypeDefinition", argument, extent);

            visitor.VisitCommandParameter(commandParameterAst);
            visitor.VisitVariableExpression(argument);

            visitor.VarName.Should().Be(argument.VariablePath.UserPath);
        }
    }
}