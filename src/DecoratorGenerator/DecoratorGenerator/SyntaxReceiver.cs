using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LoggingGenerator
{
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public HashSet<TypeDeclarationSyntax> TypeDeclarationsWithAttributes { get; } = new();
        public HashSet<InterfaceDeclarationSyntax> InterfaceDeclarations { get; } = new();
        public HashSet<ClassDeclarationSyntax> ClassDeclarations { get; } = new();
        public HashSet<ClassDeclarationSyntax> ClassThatImplimentsInterface { get; } = new();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclaration)
            {
                InterfaceDeclarations.Add(interfaceDeclaration);
            }

            if (syntaxNode is TypeDeclarationSyntax declaration
                //&& declaration.AttributeLists.Any()
                )
            {
                TypeDeclarationsWithAttributes.Add(declaration);
            }

            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            {
                ClassDeclarations.Add(classDeclaration);
                //if (HasInterface(classDeclaration))
                //{ 
                //    ClassThatImplimentsInterface.Add(classDeclaration);
                //}
            }



        }

        /// <summary>Indicates whether or not the class has a specific interface.</summary>
        /// <returns>Whether or not the SyntaxList contains the attribute.</returns>
        public bool HasInterface(ClassDeclarationSyntax source
            //, string interfaceName
            )
        {
            var interfaceName = "ISomeInterface";

            IEnumerable<BaseTypeSyntax> baseTypes = source.BaseList.Types.Select(baseType => baseType);

            // To Do - cleaner interface finding.
            return baseTypes.Any(baseType => baseType.ToString() == interfaceName);
        }

    }
}
