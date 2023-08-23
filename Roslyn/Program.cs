using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var projectPath = Environment.GetEnvironmentVariable("PROJECT_PATH");
        var nonPascalNames = new List<string>();

        var csFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);

        foreach (var csFile in csFiles)
        {
            var code = File.ReadAllText(csFile);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();

            foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var className = classDeclaration.Identifier.ValueText;
                if (!IsPascalCase(className))
                {
                    nonPascalNames.Add($"Pascal Rule{Environment.NewLine}" +
                        $"class name: {className}{Environment.NewLine}" +
                        $"File name: line {csFile}{Environment.NewLine}" +
                        $"line number: {classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1}{Environment.NewLine}" +
                        $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}");
                }

                foreach (var method in classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    var methodName = method.Identifier.ValueText;
                    if (!IsPascalCase(methodName))
                    {
                        nonPascalNames.Add($"Pascal Rule{Environment.NewLine}" +
                            $"method name: {methodName}{Environment.NewLine}" +
                            $"class name: {className}{Environment.NewLine}" +
                            $"File name: {csFile}{Environment.NewLine}" +
                            $"line number: {method.GetLocation().GetLineSpan().StartLinePosition.Line + 1}{Environment.NewLine}" +
                            $"project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}");
                    }
                }
            }
        }

        if (nonPascalNames.Any())
        {
            var reportContent = string.Join(Environment.NewLine, nonPascalNames);
            var reportFilePath = "non_pascal_names.txt";

            File.WriteAllText(reportFilePath, reportContent);
            Console.WriteLine("Non-PascalCase names report saved.");

            RunGitCommands(csFiles, reportFilePath); //추가된 부분
        }
        else
        {
            Console.WriteLine("All names follow PascalCase.");
        }

        
    }
    static void RunGitCommands(string[] csFiles, string reportFilePath)
    {
        // Git 작업을 위한 커맨드 실행
        var gitAddCommand = $"git add {reportFilePath}";
        var gitCommitCommand = $"git commit -m \"Add non-PascalCase names report\"";
        var gitPushCommand = "git push origin main";

        foreach (var csFile in csFiles)
        {
            // Git 작업 수행
            RunShellCommand(gitAddCommand, csFile);
            RunShellCommand(gitCommitCommand, csFile);
            RunShellCommand(gitPushCommand, csFile);
        }
    }

    static void RunShellCommand(string command, string workingDirectory)
    {
        var processInfo = new ProcessStartInfo("bash", $"-c \"{command}\"")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process
        {
            StartInfo = processInfo
        };

        process.Start();
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.Dispose();

        if (!string.IsNullOrEmpty(output))
        {
            Console.WriteLine(output);
        }

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine(error);
        }
    }


    static bool IsPascalCase(string s)
    {
        return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
    }
}