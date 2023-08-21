using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program
{
    static void Main(string[] args)
    {
        var calcProjectPath = @"D:\RoslynAnalyzerTest\Calc"; // Calc 프로젝트 폴더 경로
        var nonPascalMethods = new List<string>();

        var csFiles = Directory.GetFiles(calcProjectPath, "*.cs", SearchOption.AllDirectories);

        foreach (var csFile in csFiles)
        {
            var code = File.ReadAllText(csFile);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                var methodName = method.Identifier.ValueText;
                if (!IsPascalCase(methodName))
                {
                    nonPascalMethods.Add($"Method '{methodName}' in file '{csFile}' does not follow PascalCase");
                }
            }
        }

        if (nonPascalMethods.Any())
        {
            var reportContent = string.Join(Environment.NewLine, nonPascalMethods);
            var reportFilePath = "non_pascal_methods.txt";

            File.WriteAllText(reportFilePath, reportContent);
            Console.WriteLine("Non-PascalCase methods report saved.");
        }
        else
        {
            Console.WriteLine("All methods follow PascalCase.");
        }
    }

    static bool IsPascalCase(string s)
    {
        return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
    }
}
