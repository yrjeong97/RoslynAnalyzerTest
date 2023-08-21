using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program
{
    static async Task Main(string[] args)
    {
        var repoOwner = "yrjeong97";
        var repoName = "RoslynAnalyzerTest";

        var github = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Roslyn-Code-Analyzer"));
        github.Credentials = new Octokit.Credentials(Environment.GetEnvironmentVariable("TOKEN"));

        var repositoryContents = await github.Repository.Content.GetAllContents(repoOwner, repoName);

        var nonPascalMethods = new System.Collections.Generic.List<string>();

        foreach (var content in repositoryContents)
        {
            if (content.Type == Octokit.ContentType.File && content.Path.StartsWith("Calc/") && content.Path.EndsWith(".cs"))
            {
                var code = content.Content;
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();
                var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

                foreach (var method in methods)
                {
                    var methodName = method.Identifier.ValueText;
                    if (!IsPascalCase(methodName))
                    {
                        nonPascalMethods.Add($"Method '{methodName}' in file '{content.Name}' does not follow PascalCase");
                    }
                }
            }
        }

        if (nonPascalMethods.Any())
        {
            var reportContent = string.Join(Environment.NewLine, nonPascalMethods);
            var reportFilePath = "non_pascal_methods.txt";

            File.WriteAllText(reportFilePath, reportContent);

            // You can perform a Git commit and push here using Octokit or other Git library

            Console.WriteLine("Non-PascalCase methods report saved and committed.");
        }
        else
        {
            Console.WriteLine("All methods follow PascalCase.");
        }
    }

    static bool IsPascalCase(string s)
    {
        return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
    }
}
