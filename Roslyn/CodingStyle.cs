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
            foreach (var binaryExpression in root.DescendantNodes().OfType<BinaryExpressionSyntax>())
            {
                if (binaryExpression.Kind() == SyntaxKind.AddExpression)
                {
                    if (binaryExpression.Left is LiteralExpressionSyntax leftLiteral && leftLiteral.IsKind(SyntaxKind.StringLiteralExpression)
                        && binaryExpression.Right is LiteralExpressionSyntax rightLiteral && rightLiteral.IsKind(SyntaxKind.StringLiteralExpression))
                    {
                        // Both sides are string literals
                        int lineNum = leftLiteral.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                        noneCodingStlye.Add(WriteNamingRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
                    }
                }
            }

            //foreach (var binaryExpression in root.DescendantNodes().OfType<BinaryExpressionSyntax>())
            //{
            //    if (binaryExpression.Kind() == SyntaxKind.AddExpression)
            //    {
            //        if (binaryExpression.Left is LiteralExpressionSyntax leftLiteral && leftLiteral.IsKind(SyntaxKind.StringLiteralExpression))
            //        {
            //            // Left side is a string literal
            //            int lineNum = leftLiteral.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            //            noneCodingStlye.Add(WriteNamingRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
            //        }
            //        else if (binaryExpression.Right is LiteralExpressionSyntax rightLiteral && rightLiteral.IsKind(SyntaxKind.StringLiteralExpression))
            //        {
            //            // Right side is a string literal
            //            int lineNum = rightLiteral.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            //            noneCodingStlye.Add(WriteNamingRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
            //        }
            //        int lineNum = binaryExpression.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            //        noneCodingStlye.Add(WriteNamingRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
            //    }
            //}
        }

        bool IsStringConcatenationRule(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.Left is LiteralExpressionSyntax leftLiteral && leftLiteral.IsKind(SyntaxKind.StringLiteralExpression)
                        && binaryExpression.Right is LiteralExpressionSyntax rightLiteral && rightLiteral.IsKind(SyntaxKind.StringLiteralExpression))
            {
                return true;
            }
            return false;
        }
    }
}
