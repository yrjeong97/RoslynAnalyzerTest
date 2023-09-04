using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Roslyn
{
    public class WriteNamingRuleReport
    {
        static public string WriteNonePascalClass(string className, string csFile, int lineNum)
        {
            string NonePascalClass = $"Pascal Rule{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NonePascalClass;
        }

        static public string WriteNonePascalMethod(string methodName, string className, string csFile, int lineNum)
        {

            string NonePascalMethod = $"Pascal Rule{Environment.NewLine}" +
                                $"method name: {methodName}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"class name: {className}{Environment.NewLine}" +
                                $"line number: {lineNum}{Environment.NewLine}" +
                                $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NonePascalMethod;
        }

        static public string WriteNonePascalProperty(string propertyName, string className, string csFile, int lineNum)
        {

            string NonePascalProperty = $"Pascal Rule{Environment.NewLine}" +
                                $"property name: {propertyName}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"class name: {className}{Environment.NewLine}" +
                                $"line number: {lineNum}{Environment.NewLine}" +
                                $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NonePascalProperty;
        }

        static public string WriteNoneCamelParameter(string parameterName, string method, string className, string csFile, int lineNum)
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

        static public string WriteNoneCamelVariable(string fieldName, string className, string csFile, int lineNum)
        {
            string NoneCamelVariable = $"Camel Rule{Environment.NewLine}" +
                        $"field name: {fieldName}{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NoneCamelVariable;
        }

        static public string WriteNoneUpperSnakeCaseConstant(string constantName, string csFile, int lineNum)
        {
            string NoneUpperSnakeCaseConstant = $"UpperSnakeCase Rule{Environment.NewLine}" +
                        $"constant name: {constantName}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NoneUpperSnakeCaseConstant;
        }

        static public string WriteNoneValidInterfaceName(string interfaceName, string csFile, int lineNum)
        {
            string NoneValidInterfaceName = $"Interface Name Rule{Environment.NewLine}" +
                        $"interface name: {interfaceName}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return NoneValidInterfaceName;
        }

        static public string WriteStringConcatenationIssue(string csFile, int lineNum)
        {
            string StringConcatenationIssue = $"String Concatenation Rule{Environment.NewLine}" +
                       $"File name: {csFile}{Environment.NewLine}" +
                       $"line number: {lineNum}{Environment.NewLine}" +
                       $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return StringConcatenationIssue;
        }
    }
}
