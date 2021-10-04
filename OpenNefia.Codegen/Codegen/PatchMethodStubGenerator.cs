using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace OpenNefia.Codegen
{
    [Generator]
    public class PatchMethodStubGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new BaseUiLayerSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is BaseUiLayerSyntaxReceiver receiver))
                return;

            // group the fields by class, and generate the source
            foreach (var klass in receiver.Classes)
            {
                //string classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, notifySymbol, context);
                //context.AddSource($"{group.Key.Name}_autoNotify.cs", SourceText.From(classSource, Encoding.UTF8));
                Console.WriteLine($"{klass.Identifier.ToFullString()}");
            }
        }

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        class BaseUiLayerSyntaxReceiver : ISyntaxContextReceiver
        {
            public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // any field with at least one attribute is a candidate for property generation
                if (context.Node is ClassDeclarationSyntax classDeclarationSyntax
                    && (classDeclarationSyntax.BaseList?.Types.Any(IsBaseUILayer) ?? false))
                {
                    Classes.Add(classDeclarationSyntax);
                }
            }

            private bool IsBaseUILayer(BaseTypeSyntax arg)
            {
                return arg.Type.ToString().StartsWith("BaseUiLayer");
            }
        }
    }
}
