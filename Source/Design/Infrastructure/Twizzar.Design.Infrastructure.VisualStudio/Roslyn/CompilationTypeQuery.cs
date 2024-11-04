using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Roslyn;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio2019.Roslyn;

/// <inheritdoc cref="ICompilationTypeQuery"/>
public class CompilationTypeQuery : ICompilationTypeQuery
{
    #region fields

    private readonly Compilation _compilation;
    private readonly IRoslynDescriptionFactory _descriptionFactory;
    private readonly Lazy<IReadOnlyList<ICompilationTypeQuery.ITypeResult>> _cached;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationTypeQuery"/> class.
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="descriptionFactory"></param>
    /// <param name="compilationTypeCache"></param>
    public CompilationTypeQuery(
        Compilation compilation,
        IRoslynDescriptionFactory descriptionFactory,
        ICompilationTypeCache compilationTypeCache)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(compilation, nameof(compilation))
            .Parameter(descriptionFactory, nameof(descriptionFactory))
            .Parameter(compilationTypeCache, nameof(compilationTypeCache))
            .ThrowWhenNull();

        this._compilation = compilation;
        this._descriptionFactory = descriptionFactory;

        this._cached = new Lazy<IReadOnlyList<ICompilationTypeQuery.ITypeResult>>(() =>
            compilationTypeCache.GetAllTypeForAssembly(compilation.AssemblyName)
                .Select(description =>
                    new TypeResult(description.TypeFullName.GetTypeFullNameWithGenericPostfix(), description))
                .ToList());
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public IBaseDescription ObjectTypeDescription =>
        this._descriptionFactory.CreateDescription(this._compilation.ObjectType);

    /// <inheritdoc />
    public IBaseDescription ValueTypeDescription =>
        this._descriptionFactory.CreateDescription(this._compilation.GetSpecialType(SpecialType.System_ValueType));

    /// <inheritdoc />
    public IEnumerable<IBaseDescription> AllTypes => this._cached.Value.Select(x => x.Description);

    #endregion

    #region members

    /// <inheritdoc />
    public IEnumerable<ICompilationTypeQuery.ITypeResult> FindTypes(Predicate<string> predicate) =>
        this._cached.Value.Where(result => predicate(result.MetadataName));

    /// <inheritdoc />
    public IEnumerable<ICompilationTypeQuery.ITypeResult> FindTypes(
        Predicate<ICompilationTypeQuery.ITypeResult> predicate) =>
        this._cached.Value.Where(result => predicate(result));

    #endregion

    #region Nested type: TypeResult

    /// <summary>
    /// Implementation for <see cref="ICompilationTypeQuery.ITypeResult"/>.
    /// </summary>
    /// <param name="MetadataName"></param>
    /// <param name="Description"></param>
    public record TypeResult(string MetadataName, IBaseDescription Description) : ICompilationTypeQuery.ITypeResult;

    #endregion
}