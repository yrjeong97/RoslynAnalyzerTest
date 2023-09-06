using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Roslyn;

class Program
{
    static void Main(string[] args)
    {
        var reportFilePath = Environment.GetEnvironmentVariable("REPORT_FILE_PATH");
        var files = Environment.GetEnvironmentVariable("FILES");
        var keywords = Environment.GetEnvironmentVariable("KEYWORDS");

        if (string.IsNullOrEmpty(reportFilePath))
        {
            Console.WriteLine("Report file path not specified.");
            return;
        }
        var projectPath = Environment.GetEnvironmentVariable("PROJECT_PATH");

        var csFilesList = files.Split(';').Skip(1).ToArray();

        var ruleViolation = new List<string>();

        if (keywords == "")
        {
            keywords = "All;";
        }

        var keywordList = keywords.Split(';');
        if (keywordList.Contains("All"))
        {
            var namingRule = new NamingRule(csFilesList, projectPath);
            ruleViolation.AddRange(namingRule.AnalyzeNamingRule());

            var codingStyle = new CodingStyle(csFilesList, projectPath);
            ruleViolation.AddRange(codingStyle.AnalyzeCodingStyle());

            var wrongCode = new WrongCode(csFilesList, projectPath);
            ruleViolation.AddRange(wrongCode.AnalyzeWrongCode());
        }
        else
        {
            foreach (var keyword in keywordList)
            {
                switch (keyword)
                {
                    case "NamingRule":
                        var namingRule = new NamingRule(csFilesList, projectPath);
                        ruleViolation.AddRange(namingRule.AnalyzeNamingRule());
                        break;
                    case "CodingStyle":
                        var codingStyle = new CodingStyle(csFilesList, projectPath);
                        ruleViolation.AddRange(codingStyle.AnalyzeCodingStyle());
                        break;
                    case "WrongCode":
                        var wrongCode = new WrongCode(csFilesList, projectPath);
                        ruleViolation.AddRange(wrongCode.AnalyzeWrongCode());
                        break;
                }
            }
        }

        WriteResult(ruleViolation, reportFilePath);
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