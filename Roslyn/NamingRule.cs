using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn
{  

    class NamingRule
    {
        string[] csFiles;
        List<string> nonPascalMethods;

        public NamingRule(string[] csFiles)
        {
            this.csFiles = csFiles;
            this.nonPascalMethods = new List<string>();
        }

        public List<string> AnalyeNamingRule()
        {
            foreach (var csFile in csFiles)
            {
                var code = File.ReadAllText(csFile);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();
                var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

                CheckPascalRule(methods, csFile);
            }

            return nonPascalMethods;
        }

        private void CheckPascalRule(IEnumerable<MethodDeclarationSyntax>  methods, string csFile)
        {
            foreach (var method in methods)
            {
                var methodName = method.Identifier.ValueText;
                if (!isPascalCase(methodName))
                {
                    nonPascalMethods.Add($"Method '{methodName}' in file '{csFile}' does not follow PascalCase");
                }
            }
        }

        static bool isPascalCase(string s)
        {
            return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
        }

        void dfdf()
        {

        }
    }
}
