using System.Management.Automation.Language;

namespace PSPatcher.Core.AstExtensions.PS.Visitors
{
    public class FindLastAssignmentStatementVisitor : AstVisitor
    {
        private readonly string varName;
        private readonly CommandAst usedAst;
        private bool used;
        
        public AssignmentStatementAst LastAssignmentStatementAst { get; private set; }

        public FindLastAssignmentStatementVisitor(string varName, CommandAst usedAst)
        {
            this.varName = varName;
            this.usedAst = usedAst;
            used = false;

            LastAssignmentStatementAst = null;
        }

        public override AstVisitAction VisitAssignmentStatement(AssignmentStatementAst ast)
        {
            if (!used)
            {
                if (ast.Operator == TokenKind.Equals && ast.Left is VariableExpressionAst)
                {
                    if (((VariableExpressionAst) ast.Left).VariablePath.UserPath == varName)
                    {
                        LastAssignmentStatementAst = ast;
                    }
                }
            }

            return AstVisitAction.Continue;
        }

        public override AstVisitAction VisitCommand(CommandAst ast)
        {
            if (ast == usedAst)
                used = true;

            return AstVisitAction.Continue;
        }

    }
}