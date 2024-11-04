using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EnvDTE;

namespace Twizzar.Design.Infrastructure.VisualStudio.Extensions
{
    /// <summary>
    /// Extension for Visual Studio SDK ConcreteType CodeElements.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class CodeElementsExtensions
    {
        /// <summary>
        /// Converts the CodeElements object into a list of given types which can be filtered
        /// with the filterFunction.
        /// </summary>
        /// <param name="codeElements">The input enumerable codeElements.</param>
        /// <param name="filterFunc">The given filter function which can be null.</param>
        /// <typeparam name="T">The type in which the elements will be converted to.</typeparam>
        /// <returns>A list of type T from the input codeElements.</returns>
        public static IList<T> ElementsOfType<T>(this CodeElements codeElements, Func<T, bool> filterFunc = null)
            where T : class
        {
            if (!(codeElements is IEnumerable enumerable))
            {
                return new List<T>();
            }

            var typedElements = enumerable.OfType<T>();

            return filterFunc is null ?
                typedElements.ToList() :
                typedElements.Where(filterFunc).ToList();
        }
    }
}