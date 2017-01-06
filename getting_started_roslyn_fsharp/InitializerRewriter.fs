(*
Example of using "Roslyn" in F#
Nik Sultana, Cambridge University Computer Lab, January 2017

This code is a F# port of Gabriele Tomassetti's C# example code at https://github.com/unosviluppatore/getting-started-roslyn

Use of this source code is governed by the MIT license; see the file called LICENSE
*)

module InitializerRewriter

open System
open System.Collections.Generic
open System.Linq
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

type InitializerRewriter(semanticModel : SemanticModel) =
  inherit CSharpSyntaxRewriter()
  override this.VisitVariableDeclaration (node : VariableDeclarationSyntax) : SyntaxNode =
    // determination of the type of the variable(s)
    let typeSymbol = semanticModel.GetSymbolInfo(node.Type).Symbol :?> ITypeSymbol
    let changed : bool ref = ref false
    
    // you could declare more than one variable with one expression
    let vs : SeparatedSyntaxList<VariableDeclaratorSyntax> ref = ref node.Variables
    // we create a space to improve readability
    let space : SyntaxTrivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ")

    for i = 0 to (node.Variables.Count - 1) do
      // there is not an initialization
      if (semanticModel.GetSymbolInfo(node.Type).Symbol.ToString() = "int" &&
          node.Variables.[i].Initializer = null) then
        // we create a new espression "42"
        // preceded by the space we create earlier
        let es : ExpressionSyntax = SyntaxFactory.ParseExpression("42").WithLeadingTrivia(space)
        // basically we create an assignment to the espression we just created
        let evc : EqualsValueClauseSyntax = SyntaxFactory.EqualsValueClause(es).WithLeadingTrivia(space)

        // we replace the null initializer with ours
        vs := vs.Value.Replace(vs.Value.ElementAt(i), vs.Value.ElementAt(i).WithInitializer(evc));                                

        changed := true
      else ();

      // there is an initialization but it's not to 42
      if (semanticModel.GetSymbolInfo(node.Type).Symbol.ToString() = "int" &&
          node.Variables.[i].Initializer <> null &&
          not (node.Variables.[i].Initializer.Value.IsEquivalentTo(SyntaxFactory.ParseExpression("42")))) then
        let es : ExpressionSyntax = SyntaxFactory.ParseExpression("42").WithLeadingTrivia(space)
        let evc : EqualsValueClauseSyntax = SyntaxFactory.EqualsValueClause(es)
        vs := vs.Value.Replace(vs.Value.ElementAt(i), vs.Value.ElementAt(i).WithInitializer(evc))
        changed := true
      else ()

    if !changed then node.WithVariables(!vs) :> SyntaxNode
    else base.VisitVariableDeclaration(node)