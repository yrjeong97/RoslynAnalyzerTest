//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace Roslyn
//{
//    public class Member
//    {
//        public string MemberName { get; }
//        public string MemberType { get; }
//        public int LineNumber { get; }

//        public Member(string memberName, string memberType, int lineNumber)
//        {
//            MemberName = memberName;
//            MemberType = memberType;
//            LineNumber = lineNumber;
//        }
//    }

//    public class MemberVariablesOrder : WriteNamingRuleReport
//    {
//        private readonly string[] csFilesList;
//        private readonly string projectPath;
//        List<string> unorderedMemberVariable;

//        public MemberVariablesOrder(string[] csFilesList, string projectPath)
//        {
//            this.csFilesList = csFilesList;
//            this.projectPath = projectPath;
//            unorderedMemberVariable = new List<string>();
//        }

//        public List<string> AnalyzeMemberOrder()
//        {
//            foreach (var csFile in csFilesList)
//            {
//                var fullPath = Path.Combine(projectPath, csFile);
//                var code = File.ReadAllText(fullPath);
//                var syntaxTree = CSharpSyntaxTree.ParseText(code);
//                var root = syntaxTree.GetRoot();
//                var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

//                foreach (var classDeclaration in classDeclarations)
//                {
//                    var members = classDeclaration.Members;

//                    Dictionary<string, int> memberTypeIndexMap = new Dictionary<string, int>
//                    {
//                        {"Constant", 0},
//                        {"Field", 1},
//                        {"Property", 2},
//                        {"Constructor", 3},
//                        {"Method", 4},
//                        {"PrivateMethod", 5},
//                        {"NestedClass", 6}
//                    };

//                    // 클래스 내의 멤버 변수 목록 초기화
//                    var memberList = CreateMemberList(members, csFile);

//                    // 순서 어긋난 멤버 변수 찾기
//                    var unorderedMembers = GetUnorderedMembers(memberList, memberTypeIndexMap);

//                    if (unorderedMembers.Count() > 0)
//                    {
//                        var className = classDeclaration.Identifier.ValueText;

//                        unorderedMemberVariable.Add(WriteMemberOrderIssue(csFile, className));
//                    }
//                }
//            }
//            return unorderedMemberVariable;
//        }

//        private List<Member> CreateMemberList(SyntaxList<MemberDeclarationSyntax> members, string csFile)
//        {
//            // 클래스 내의 멤버 변수 목록 초기화
//            var memberList = new List<Member>();
//            var fileName = csFile;
//            var projectName = Path.GetFileNameWithoutExtension(csFile);

//            // 멤버 변수 순서를 검사하고 memberList에 추가
//            foreach (var member in members)
//            {
//                if (member is FieldDeclarationSyntax fieldDeclaration)
//                {
//                    // 변수 타입을 얻어옵니다.
//                    var memberName = fieldDeclaration.Declaration.Variables.First().Identifier.Text;
//                    var lineNum = fieldDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
//                    memberList.Add(new Member(memberName, "Constant", lineNum));
//                }
//                else if (member is PropertyDeclarationSyntax propertyDeclaration)
//                {
//                    var propertyName = propertyDeclaration.Identifier.Text;
//                    var lineNum = propertyDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

//                    memberList.Add(new Member(propertyName, "Property", lineNum));
//                }
//                else if (member is ConstructorDeclarationSyntax constructorDeclaration)
//                {
//                    var constructorName = constructorDeclaration.Identifier.Text;
//                    var lineNum = constructorDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

//                    memberList.Add(new Member(constructorName, "Constructor", lineNum));
//                }
//                else if (member is MethodDeclarationSyntax methodDeclaration)
//                {
//                    var methodName = methodDeclaration.Identifier.Text;
//                    var methodType = methodDeclaration.ReturnType.ToString();
//                    var lineNum = methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

//                    if (methodDeclaration.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.PublicKeyword))
//                    {
//                        memberList.Add(new Member(methodName, "Method", lineNum));
//                    }
//                    else
//                    {
//                        memberList.Add(new Member(methodName, "PrivateMethod", lineNum));
//                    }
//                }
//                else if (member is ClassDeclarationSyntax nestedClassDeclaration)
//                {
//                    var nestedClassName = nestedClassDeclaration.Identifier.Text;
//                    var lineNum = nestedClassDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

//                    memberList.Add(new Member(nestedClassName, "NestedClass", lineNum));
//                }
//                // 상수를 추가하려면 추가적인 논리 필요
//            }

//            return memberList;
//        }

//        private List<Member> GetUnorderedMembers(List<Member> members, Dictionary<string, int> memberTypeIndexMap)
//        {
//            var unorderedMembers = new List<Member>();
//            var currentIndex = 0;

//            foreach (var member in members)
//            {
//                if (currentIndex < memberTypeIndexMap[member.MemberType])
//                {
//                    currentIndex = memberTypeIndexMap[member.MemberType];
//                }
//                else if (currentIndex > memberTypeIndexMap[member.MemberType])
//                {
//                    unorderedMembers.Add(member);
//                }
//            }

//            return unorderedMembers;
//        }

//        private void ReportWarnings(string csFile, List<Member> unorderedMembers)
//        {
//            if (unorderedMembers.Count > 0)
//            {
//                // 경고 메시지 생성
//                var warnings = unorderedMembers.Select(member =>
//                    $"Warning in {csFile}: Member '{member.MemberName}' of type '{member.MemberType}' is out of order on line {member.LineNumber}").ToList();

//                // 경고 메시지를 txt 파일로 출력
//                var reportFileName = Path.Combine(reportFilePath, "warnings.txt");
//                File.AppendAllLines(reportFileName, warnings);
//            }
//        }
//    }
//}
