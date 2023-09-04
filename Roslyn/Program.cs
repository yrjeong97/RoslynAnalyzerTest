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

        var csFilesList = changedFiles.Split(';'); 
        csFilesList = csFilesList.Skip(1).ToArray(); ;

        NamingRule namingRule = new NamingRule(csFilesList, projectPath, reportFilePath);
        CodingStyle codingStyle = new CodingStyle(csFilesList, projectPath, reportFilePath);
        List<string> ruleViolation = namingRule.AnalyzeNamingRule();
        List<string> codingStyleViolation = codingStyle.AnalyzeCodingStyle();

        ruleViolation.AddRange(codingStyleViolation);
        WriteResult(ruleViolation, reportFilePath);

        int num = 21;
        string str;
        str = "strn";
        string sss = num + str;

        string d = "ab";
        string b = "bb";
        string rr = d + b;

        string a;
        string c;
        a = "aa";
        c = "cc";
        string r2 = a + c;

        int number1;
        int number2;
        number1 = 1;
        number2 = 2;
        int nubmers = number1 + number2;
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
            Console.WriteLine("Everything follows the coding rule.");
        }
    }

}