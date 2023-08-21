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
        var token = Environment.GetEnvironmentVariable("TOKEN");
        var repoOwner = "yrjeong97";
        var repoName = "RsolynAnalyzerTest";

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

        //var reportContent = nonPascalMethods.Any()
        //    ? string.Join(Environment.NewLine, nonPascalMethods)
        //    : "All methods follow PascalCase.";

        //// Post the report to GitHub Wiki
        //var wikiPageName = "CodeAnalysisReport";
        //var update = new NewWikiPageUpdate(reportContent, "Update Code Analysis Report");
        //await github.Repository.Wiki.UpdatePage(repoOwner, repoName, wikiPageName, update);

        Console.WriteLine("Code Analysis Report updated on GitHub Wiki.");
    }

    static bool IsPascalCase(string s)
    {
        return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
    }
}
