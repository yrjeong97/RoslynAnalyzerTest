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

        public List<string> AnalyzeNamingRule()
        {
            foreach (var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    var className = classDeclaration.Identifier.ValueText;
                    if (!IsPascalCase(className))
                    {
                        int lineNum = classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                        nonPascalNames.Add(WriteNonePascalClass(className, csFile, lineNum));
                    }

                    foreach (var member in classDeclaration.Members)
                    {
                        if (member is MethodDeclarationSyntax method)
                        {
                            var methodName = method.Identifier.ValueText;
                            if (!IsPascalCase(methodName))
                            {
                                int lineNum = method.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                                nonPascalNames.Add(WriteNonePascalMethod(methodName, className, csFile, lineNum));
                            }
                            AnalyzeParameterCamel(method, csFile, className);
                        }
                        else if (member is PropertyDeclarationSyntax property)
                        {
                            var propertyName = property.Identifier.ValueText;
                            if (!IsPascalCase(propertyName))
                            {
                                int lineNum = property.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                                nonPascalNames.Add(WriteNonePascalProperty(propertyName, className, csFile, lineNum));
                            }
                        }
                        else if (member is FieldDeclarationSyntax field)
                        {
                            AnalyzerVariableCamel(field, csFile, className);
                        }
                    }
                }
                foreach (var constDeclaration in root.DescendantNodes().OfType<FieldDeclarationSyntax>())
                {
                    if (constDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword))
                    {
                        foreach (var variable in constDeclaration.Declaration.Variables)
                        {
                            var constantName = variable.Identifier.ValueText;
                            if (!IsUpperSnakeCase(constantName))
                            {
                                int lineNum = constDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                                nonPascalNames.Add(WriteNoneUpperSnakeCaseConstant(constantName, csFile, lineNum));
                            }
                        }
                    }
                }
            }            
            return nonPascalNames;
        }

        void AnalyzeParameterCamel(MethodDeclarationSyntax method, string csFile, string className)
        {
            foreach (var parameter in method.ParameterList.Parameters)
            {
                var parameterName = parameter.Identifier.ValueText;
                if (!IsCamelCase(parameterName))
                {
                    int lineNum = parameter.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    nonPascalNames.Add(WriteNoneCamelParameter(parameterName, method.Identifier.ValueText, className, csFile, lineNum));
                }
            }
        }

        void AnalyzerVariableCamel(FieldDeclarationSyntax field, string csFile, string className)
        {
            foreach (var variable in field.Declaration.Variables)
            {
                var fieldName = variable.Identifier.ValueText;
                if (!IsCamelCase(fieldName))
                {
                    int lineNum = field.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    nonPascalNames.Add(WriteNoneCamelVariable(fieldName, className, csFile,lineNum));
                }
            }
        }

        static bool IsPascalCase(string s)
        {
            return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
        }

        bool IsCamelCase(string s)
        {
            return !string.IsNullOrEmpty(s) && char.IsLower(s[0]) && s.All(char.IsLetterOrDigit);
        }

        bool IsUpperSnakeCase(string s)
        {
            return !string.IsNullOrEmpty(s) && s.All(char.IsUpper) && s.All(c => c == '_' || char.IsLetterOrDigit(c));
        }

        string WriteNonePascalClass(string className, string csFile, int lineNum )
        {
            string NonePascalClass = $"Pascal Rule{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NonePascalClass;
        }

        string WriteNonePascalMethod(string methodName, string className, string csFile, int lineNum)
        {

            string NonePascalMethod = $"Pascal Rule{Environment.NewLine}" +
                                $"method name: {methodName}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"class name: {className}{Environment.NewLine}" +
                                $"line number: {lineNum}{Environment.NewLine}" +
                                $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NonePascalMethod;
        }

        string WriteNonePascalProperty(string propertyName, string className, string csFile, int lineNum)
        {

            string NonePascalProperty = $"Pascal Rule{Environment.NewLine}" +
                                $"property name: {propertyName}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"class name: {className}{Environment.NewLine}" +
                                $"line number: {lineNum}{Environment.NewLine}" +
                                $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NonePascalProperty;
        }

        string WriteNoneCamelParameter(string parameterName, string method, string className, string csFile, int lineNum)
        {
            string NoneCamelParameter = $"Camel Rule{Environment.NewLine}" +
                        $"parameter name: {parameterName}{Environment.NewLine}" +
                        $"method name: {method}{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NoneCamelParameter;
        }

        string WriteNoneCamelVariable(string fieldName, string className, string csFile, int lineNum)
        {
            string NoneCamelVariable = $"Camel Rule{Environment.NewLine}" +
                        $"field name: {fieldName}{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NoneCamelVariable;
        }

        string WriteNoneUpperSnakeCaseConstant(string constantName, string csFile, int lineNum)
        {
            string NoneUpperSnakeCaseConstant = $"UpperSnakeCase Rule{Environment.NewLine}" +
                        $"constant name: {constantName}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NoneUpperSnakeCaseConstant;
        }
    }
}
