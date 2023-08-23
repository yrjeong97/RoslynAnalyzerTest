using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var projectPath = Environment.GetEnvironmentVariable("PROJECT_PATH");
        var nonPascalNames = new List<string>();

        var csFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);

        foreach (var csFile in csFiles)
        {
            var code = File.ReadAllText(csFile);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();

            foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var className = classDeclaration.Identifier.ValueText;
                if (!IsPascalCase(className))
                {
                    nonPascalNames.Add($"Pascal Rule{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}");
                }

                foreach (var method in classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    var methodName = method.Identifier.ValueText;
                    if (!IsPascalCase(methodName))
                    {
                        nonPascalNames.Add($"Pascal Rule{Environment.NewLine}" +
                            $"method name: {methodName}{Environment.NewLine}" +
                            $"class name: {className}{Environment.NewLine}" +
                            $"File name: {csFile}{Environment.NewLine}" +
                            $"line number: {method.GetLocation().GetLineSpan().StartLinePosition.Line + 1}{Environment.NewLine}" +
                            $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}");
                    }
                }
            }
        }

        if (nonPascalNames.Any())
        {
            var reportContent = string.Join(Environment.NewLine, nonPascalNames);
            var reportFilePath = "non_pascal_names00.txt";

            File.WriteAllText(reportFilePath, reportContent);
            Console.WriteLine("Non-PascalCase names report saved.");
        }
        else
        {
            Console.WriteLine("All names follow PascalCase.");
        }


    }

    static bool IsPascalCase(string s)
    {
        return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
    }
}
