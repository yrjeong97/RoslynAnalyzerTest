using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Roslyn
{
    public class CodingStyle : WriteNamingRuleReport
    {
        string[] csFilesList;
        string projectPath;
        string reportFilePath;
        List<string> noneCodingStlye;

        public CodingStyle(string[] csFilesList, string projectPath, string reportFilePath)
        {
            this.csFilesList = csFilesList;
            this.projectPath = projectPath;
            this.reportFilePath = reportFilePath;
            this.noneCodingStlye = new List<string>();
        }

        public List<string> AnalyzeCodingStyle()
        {
            foreach(var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                AnalyzeStringConcatenation(root, csFile);
            }

            return noneCodingStlye;
        }

        void AnalyzeStringConcatenation(SyntaxNode root, string csFile)
        {
            var uninitializedVariables = new HashSet<string>();

            foreach (var declaration in root.DescendantNodes().OfType<VariableDeclarationSyntax>())
            {
                var variableType = declaration.Type as PredefinedTypeSyntax;
                if (variableType != null && variableType.Keyword.Text == "string")
                {
                    foreach (var variable in declaration.Variables)
                    {
                        if (variable.Initializer == null)
                        {
                            uninitializedVariables.Add(variable.Identifier.Text);
                        }
                    }
                }
            }

            foreach (var binaryExpression in root.DescendantNodes().OfType<BinaryExpressionSyntax>())
            {
                if (IsStringConcatenationRule(binaryExpression, uninitializedVariables))
                {
                    int lineNum = binaryExpression.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    noneCodingStlye.Add(WriteNamingRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
                }
            }
        }

        bool IsStringConcatenationRule(BinaryExpressionSyntax binaryExpression, HashSet<string> uninitializedVariables)
        {
            if (IsVariableOrLiteral(binaryExpression.Left, uninitializedVariables) && IsVariableOrLiteral(binaryExpression.Right, uninitializedVariables))
            {
                return true;
            }
            return false;
        }

        bool IsVariableOrLiteral(ExpressionSyntax expression, HashSet<string> uninitializedVariables)
        {
            if (expression is IdentifierNameSyntax identifierNameSyntax)
            {
                if (uninitializedVariables.Contains(identifierNameSyntax.Identifier.Text))
                {
                    return true;
                }
            }
            return false;
        }
       

        //void AnalyzeStringConcatenation(SyntaxNode root, string csFile)
        //{
        //    var uninitializedVariables = new HashSet<string>();

        //    foreach (var declaration in root.DescendantNodes().OfType<VariableDeclarationSyntax>())
        //    {
        //        var variableType = declaration.Type as PredefinedTypeSyntax;
        //        if (variableType != null && variableType.Keyword.Text == "string")
        //        {
        //            foreach (var variable in declaration.Variables)
        //            {
        //                if (variable.Initializer == null)
        //                {
        //                    uninitializedVariables.Add(variable.Identifier.Text);
        //                }
        //            }
        //        }
        //    }

        //    foreach (var binaryExpression in root.DescendantNodes().OfType<BinaryExpressionSyntax>())
        //    {
        //        var leftIdentifier = binaryExpression.Left as IdentifierNameSyntax;
        //        var rightIdentifier = binaryExpression.Right as IdentifierNameSyntax;

        //        if (leftIdentifier != null && uninitializedVariables.Contains(leftIdentifier.Identifier.Text))
        //        {
        //            int lineNum = binaryExpression.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        //            noneCodingStlye.Add(WriteNamingRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
        //        }

        //        if (rightIdentifier != null && uninitializedVariables.Contains(rightIdentifier.Identifier.Text))
        //        {
        //            int lineNum = binaryExpression.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        //            noneCodingStlye.Add(WriteNamingRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
        //        }
        //    }
        //}

        //bool IsStringConcatenationRule(BinaryExpressionSyntax binaryExpression)
        //{
        //    if (binaryExpression.Left is LiteralExpressionSyntax leftLiteral && leftLiteral.IsKind(SyntaxKind.StringLiteralExpression)
        //                && binaryExpression.Right is LiteralExpressionSyntax rightLiteral && rightLiteral.IsKind(SyntaxKind.StringLiteralExpression))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
