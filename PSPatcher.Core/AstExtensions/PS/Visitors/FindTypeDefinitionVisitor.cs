using System.Management.Automation.Language;

namespace PSPatcher.Core.AstExtensions.PS.Visitors
{
    public class FindTypeDefinitionVisitor : AstVisitor
    {
        public FindTypeDefinitionVisitor()
        {
            IsTypeDefinitionAst = false;
            VarName = null;
        }

        public string VarName{ get; private set; }
        public bool IsTypeDefinitionAst { get; private set; }

        public override AstVisitAction VisitCommandParameter(CommandParameterAst ast)
        {
            if (ast.ParameterName.ToLower() == "typedefinition")
                IsTypeDefinitionAst = true;

            return AstVisitAction.Continue;
        }

        public override AstVisitAction VisitVariableExpression(VariableExpressionAst ast)
        {
            if (IsTypeDefinitionAst)
                VarName = ast.VariablePath.UserPath;

            return AstVisitAction.Continue;
        }
    }
}