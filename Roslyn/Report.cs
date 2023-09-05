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
            string nonePascalClass = $"Pascal Rule{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return nonePascalClass;
        }

        static public string WriteNonePascalMethod(string methodName, string className, string csFile, int lineNum)
        {

            string nonePascalMethod = $"Pascal Rule{Environment.NewLine}" +
                                $"method name: {methodName}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"class name: {className}{Environment.NewLine}" +
                                $"line number: {lineNum}{Environment.NewLine}" +
                                $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return nonePascalMethod;
        }

        static public string WriteNonePascalProperty(string propertyName, string className, string csFile, int lineNum)
        {

            string nonePascalProperty = $"Pascal Rule{Environment.NewLine}" +
                                $"property name: {propertyName}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"class name: {className}{Environment.NewLine}" +
                                $"line number: {lineNum}{Environment.NewLine}" +
                                $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return nonePascalProperty;
        }

        static public string WriteNoneCamelParameter(string parameterName, string method, string className, string csFile, int lineNum)
        {
            string noneCamelParameter = $"Camel Rule{Environment.NewLine}" +
                        $"parameter name: {parameterName}{Environment.NewLine}" +
                        $"method name: {method}{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneCamelParameter;
        }

        static public string WriteNoneCamelVariable(string fieldName, string className, string csFile, int lineNum)
        {
            string noneCamelVariable = $"Camel Rule{Environment.NewLine}" +
                        $"field name: {fieldName}{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneCamelVariable;
        }

        static public string WriteNoneUpperSnakeCaseConstant(string constantName, string csFile, int lineNum)
        {
            string noneUpperSnakeCaseConstant = $"UpperSnakeCase Rule{Environment.NewLine}" +
                        $"constant name: {constantName}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneUpperSnakeCaseConstant;
        }

        static public string WriteNoneValidInterfaceName(string interfaceName, string csFile, int lineNum)
        {
            string noneValidInterfaceName = $"Interface Name Rule{Environment.NewLine}" +
                        $"interface name: {interfaceName}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneValidInterfaceName;
        }

        static public string WriteStringConcatenationIssue(string csFile, int lineNum)
        {
            string stringConcatenationIssue = $"String Concatenation Rule{Environment.NewLine}" +
                       $"File name: {csFile}{Environment.NewLine}" +
                       $"line number: {lineNum}{Environment.NewLine}" +
                       $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return stringConcatenationIssue;       
        }

        static public string WriteMemberOrderIssue(string csFile, string className)
        {
            string unorderedMember = $"Unordered Member Rule{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return unorderedMember;
        }

        static public string WriteUnusedVariable(string className, string csFile, int lineNum)
        {
            string unusedVariable = $"Pascal Rule{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"line number: {lineNum}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return unusedVariable;
        }
    }
}
