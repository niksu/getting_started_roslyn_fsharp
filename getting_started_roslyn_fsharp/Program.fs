(*
Example of using "Roslyn" in F#
Nik Sultana, Cambridge University Computer Lab, January 2017

This code is a F# port of Gabriele Tomassetti's C# example code at https://github.com/unosviluppatore/getting-started-roslyn

Use of this source code is governed by the MIT license; see the file called LICENSE
*)

module Program

open System
open System.Collections.Generic
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open System.Reflection
open System.IO

open InitializerRewriter

let CreateTestCompilation() : Compilation =
  // creation of the syntax tree for every file
  let programPath : String = @"Program.cs"
  let programText : String = File.ReadAllText(programPath)
  let programTree : SyntaxTree =
    CSharpSyntaxTree.ParseText(programText).WithFilePath(programPath)

  let rewriterPath : String = @"InitializerRewriter.cs"
  let rewriterText : String = File.ReadAllText(rewriterPath)
  let rewriterTree : SyntaxTree =
    CSharpSyntaxTree.ParseText(rewriterText).WithFilePath(rewriterPath)

  let sourceTrees : SyntaxTree list = [programTree; rewriterTree];

  // gathering the assemblies
  let mscorlib : MetadataReference =
    MetadataReference.CreateFromFile(typedefof<obj>.GetTypeInfo().Assembly.Location) :> MetadataReference
  let codeAnalysis : MetadataReference =
    MetadataReference.CreateFromFile(typedefof<SyntaxTree>.GetTypeInfo().Assembly.Location) :> MetadataReference
  let csharpCodeAnalysis : MetadataReference =
    MetadataReference.CreateFromFile(typedefof<CSharpSyntaxTree>.GetTypeInfo().Assembly.Location) :> MetadataReference

  let references : MetadataReference list = [mscorlib; codeAnalysis; csharpCodeAnalysis]

  // compilation
  CSharpCompilation.Create("ConsoleApplication", sourceTrees, references,
    new CSharpCompilationOptions(OutputKind.ConsoleApplication)) :> Compilation

[<EntryPoint>]
let main argv =
  let test : Compilation = CreateTestCompilation()

  Seq.iter (fun (sourceTree : SyntaxTree) ->
    // creation of the semantic model
    let model : SemanticModel = test.GetSemanticModel(sourceTree)

    // initialization of our rewriter class
    let rewriter : InitializerRewriter = new InitializerRewriter(model)

    // analysis of the tree
    let newSource : SyntaxNode = rewriter.Visit(sourceTree.GetRoot())

    if (not(Directory.Exists(@"../new_src"))) then
      ignore(Directory.CreateDirectory(@"../new_src"))
    else ()

    // if we changed the tree we save a new file
    if (newSource <> sourceTree.GetRoot()) then
      File.WriteAllText(Path.Combine(@"../new_src", Path.GetFileName(sourceTree.FilePath)), newSource.ToFullString())
    else ()
    ) test.SyntaxTrees

  0 // return an integer exit code