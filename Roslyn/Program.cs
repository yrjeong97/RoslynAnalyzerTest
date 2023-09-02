using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using Roslyn;

class Program
{
    static void Main(string[] args)
    {
        var reportFilePath = Environment.GetEnvironmentVariable("REPORT_FILE_PATH");
        var changedFiles = Environment.GetEnvironmentVariable("CHANGED_FILES");

        if (string.IsNullOrEmpty(reportFilePath))
        {
            Console.WriteLine("Report file path not specified.");
            return;
        }
        var projectPath = Environment.GetEnvironmentVariable("PROJECT_PATH");
        //var nonPascalNames = new List<string>();

        var csFilesList = changedFiles.Split(';'); 
        csFilesList = csFilesList.Skip(1).ToArray(); ;

        NamingRule namingRule = new NamingRule(csFilesList, projectPath, reportFilePath);
        List<string> nonPascalNames = namingRule.AnalyzeNamingRule();
        WriteResult(nonPascalNames, reportFilePath);

    }

    static void WriteResult(List<string> reportList, string reportFilePath)
    {
        if (reportList.Any())
        {
            var reportContent = string.Join(Environment.NewLine, reportList);

            File.WriteAllText(reportFilePath, reportContent);
        }
        else
        {
            Console.WriteLine("All names follow PascalCase.");
        }
    }

    int Value;

    void TESTMethod(string S)
    {

    }
    
    string dddd
    {
        get;
        set;
    }
}