using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OhioBox.Time.Analyzer
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SystemTimeUsageCodeFixProvider)), Shared]
	public class SystemTimeUsageCodeFixProvider : CodeFixProvider
	{
		private const string Title = "use SystemTime insted";

		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SystemTimeUsageDiagnosticAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root =
			  await context.Document.GetSyntaxRootAsync(context.CancellationToken)
			  .ConfigureAwait(false);

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var expressionSyntax =
			  root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
			  .OfType<MemberAccessExpressionSyntax>().First();

			context.RegisterCodeFix(
			  CodeAction.Create(Title, c =>
			  FixRegexAsync(context.Document, expressionSyntax, c), equivalenceKey:Title), diagnostic);
		}

		private async Task<Document> FixRegexAsync(Document document,
			MemberAccessExpressionSyntax expressionSyntax,
		  CancellationToken cancellationToken)
		{
			var semanticModel =
			  await document.GetSemanticModelAsync(cancellationToken);

			var identifier = (IdentifierNameSyntax)expressionSyntax.Expression;
			var systemTimeNameSyntax = SyntaxFactory.IdentifierName("SystemTime");

			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = root.ReplaceNode(identifier, systemTimeNameSyntax);

			newRoot = AddUsing(newRoot);

			var newDocument = document.WithSyntaxRoot(newRoot);
			return newDocument;
		}

		private static SyntaxNode AddUsing(SyntaxNode newRoot)
		{
			if (newRoot is CompilationUnitSyntax compilationUnitSyntax)
			{
				if (!compilationUnitSyntax.Usings.Any(use => use.Name.ToString() == "OhioBox.Time"))
				{
					var usingStatement = SyntaxFactory.UsingDirective(
											SyntaxFactory.QualifiedName(
												SyntaxFactory.IdentifierName("OhioBox"),
												SyntaxFactory.IdentifierName("Time")));

					newRoot = compilationUnitSyntax.AddUsings(usingStatement);
				}
			}

			return newRoot;
		}
	}
}