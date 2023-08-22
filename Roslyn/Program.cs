using Octokit;
using LibGit2Sharp;
using System;
using System.IO;
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
        var repoEmail = "yrjeong@ati2000.co.kr";

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

        var reportContent = nonPascalMethods.Any()
            ? string.Join(Environment.NewLine, nonPascalMethods)
            : "All methods follow PascalCase.";

        // Save the report to a text file
        var reportFilePath = "non_pascal_methods.txt";
        File.WriteAllText(reportFilePath, reportContent);

        // Commit and push the report file to the repository
        var repoPath = Path.Combine(Environment.GetEnvironmentVariable("GITHUB_WORKSPACE"), repoName);
        using (var repo = new LibGit2Sharp.Repository(repoPath))
        {
            Commands.Stage(repo, reportFilePath);
            var sig = new LibGit2Sharp.Signature(repoOwner, repoEmail, DateTimeOffset.Now);
            var commitMessage = "update text file";
            repo.Commit(commitMessage, sig, sig);
            var remote = repo.Network.Remotes["origin"];
            var pushRefSpec = $"refs/heads/main:refs/heads/main";
            repo.Network.Push(remote, pushRefSpec);
        }     

        Console.WriteLine("Non-PascalCase methods report saved and committed.");
    }

    static bool IsPascalCase(string s)
    {
        return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
    }
}
