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

                checkPascalRule(methods, csFile);
            }

            return nonPascalMethods;
        }

        private void checkPascalRule(IEnumerable<MethodDeclarationSyntax>  methods, string csFile)
        {
            foreach (var method in methods)
            {
                var methodName = method.Identifier.ValueText;
                if (!IsPascalCase(methodName))
                {
                    nonPascalMethods.Add($"Method '{methodName}' in file '{csFile}' does not follow PascalCase");
                }
            }
        }

        static bool IsPascalCase(string s)
        {
            return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
        }

        void fdfdfae()
        {
            // 테스트입니다. josephine 0823
        }
    }
}
