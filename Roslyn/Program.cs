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
        var reportFilePath = Environment.GetEnvironmentVariable("REPORT_FILE_PATH");
        var changedFiles = Environment.GetEnvironmentVariable("CHANGED_FILES");

        if (string.IsNullOrEmpty(reportFilePath))
        {
            Console.WriteLine("Report file path not specified.");
            return;
        }
        var projectPath = Environment.GetEnvironmentVariable("PROJECT_PATH");
        var nonPascalNames = new List<string>();

        var csFilesList = changedFiles.Split(';'); // Assuming the names are semicolon separated

        foreach (var csFile in csFilesList)
        {
            var fullPath = Path.Combine(projectPath, csFile);
            var code = File.ReadAllText(fullPath);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();
            //var relativeFilePath = fullPath.Substring(projectPath.Length).TrimStart('\\', '/');

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