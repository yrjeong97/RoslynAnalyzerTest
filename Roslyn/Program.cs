using Octokit;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program
{
    static async Task Main(string[] args)
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
            if (content.Type == ContentType.File && content.Name.EndsWith(".cs")) // 현재 예시에서 C# 파일만 분석
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

            // Commit the report file to the repository
            var commitMessage = "Add non-PascalCase methods report";
            await github.Repository.Content.CreateFile(repoOwner, repoName, reportFilePath, new CreateFileRequest(commitMessage, reportContent));

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


