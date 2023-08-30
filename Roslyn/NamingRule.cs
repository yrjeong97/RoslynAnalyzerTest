using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn
{   
    public class NamingRule
    {
        string reportFilePath;
        string projectPath;
        string[] csFilesList;

      
        public NamingRule(string reportFilePath, string projectPath, string[] csFilesList)
        {
            this.reportFilePath = reportFilePath;
            this.projectPath = projectPath;
            this.csFilesList = csFilesList;
        }

        public void CheckPascalCase()
        {
            var nonPascalNames = new List<string>();
            foreach (var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();
                var relativeFilePath = fullPath.Substring(projectPath.Length).TrimStart('\\', '/');

                foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    var className = classDeclaration.Identifier.ValueText;
                    if (!IsPascalCase(className))
                    {
                        nonPascalNames.Add($"Pascal Rule{Environment.NewLine}" +
                            $"class name: {className}{Environment.NewLine}" +
                            $"File name: {relativeFilePath}{Environment.NewLine}" +
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
                                $"File name: {relativeFilePath}{Environment.NewLine}" +
                                $"line number: {method.GetLocation().GetLineSpan().StartLinePosition.Line + 1}{Environment.NewLine}" +
                                $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}");
                        }
                    }
                }
            }
        }

        void createContents(List<string> contents, string reportFilePath)
        {
            if (contents.Any())
            {
                var reportContent = string.Join(Environment.NewLine, contents);

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
}
