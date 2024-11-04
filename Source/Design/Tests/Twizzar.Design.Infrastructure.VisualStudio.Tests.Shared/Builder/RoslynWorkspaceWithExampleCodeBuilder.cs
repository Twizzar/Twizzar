using System.IO;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;

public class RoslynWorkspaceWithExampleCodeBuilder : RoslynWorkspaceBuilder
{
    #region static fields and constants

    public const string ExampleCodeFilePath = "Roslyn/RoslynDocumentReaderExampleCode.cs";

    #endregion

    #region ctors

    public RoslynWorkspaceWithExampleCodeBuilder()
    {
        var sourceText = File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(RoslynWorkspaceWithExampleCodeBuilder).Assembly.Location), ExampleCodeFilePath));
        this.AddDocument(ExampleCodeFilePath, sourceText);
    }

    #endregion
}