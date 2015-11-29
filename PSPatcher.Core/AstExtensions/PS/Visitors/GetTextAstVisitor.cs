using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation.Language;

namespace PSPatcher.Core.AstExtensions.PS.Visitors
{
    public class GetTextAstVisitor : ICustomAstVisitor
    {
        private readonly PSScriptAst rootAst;
        public string Text = "";

        public GetTextAstVisitor(PSScriptAst ast)
        {
            rootAst = ast;
        }

        public void VisitElements<T>(ReadOnlyCollection<T> elements) where T : Ast
        {
            if (elements == null)
            {
                return;
            }


            foreach (T t in elements)
            {
                t.Visit(this);
            }

        }

        public void VisitElement<T>(T element) where T : Ast
        {
            if (element == null)
                return;
            element.Visit(this);
        }

        public object VisitFunctionDefinition(FunctionDefinitionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                Text += "function " + ast.Name + "(";
                if (ast.Parameters != null)
                    for (int i = 0; i < ast.Parameters.Count; i++)
                    {
                        ast.Parameters[i].Visit(this);
                        if (i < ast.Parameters.Count - 1)
                            Text += ", ";
                    }

                Text += ")\n{";
                VisitElement(ast.Body);
                Text += "}";
            }
            Text += "\n";
            return ast;
        }

        public object VisitIfStatement(IfStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                for (int index = 0; index < ast.Clauses.Count; index++)
                {
                    var clause = ast.Clauses[index];
                    if (index == 0)
                        Text += "if";
                    else
                        Text += "elseif";
                    Text += "(";
                    VisitElement(clause.Item1);
                    Text += ")";
                    VisitElement(clause.Item2);
                }

                if (ast.ElseClause != null)
                {
                    Text += "else";
                    ast.ElseClause.Visit(this);
                }
            }

