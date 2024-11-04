namespace Twizzar.SharedKernel.CoreInterfaces.Extensions;

/// <summary>
/// Extension methods for string.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///  Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
    ///  When self is empty or oldValue is empty then this will return self.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public static string ReplaceSafe(this string self, string oldValue, string newValue)
    {
        if (string.IsNullOrEmpty(self))
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(oldValue))
        {
            return self;
        }

        return self.Replace(oldValue, newValue);
    }
}