using System.Diagnostics.CodeAnalysis;
using Autofac;
using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.Factories;

namespace Twizzar.VsAddin.Factory;

/// <summary>
/// Factory for <see cref="ICompilationTypeQuery"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class CompilationTypeQueryFactory : FactoryBase, ICompilationTypeQueryFactory
{
    private readonly Factory _compilationTypeQueryFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationTypeQueryFactory"/> class.
    /// </summary>
    /// <param name="componentContext"></param>
    /// <param name="compilationTypeQueryFactory"></param>
    public CompilationTypeQueryFactory(IComponentContext componentContext, Factory compilationTypeQueryFactory)
        : base(componentContext)
    {
        this._compilationTypeQueryFactory = compilationTypeQueryFactory;
    }

    /// <summary>
    /// Delegate for autofac.
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns>A new instance of <see cref="ITypeDescription"/>.</returns>
    public delegate ICompilationTypeQuery Factory(Compilation compilation);

    /// <summary>
    /// Create a new instance of <see cref="ICompilationTypeQuery"/>.
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public ICompilationTypeQuery Create(Compilation compilation) =>
        this._compilationTypeQueryFactory(compilation);
}