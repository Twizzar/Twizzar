using System;
using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.CoreInterfaces.Query.Services;

/// <summary>
/// Query for types of one specific compilation (assembly).
/// </summary>
public interface ICompilationTypeQuery
{
    /// <summary>
    /// Resulting type.
    /// </summary>
    public interface ITypeResult
    {
        #region properties

        /// <summary>
        /// Gets the metadata name.
        /// </summary>
        string MetadataName { get; }

        /// <summary>
        /// Gets the type description.
        /// </summary>
        IBaseDescription Description { get; }

        #endregion
    }

    #region properties

    /// <summary>
    /// Gets the type description for the <see cref="object"/> type.
    /// </summary>
    IBaseDescription ObjectTypeDescription { get; }

    /// <summary>
    /// Gets the type description for the <see cref="ValueType"/> type.
    /// </summary>
    IBaseDescription ValueTypeDescription { get; }

    /// <summary>
    /// Gets all types form a specific compilation.
    /// </summary>
    IEnumerable<IBaseDescription> AllTypes { get; }

    #endregion

    #region members

    /// <summary>
    /// Find a type be a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IEnumerable<ITypeResult> FindTypes(Predicate<string> predicate);

    /// <summary>
    /// Find a type be a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IEnumerable<ITypeResult> FindTypes(Predicate<ITypeResult> predicate);

    #endregion
}