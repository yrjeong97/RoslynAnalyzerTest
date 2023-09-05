using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn
{
    class WrongCode
    {
        string[] csFilesList;
        string projectPath;
        List<string> wrongCode;

        public WrongCode(string[] csFilesList, string projectPath)
        {
            this.csFilesList = csFilesList;
            this.projectPath = projectPath;
            this.wrongCode = new List<string>();
        }

        public List<string> AnalyzeWrongCode()
        {
            foreach(var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                var declaredVariables = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(variable => variable.Identifier.Text);

                var usedVariables = root.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Select(identifier => identifier.Identifier.Text);

                var unusedVariables = declaredVariables.Except(usedVariables);

                foreach (var variable in unusedVariables)
                {
                    Console.WriteLine($"Unused variable: {variable}");
                }
            }

            return wrongCode;
        }
    }
}
