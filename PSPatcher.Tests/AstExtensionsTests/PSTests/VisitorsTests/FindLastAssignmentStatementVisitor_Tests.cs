using System;
using System.Collections.Generic;
using System.Management.Automation.Language;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PSPatcher.Core.AstExtensions.PS.Visitors;

namespace PSPatcher.Tests.AstExtensionsTests.PSTests.VisitorsTests
{
    [TestFixture]
    public class FindLastAssignmentStatementVisitor_Tests
    {
        private FindLastAssignmentStatementVisitor visitor;
        private string varName;
        private CommandAst commandAst;
        private IScriptExtent extent;
        private IEnumerable<RedirectionAst> redirections;

        [SetUp]
        public void SetUp()
        {
            varName = Guid.NewGuid().ToString();

            extent = Substitute.For<IScriptExtent>();
            var commandElements = new List<CommandElementAst>(){new ConstantExpressionAst(extent, new object())};
            redirections = Substitute.For<IEnumerable<RedirectionAst>>();
            commandAst = new CommandAst(extent, commandElements, TokenKind.Unknown, redirections);

            visitor = new FindLastAssignmentStatementVisitor(varName, commandAst);
        }

        [Test]
        public void LastAssignmentStatementAst_should_be_null_when_not_visit_AssignmentStatement()
        {
            visitor.LastAssignmentStatementAst.Should().BeNull();
        }

        [Test]
        public void LastAssignmentStatementAst_should_be_ast()
        {
            var leftExpressionAst = new VariableExpressionAst(extent, varName , false);
            var assignmentStatementAst = GetAssignmentStatementAst(leftExpressionAst, TokenKind.Equals);

            visitor.VisitAssignmentStatement(assignmentStatementAst);

            visitor.LastAssignmentStatementAst.Should().BeSameAs(assignmentStatementAst);
        }

        [Test]
        public void LastAssignmentStatementAst_should_be_null_when_not_equals_operation()
        {
            var leftExpressionAst = new VariableExpressionAst(extent, varName, false);
            var assignmentStatementAst = GetAssignmentStatementAst(leftExpressionAst, TokenKind.MinusEquals);

            visitor.VisitAssignmentStatement(assignmentStatementAst);

            visitor.LastAssignmentStatementAst.Should().BeNull();
        }

        [Test]
        public void LastAssignmentStatementAst_should_be_null_when_not_VariableExpression()
        {
            var leftExpressionAst = new StringConstantExpressionAst(extent, varName, StringConstantType.BareWord);
            var assignmentStatementAst = GetAssignmentStatementAst(leftExpressionAst, TokenKind.Equals);

            visitor.VisitAssignmentStatement(assignmentStatementAst);

            visitor.LastAssignmentStatementAst.Should().BeNull();
        }

        [Test]
        public void LastAssignmentStatementAst_should_be_null_when_other_var_name()
        {
            var leftExpressionAst = new VariableExpressionAst(extent, "other", false);
            var assignmentStatementAst = GetAssignmentStatementAst(leftExpressionAst, TokenKind.Equals);

            visitor.VisitAssignmentStatement(assignmentStatementAst);

            visitor.LastAssignmentStatementAst.Should().BeNull();
        }

        [Test]
        public void LastAssignmentStatementAst_should_be_null_after_visit_command()
        {
            var leftExpressionAst = new VariableExpressionAst(extent, varName, false);
            var assignmentStatementAst = GetAssignmentStatementAst(leftExpressionAst, TokenKind.Equals);

            visitor.VisitCommand(commandAst);
            visitor.VisitAssignmentStatement(assignmentStatementAst);

            visitor.LastAssignmentStatementAst.Should().BeNull();
        }

        [Test]
        public void LastAssignmentStatementAst_should_be_ast_before_visit_command()
        {
            var leftExpressionAst = new VariableExpressionAst(extent, varName, false);
            var assignmentStatementAst = GetAssignmentStatementAst(leftExpressionAst, TokenKind.Equals);

            visitor.VisitAssignmentStatement(assignmentStatementAst);
            visitor.VisitCommand(commandAst);

            visitor.LastAssignmentStatementAst.Should().BeSameAs(assignmentStatementAst);
        }

        [Test]
        public void LastAssignmentStatementAst_should_be_last_ast_when_multiAssigment()
        {
            var assignmentStatementAst1 = GetAssignmentStatementAst(new VariableExpressionAst(extent, varName, false), TokenKind.Equals);
            var assignmentStatementAst2 = GetAssignmentStatementAst(new VariableExpressionAst(extent, varName, false), TokenKind.Equals);
            var assignmentStatementAst3 = GetAssignmentStatementAst(new VariableExpressionAst(extent, varName, false), TokenKind.Equals);

            visitor.VisitAssignmentStatement(assignmentStatementAst1);
            visitor.VisitAssignmentStatement(assignmentStatementAst2);
            visitor.VisitAssignmentStatement(assignmentStatementAst3);

            visitor.LastAssignmentStatementAst.Should().BeSameAs(assignmentStatementAst3);
        }

        private AssignmentStatementAst GetAssignmentStatementAst(ExpressionAst leftExpressionAst, TokenKind tokenKind)
        {
            var expressionAst = new StringConstantExpressionAst(extent, "tmp", StringConstantType.BareWord);
            var commandExpressionAst = new CommandExpressionAst(extent, expressionAst, redirections);

            return new AssignmentStatementAst(extent, leftExpressionAst, tokenKind, commandExpressionAst, extent);
        }
    }
}