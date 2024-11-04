using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Query.Services;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces;

/// <summary>
/// Factory for <see cref="ICompilationTypeQuery"/>.
/// </summary>
public interface ICompilationTypeQueryFactory
{
    /// <summary>
    /// Create a new instance of <see cref="ICompilationTypeQuery"/>.
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    ICompilationTypeQuery Create(Compilation compilation);
}