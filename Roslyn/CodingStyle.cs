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
        List<string> noneCodingStyle;
        private readonly Compilation compilation;
        private readonly SemanticModel semanticModel;

        public CodingStyle(string[] csFilesList, string projectPath, string reportFilePath)
        {
            this.csFilesList = csFilesList;
            this.projectPath = projectPath;
            this.reportFilePath = reportFilePath;
            this.noneCodingStyle = new List<string>();

            // CreateCompilation 메서드 수정
            compilation = CreateCompilation();

            // 컴파일러에 구문 트리 추가
            foreach (var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var csSyntaxTree = CSharpSyntaxTree.ParseText(code);
                compilation = compilation.AddSyntaxTrees(csSyntaxTree);
            }

            // SemanticModel 초기화
            var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
            if (syntaxTree != null)
            {
                semanticModel = compilation.GetSemanticModel(syntaxTree);
            }
        }

        public List<string> AnalyzeCodingStyle()
        {
            foreach (var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                AnalyzeStringConcatenation(root, csFile);
            }

            return noneCodingStyle;
        }

        void AnalyzeStringConcatenation(SyntaxNode root, string csFile)
        {
            var stringVariables = new HashSet<string>();

            foreach (var declaration in root.DescendantNodes().OfType<VariableDeclarationSyntax>())
            {
                var variableType = declaration.Type as PredefinedTypeSyntax;
                if (variableType != null && variableType.Keyword.Text == "string")
                {
                    foreach (var variable in declaration.Variables)
                    {
                        stringVariables.Add(variable.Identifier.Text);
                    }
                }
            }

            foreach (var binaryExpression in root.DescendantNodes().OfType<BinaryExpressionSyntax>())
            {
                // 왼쪽 및 오른쪽 피연산자의 형식 확인
                var leftType = semanticModel.GetTypeInfo(binaryExpression.Left).Type;
                var rightType = semanticModel.GetTypeInfo(binaryExpression.Right).Type;

                // 형식이 string이면서 변수 이름이 string 변수 목록에 포함되어 있는 경우 경고 추가
                if ((leftType?.SpecialType == SpecialType.System_String || rightType?.SpecialType == SpecialType.System_String) &&
                    (leftType != null && stringVariables.Contains(leftType.Name) || rightType != null && stringVariables.Contains(rightType.Name)))
                {
                    int lineNum = binaryExpression.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    noneCodingStyle.Add(WriteNamingRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
                }
            }
        }

        // CreateCompilation 메서드 수정
        private Compilation CreateCompilation()
        {
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

            return CSharpCompilation.Create("MyCompilation")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references);
        }
    }
}
