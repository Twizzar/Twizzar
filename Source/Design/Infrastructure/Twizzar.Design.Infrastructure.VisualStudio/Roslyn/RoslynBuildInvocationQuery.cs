using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn;

/// <inheritdoc />
public class RoslynBuildInvocationQuery : IBuildInvocationQuery
{
    #region fields

    private readonly Workspace _workspace;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="RoslynBuildInvocationQuery"/> class.
    /// </summary>
    /// <param name="workspace"></param>
    public RoslynBuildInvocationQuery(Workspace workspace)
    {
        this._workspace = workspace;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public Task<IResult<int, Failure>> GetBuildInvocationCountAsync(
        string projectName,
        CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return this._workspace.CurrentSolution.GetProjectByName(projectName)
            .ToResult($"Cannot find the project with the name {projectName}")
            .MapSuccessAsync(project => project.GetCompilationAsync(token))
            .BindAsync(compilation =>
                ToResult(
                    compilation.GetTypeByMetadataName(ApiNames.ProjectStatisticsTypeFullName),
                    $"Cannot find the type {ApiNames.ProjectStatisticsTypeFullName}"))
            .BindAsync(symbol =>
                symbol.GetMembers(ApiNames.BuilderInvocationCountMemberName)
                    .FirstOrNone()
                    .ToResult($"Cannot find the member {ApiNames.BuilderInvocationCountMemberName}"))
            .BindAsync(symbol =>
                symbol is IFieldSymbol fieldSymbol
                    ? Success(fieldSymbol)
                    : Failure<IFieldSymbol>($"The member {ApiNames.BuilderInvocationCountMemberName} is not a field."))
            .BindAsync(symbol => ToResult(
                symbol.ConstantValue,
                $"Cannot find the constant value of the field {ApiNames.BuilderInvocationCountMemberName}"))
            .BindAsync(o =>
                o is int i
                    ? Success(i)
                    : Failure<int>(
                        $"The constant value of {ApiNames.BuilderInvocationCountMemberName} is not of type int"));
    }

    private static IResult<T, Failure> Success<T>(T success) => Result.Success<T, Failure>(success);

    private static IResult<T, Failure> Failure<T>(string message) => Result.Failure<T, Failure>(new Failure(message));

    private static IResult<T, Failure> ToResult<T>(T nullable, string message) =>
        nullable is not null
            ? Success(nullable)
            : Failure<T>(message);

    #endregion
}