            Text += "\n";
            return ast;
        }

        public object VisitStatementBlock(StatementBlockAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                Text += "{\n";
                if (ast.Statements != null)
                {
                    foreach (StatementAst statement in ast.Statements)
                    {
                        statement.Visit(this);
                        Text += ";\n";
                    }
                }
                VisitElements(ast.Traps);
                Text += "}";
            }
            return ast;
        }

        public object VisitInvokeMemberExpression(InvokeMemberExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Expression);
                Text += ".";
                VisitElement(ast.Member);
                Text += "(";

                if (ast.Arguments != null)
                {
                    for (int i = 0; i < ast.Arguments.Count; i++)
                    {
                        ast.Arguments[i].Visit(this);
                        if (i < ast.Arguments.Count - 1)
                        {
                            Text += ", ";
                        }
                    }
                }
                
                Text += ")";
            }
            return ast;
        }

        public object VisitStringConstantExpression(StringConstantExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                switch (ast.StringConstantType)
                {
                    case StringConstantType.SingleQuoted:
                        Text += "'";
                        break;
                    case StringConstantType.SingleQuotedHereString:
                        Text += "@'\n";
                        break;
                    case StringConstantType.DoubleQuoted:
                        Text += "\"";
                        break;
                    case StringConstantType.DoubleQuotedHereString:
                        Text += "@\"\n";
                        break;
                }
                Text += ast.Value;
                switch (ast.StringConstantType)
                {
                    case StringConstantType.SingleQuoted:
                        Text += "'";
                        break;
                    case StringConstantType.SingleQuotedHereString:
                        Text += "\n'@";
                        break;
                    case StringConstantType.DoubleQuoted:
                        Text += "\"";
                        break;
                    case StringConstantType.DoubleQuotedHereString:
                        Text += "\n\"@\n";
                        break;
                }
            }
            return ast;
        }

        public object VisitAssignmentStatement(AssignmentStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Left);
                Text += " " + TokenTraits.Text(ast.Operator) + " ";
                VisitElement(ast.Right);
            }
            
            return ast;
        }

        public object VisitScriptBlock(ScriptBlockAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.ParamBlock);
                VisitElement(ast.BeginBlock);
                VisitElement(ast.ProcessBlock);
                VisitElement(ast.EndBlock);
                VisitElement(ast.DynamicParamBlock);
            }

            Text += "\n";
            return ast;
        }

        public object VisitNamedBlock(NamedBlockAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElements(ast.Traps);

                if (ast.Statements != null)
                    foreach (var t in ast.Statements)
                    {
                        t.Visit(this);
                        Text += "\n";
                    }
            }
            return ast;
        }

        public object VisitTrap(TrapStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.TrapType);
                VisitElement(ast.Body);
            }
            return ast;
        }

        public object VisitSwitchStatement(SwitchStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Condition);

                foreach (var clause in ast.Clauses)
                {
                    VisitElement(clause.Item1);
                        VisitElement(clause.Item2);
                }

                VisitElement(ast.Default);

                return ast;
            }
            return ast;
        }

        public object VisitDataStatement(DataStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Body);
                VisitElements(ast.CommandsAllowed);

                return ast;
            }
            return ast;
        }

        public object VisitForEachStatement(ForEachStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Variable);
                VisitElement(ast.Condition);
                VisitElement(ast.Body);
            }
            return ast;
        }

        public object VisitDoWhileStatement(DoWhileStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Condition);
                VisitElement(ast.Body);

            }
            return ast;
        }

        public object VisitForStatement(ForStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Initializer);
                VisitElement(ast.Condition);
                VisitElement(ast.Iterator);
                VisitElement(ast.Body);

            }
            return ast;
        }

        public object VisitWhileStatement(WhileStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Condition);
                VisitElement(ast.Body);

            }
            return ast;
        }

        public object VisitCatchClause(CatchClauseAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Body);

            }
            return ast;
        }

        public object VisitTryStatement(TryStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Body);
                VisitElements(ast.CatchClauses);
                VisitElement(ast.Finally);

            }
            return ast;
        }

        public object VisitDoUntilStatement(DoUntilStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Condition);
                VisitElement(ast.Body);

            }
            return ast;
        }

        public object VisitParamBlock(ParamBlockAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElements(ast.Attributes);
                VisitElements(ast.Parameters);

            }
            return ast;
        }

        public object VisitErrorStatement(ErrorStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                return ast;
            }
            return ast;
        }

        public object VisitErrorExpression(ErrorExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                return ast;
            }
            return ast;
        }

        public object VisitTypeConstraint(TypeConstraintAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {

            }
            return ast;
        }

        public object VisitAttribute(AttributeAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElements(ast.PositionalArguments);
                VisitElements(ast.NamedArguments);

            }
            return ast;
        }

        public object VisitNamedAttributeArgument(NamedAttributeArgumentAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Argument);

            }
            return ast;
        }

        public object VisitParameter(ParameterAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Name);
                VisitElements(ast.Attributes);
                VisitElement(ast.DefaultValue);

            }
            return ast;
        }

        public object VisitBreakStatement(BreakStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Label);

            }
            return ast;
        }

        public object VisitContinueStatement(ContinueStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Label);

            }
            return ast;
        }

        public object VisitReturnStatement(ReturnStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Pipeline);

            }
            Text += "\n";
            return ast;
        }

        public object VisitExitStatement(ExitStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Pipeline);

            }
            return ast;
        }

        public object VisitThrowStatement(ThrowStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Pipeline);

            }
            return ast;
        }

        

        public object VisitPipeline(PipelineAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElements(ast.PipelineElements);

            }
            return ast;
        }

        public object VisitCommand(CommandAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElements(ast.CommandElements);
                VisitElements(ast.Redirections);

            }
            return ast;
        }

        public object VisitCommandExpression(CommandExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Expression);
                VisitElements(ast.Redirections);

            }
            return ast;
        }

        public object VisitCommandParameter(CommandParameterAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Argument);

            }
            return ast;
        }

        public object VisitFileRedirection(FileRedirectionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Location);

            }
            return ast;
        }

        public object VisitMergingRedirection(MergingRedirectionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {

            }
            return ast;
        }

        public object VisitBinaryExpression(BinaryExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Left);
                VisitElement(ast.Right);

            }
            return ast;
        }

        public object VisitUnaryExpression(UnaryExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Child);

            }
            return ast;
        }

        public object VisitConvertExpression(ConvertExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Child);
                VisitElement(ast.Type);

            }
            return ast;
        }

        public object VisitTypeExpression(TypeExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {

            }
            return ast;
        }

        public object VisitConstantExpression(ConstantExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {

            }
            return ast;
        }

        
        public object VisitSubExpression(SubExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.SubExpression);

            }
            return ast;
        }

        public object VisitUsingExpression(UsingExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.SubExpression);

            }
            return ast;
        }

        public object VisitVariableExpression(VariableExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += " " + ast.Extent.Text;
            }
            else
            {

            }
            return ast;
        }

        public object VisitMemberExpression(MemberExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Expression);
                VisitElement(ast.Member);

            }
            return ast;
        }

        public object VisitArrayExpression(ArrayExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.SubExpression);

            }
            return ast;
        }

        public object VisitArrayLiteral(ArrayLiteralAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElements(ast.Elements);

            }
            return ast;
        }

        public object VisitHashtable(HashtableAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                foreach (var keyValuePair in ast.KeyValuePairs)
                {
                        VisitElement(keyValuePair.Item1);
                        VisitElement(keyValuePair.Item2);
                }
            }
            return ast;
        }

        public object VisitScriptBlockExpression(ScriptBlockExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.ScriptBlock);

            }
            return ast;
        }

        public object VisitParenExpression(ParenExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Pipeline);

            }
            return ast;
        }

        public object VisitExpandableStringExpression(ExpandableStringExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {

            }
            return ast;
        }

        public object VisitIndexExpression(IndexExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Target);
                VisitElement(ast.Index);

            }
            return ast;
        }

        public object VisitAttributedExpression(AttributedExpressionAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Attribute);
                VisitElement(ast.Child);

            }
            return ast;
        }

        public object VisitBlockStatement(BlockStatementAst ast)
        {
            if (!ast.FindAll(x => rootAst.ModifiedAsts.Contains(x), true).Any())
            {
                Text += ast.Extent.Text;
            }
            else
            {
                VisitElement(ast.Body);

            }
            return ast;
        }

    }
}