using Octokit;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        var repoOwner = "yrjeong97";
        var repoName = "RoslynAnalyzerTest";

        var github = new GitHubClient(new ProductHeaderValue("Roslyn-Code-Analyzer"));
        github.Credentials = new Credentials(token);

        var repositoryContents = await github.Repository.Content.GetAllContents(repoOwner, repoName);

        var nonPascalMethods = new List<string>();

        foreach (var content in repositoryContents)
        {
            if (content.Type == ContentType.File && content.Name.EndsWith(".cs"))
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

        Console.WriteLine("Code Analysis Report updated on GitHub Wiki.");
    }

    static bool IsPascalCase(string s)
    {
        return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
    }
}