using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

using Autofac.Core.Activators.Reflection;

namespace Twizzar.VsAddin.CompositionRoot.Autofac
{
    /// <summary>
    /// Autofac ctor finder for internal ctors.
    /// </summary>
    public class AllCtorFinder : IConstructorFinder
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfo[]> Cache =
            new();

        /// <inheritdoc />
        public ConstructorInfo[] FindConstructors(Type targetType)
        {
            var result = Cache.GetOrAdd(
                targetType,
                t => t.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsStatic).ToArray());

            return result.Length > 0 ? result : throw new NoConstructorsFoundException(targetType);
        }
    }
}
