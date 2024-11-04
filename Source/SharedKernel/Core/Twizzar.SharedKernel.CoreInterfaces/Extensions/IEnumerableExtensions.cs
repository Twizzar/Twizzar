using System;
using System.Collections.Generic;
using System.Linq;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions;

/// <summary>
/// Basic extension methods for the <see cref="IEnumerable{T}"/> class.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Calculate the cartesian product of sequences.
    /// The cartesian product returns all possible combinations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequences"></param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };

        return sequences.Aggregate(
            emptyProduct,
            (accumulator, sequence) => accumulator.SelectMany(
                _ => sequence,
                (accseq, item) => accseq.Concat(new[] { item })));
    }

    /// <summary>
    /// Linq style for each implementation for enumerable.
    /// </summary>
    /// <param name="enumeration">The enumerable which will be processed.</param>
    /// <param name="action">The executing action for the elements on the enumeration.</param>
    /// <typeparam name="T">The type parameter for the enumeration.</typeparam>
    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        if (enumeration != null && action != null)
        {
            foreach (var item in enumeration)
            {
                action(item);
            }
        }
    }

    /// <summary>
    /// Gets the variance of numbers.
    /// </summary>
    /// <param name="values">a sequence of value.</param>
    /// <returns>The variance. 1/n * sum((x - avg)^2).</returns>
    public static double Variance(this IEnumerable<double> values)
    {
        var vs = values.ToList();
        var mean = (float)vs.Average();
        return Variance(vs, mean);
    }

    /// <summary>
    /// Gets the variance of numbers.
    /// </summary>
    /// <param name="values">A sequence of value.</param>
    /// <param name="mean">Pre computed mean.</param>
    /// <returns>The variance. 1/n * sum((x - avg)^2).</returns>
    public static double Variance(this IEnumerable<double> values, double mean)
    {
        var vs = values.ToList();
        var n = vs.Count;
        return 1f / n *
               vs
                   .Select(x => Math.Pow(x - mean, 2))
                   .Sum();
    }

    /// <summary>
    /// Gets the standard deviation of numbers.
    /// </summary>
    /// <param name="values">a sequence of value.</param>
    /// <returns>The variance. sqrt(1/n * sum((x - avg)^2)).</returns>
    public static double StandardDeviation(this IEnumerable<double> values)
    {
        return Math.Sqrt(values.Variance());
    }

    /// <summary>
    /// Gets the standard deviation of numbers.
    /// </summary>
    /// <param name="values">a sequence of value.</param>
    /// <param name="mean">Pre computed mean.</param>
    /// <returns>The variance. sqrt(1/n * sum((x - avg)^2)).</returns>
    public static double StandardDeviation(this IEnumerable<double> values, double mean)
    {
        return Math.Sqrt(values.Variance(mean));
    }

    /// <summary>
    /// Gets a HashCode of a collection by combining the hash codes of its elements. Takes order into account.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    /// <param name="source">Collection to generate the hash code for.</param>
    /// <param name="comparer">The comparer to use default is <see cref="IEqualityComparer{T}"/>.Default.</param>
    /// <returns>Generated hash code.</returns>
    public static int GetHashCodeOfElements<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        comparer ??= EqualityComparer<T>.Default;

        unchecked
        {
            return source.Aggregate(17, (current, element) => (current * 23) + comparer.GetHashCode(element));
        }
    }
}