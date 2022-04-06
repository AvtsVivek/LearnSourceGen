using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        private const string _logAttributeName = "LogAttribute";
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            var logSrc = @"
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
[GeneratedCode(""LogAttribute"", ""x.x.x"")] // Check the namespace and version
[CompilerGenerated]
";
            logSrc = logSrc + "public class " + _logAttributeName + " : System.Attribute { }";
            context.AddSource("Log.cs", logSrc);

            var options = (CSharpParseOptions)compilation.SyntaxTrees.First().Options;
            var logSyntaxTree = CSharpSyntaxTree.ParseText(logSrc, options);
            compilation = compilation.AddSyntaxTrees(logSyntaxTree);

            var logAttribute = compilation.GetTypeByMetadataName(_logAttributeName);

            var sb = new StringBuilder();

            // retreive the populated receiver 
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;


            // loop over the candidate fields, and keep the ones that are actually annotated
            var classSymbols = new List<ITypeSymbol>();
            foreach (var decl in receiver.ClassDeclarations)
            {
                var model = compilation.GetSemanticModel(decl.SyntaxTree);
                if (model.GetDeclaredSymbol(decl, context.CancellationToken) is ITypeSymbol symbol)
                {
                    classSymbols.Add(symbol);
                }
            }

            var interfaceSymbols = new List<ITypeSymbol>();
            foreach (var decl in receiver.InterfaceDeclarations)
            {
                var model = compilation.GetSemanticModel(decl.SyntaxTree);
                if (model.GetDeclaredSymbol(decl, context.CancellationToken) is ITypeSymbol symbol)
                {
                    interfaceSymbols.Add(symbol);
                }
            }

            var classesThatImplimentInterfaces = new List<ITypeSymbol>();
            foreach (var decl in receiver.ClassDeclarations)
            {
                var semanticModel = compilation.GetSemanticModel(decl.SyntaxTree);
                
                var declaredSymbol = semanticModel.GetDeclaredSymbol(decl, context.CancellationToken);

                if (declaredSymbol is INamedTypeSymbol namedTypeSymbol)
                {
                    if(namedTypeSymbol.AllInterfaces.Count() > 0)
                    classesThatImplimentInterfaces.Add(namedTypeSymbol);
                }
            }

            var classesThatImplimentInterfacesWithAttributes = new List<ITypeSymbol>();

            foreach (var decl in receiver.ClassDeclarations)
            {
                var semanticModel = compilation.GetSemanticModel(decl.SyntaxTree);

                var declaredSymbol = semanticModel.GetDeclaredSymbol(decl, context.CancellationToken);

                var hasLogAttribute = declaredSymbol.GetAttributes()
                    .Any(x => x.AttributeClass.Equals(logAttribute));

                if (!hasLogAttribute)
                    continue;                

                if (declaredSymbol is INamedTypeSymbol namedTypeSymbol)
                {
                    if (hasLogAttribute)
                    {
                        var attributeData = declaredSymbol.GetAttributes().FirstOrDefault();
                        sb.AppendLine("//  " + declaredSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + " has LOG attribute applied to it");
                        sb.AppendLine("// The attribute data is FullyQualifiedFormat " + attributeData.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                        sb.AppendLine("// The attribute data is Minimal " + attributeData.AttributeClass.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                        //sb.AppendLine("// " + logAttribute.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                    }
                    if (!hasLogAttribute)
                        sb.AppendLine("//  " + declaredSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + " has NO LOG attribute applied to it");

                    if (namedTypeSymbol.AllInterfaces.Count() > 0)
                    {
                        classesThatImplimentInterfacesWithAttributes.Add(namedTypeSymbol);
                    }
                }
            }

            WriteSymbols(classSymbols, sb);

            if (interfaceSymbols.Count > 0)
            {
                sb.AppendLine(" ");
                sb.AppendLine("// Interfaces symbols found as follows.");
            }

            foreach (var symbol in interfaceSymbols)
            {
                sb.AppendLine("// " + symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                sb.AppendLine("// " + symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }

            WriteSymbols(interfaceSymbols, sb);

            if (classesThatImplimentInterfaces.Count > 0)
            {
                sb.AppendLine("");
                sb.AppendLine("// Class that impliment interfaces are as follows.");
            }

            WriteSymbols(classesThatImplimentInterfaces, sb);

            if (classesThatImplimentInterfacesWithAttributes.Count > 0)
            {
                sb.AppendLine("");
                sb.AppendLine("// Class that impliment interfaces AND have Log attribute are as follows.");
            }
            else
            {
                sb.AppendLine("");
                sb.AppendLine("// There are no classes that impliment the interface and are decorated with Log attribute.");
            }

            WriteSymbols(classesThatImplimentInterfacesWithAttributes, sb);

            context.AddSource($"all_types.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private void WriteSymbols(List<ITypeSymbol> typeSymbols, StringBuilder sb)
        {
            foreach (var symbol in typeSymbols)
            {
                sb.AppendLine("// " + symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                sb.AppendLine("// " + symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }
        }
    }
}
