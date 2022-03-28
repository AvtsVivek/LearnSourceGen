using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator
{
    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
    class SyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ClassDeclarations { get; } = new List<ClassDeclarationSyntax>();
        public List<InterfaceDeclarationSyntax> InterfaceDeclarations { get; } = new List<InterfaceDeclarationSyntax>();
        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // any field with at least one attribute is a candidate for property generation
            if (syntaxNode is ClassDeclarationSyntax decl)
            {
                ClassDeclarations.Add(decl);
            }

            if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarations)
            {
                InterfaceDeclarations.Add(interfaceDeclarations);
            }
        }
    }
}
