using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;

public class RoslynWorkspaceBuilder
{
    #region static fields and constants

    public const string ProjectName = "TestProject";

    #endregion

    #region fields

    private readonly ProjectId _projectId;

    private readonly List<DocumentInfo> _documents = new List<DocumentInfo>();
    private readonly List<MetadataReference> _metaDataReferences = new List<MetadataReference>();
    private readonly List<ProjectReference> _projectReferences = new List<ProjectReference>();
    private readonly List<ProjectInfo> _projects = new List<ProjectInfo>();
    private readonly string _projectName = ProjectName;

    #endregion

    #region ctors

    public RoslynWorkspaceBuilder()
    {
        this._projectId = ProjectId.CreateNewId();
    }

    #endregion

    #region members

    public RoslynWorkspaceBuilder AddDocument(string filePath, string sourceCode)
    {
        var sourceText = SourceText.From(sourceCode);

        this._documents.Add(this.CreateDocumentInfo(sourceText, filePath));
        return this;
    }

    public RoslynWorkspaceBuilder AddReference(string filePath)
    {
        var metadataReference = MetadataReference.CreateFromFile(filePath);
        this._metaDataReferences.Add(metadataReference);
        return this;
    }

    public RoslynWorkspaceBuilder AddReference(ProjectReference projectReference)
    {
        this._projectReferences.Add(projectReference);
        return this;
    }

    public RoslynWorkspaceBuilder AddProject(string projectName, string assemblyName, string filePath = null)
    {
        var versionStamp = VersionStamp.Create();
        var info = ProjectInfo.Create(ProjectId.CreateNewId(), versionStamp, projectName, assemblyName, LanguageNames.CSharp, filePath: filePath);
        this._projects.Add(info);
        return this;
    }

    public Workspace Build()
    {
        var workspace = new AdhocWorkspace();

        var versionStamp = VersionStamp.Create();

        var projectInfo = ProjectInfo.Create(
            this._projectId,
            versionStamp,
            this._projectName,
            this._projectName,
            LanguageNames.CSharp,
            this._projectName,
            compilationOptions: new CSharpCompilationOptions(
                OutputKind.ConsoleApplication,
                metadataImportOptions: MetadataImportOptions.All));

        workspace.AddProject(projectInfo);

        foreach (var info in this._documents)
        {
            workspace.AddDocument(info);
        }

        var solution = workspace.CurrentSolution.AddMetadataReferences(this._projectId, this._metaDataReferences);
        solution = solution.AddProjectReferences(this._projectId, this._projectReferences);

        foreach (var project in this._projects)
        {
            solution = solution.AddProject(project);
            solution = solution.AddMetadataReferences(project.Id, this._metaDataReferences);
            solution = solution.AddProjectReferences(project.Id, this._projectReferences);
        }

        if (!workspace.TryApplyChanges(solution))
        {
            throw new ArgumentException("Cannot apply changes.");
        }

        return workspace;
    }

    private DocumentInfo CreateDocumentInfo(SourceText sourceText, string filePath)
    {
        var loader = TextLoader.From(TextAndVersion.Create(sourceText, VersionStamp.Create()));

        return DocumentInfo.Create(
            DocumentId.CreateNewId(this._projectId),
            filePath,
            null,
            SourceCodeKind.Regular,
            loader,
            filePath);
    }

    #endregion
}