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
        string[] csFilesList;
        string projectPath;
        string reportFilePath;
        List<string> nonPascalNames;

        public NamingRule(string[] csFilesList, string projectPath, string reportFilePath)
        {
            this.csFilesList = csFilesList;
            this.projectPath = projectPath;
            this.reportFilePath = reportFilePath;
            this.nonPascalNames = new List<string>();
        }

        public List<string> CheckPascal()
        {
            foreach (var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                AnalyzePascal(fullPath, code, csFile);
            }
            return nonPascalNames;
        }

        void AnalyzePascal(string fullPath, string code, string csFile)
        {
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

                foreach (var member in classDeclaration.Members)
                {
                    if (member is MethodDeclarationSyntax method)
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
                    else if (member is PropertyDeclarationSyntax property)
                    {
                        var propertyName = property.Identifier.ValueText;
                        if (!IsPascalCase(propertyName))
                        {
                            nonPascalNames.Add($"Pascal Rule{Environment.NewLine}" +
                                $"property name: {propertyName}{Environment.NewLine}" +
                                $"class name: {className}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"line number: {property.GetLocation().GetLineSpan().StartLinePosition.Line + 1}{Environment.NewLine}" +
                                $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}");
                        }
                    }           
                }
            }
        }
        static bool IsPascalCase(string s)
        {
            return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
        }
    }

    class Pascal
    {

    }
}
