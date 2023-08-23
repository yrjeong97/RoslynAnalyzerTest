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
        var projectPath = Environment.GetEnvironmentVariable("PROJECT_PATH");
        var nonPascalNames = new List<string>();

        var csFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);

        foreach (var csFile in csFiles)
        {
            var code = File.ReadAllText(csFile);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();

            var classNames = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Select(c => c.Identifier.ValueText);
            foreach (var className in classNames)
            {
                if (!IsPascalCase(className))
                {
                    nonPascalNames.Add($"Class '{className}' in file '{csFile}' does not follow PascalCase");
                }
            }

            // Check method names
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var method in methods)
            {
                var methodName = method.Identifier.ValueText;
                if (!IsPascalCase(methodName))
                {
                    nonPascalNames.Add($"Method '{methodName}' in file '{csFile}' does not follow PascalCase");
                }
            }
        }

        if (nonPascalNames.Any())
        {
            var reportContent = string.Join(Environment.NewLine, nonPascalNames);
            var reportFilePath = "non_pascal_names.txt";

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