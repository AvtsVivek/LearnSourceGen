using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LoggingGenerator
{
    [Generator]
    public class TrialGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (SyntaxReceiver)context.SyntaxReceiver;
            var typeTargets = syntaxReceiver.TypeDeclarationsWithAttributes;
            var interfaceTargets = syntaxReceiver.InterfaceDeclarations;
            var classTargets = syntaxReceiver.ClassDeclarations;
            var classThatImplimentInterfaces = syntaxReceiver.ClassThatImplimentsInterface;

            var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
            //var namespaceName = mainMethod.ContainingNamespace.ToDisplayString();

            var compilation = context.Compilation;



            var i = 0;
            var finalSource = string.Empty;
            finalSource = $@" // Auto-generated code
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;            
using System;
namespace {mainMethod.ContainingNamespace.ToDisplayString()}
{{";

            var source = string.Empty;
            foreach (var targetTypeSyntax in classTargets)
            {

                var semanticModel = compilation.GetSemanticModel(targetTypeSyntax.SyntaxTree);
                var targetType = semanticModel.GetDeclaredSymbol(targetTypeSyntax);
                

                i++;

                source = $@"
    [GeneratedCode(""{mainMethod.ContainingNamespace.ToDisplayString()}.A{i}"", ""x.x.x"")] // Check namespace and version info
    [CompilerGenerated]
    public class A{i}
    {{
/*
{targetType} 
The number of interfaces this type impliments is 
{targetType.AllInterfaces.Count()}
*/
    }}
";
                finalSource = finalSource + source;
            }
            finalSource = finalSource + $@"
}}";

            context.AddSource($"A.g.cs", finalSource);
        }


    }
}
