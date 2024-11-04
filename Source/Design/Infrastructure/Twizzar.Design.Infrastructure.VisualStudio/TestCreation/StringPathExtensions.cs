using System.IO;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation;

/// <summary>
/// Extension method for strings which represent a file path.
/// </summary>
public static class StringPathExtensions
{
    /// <summary>
    /// Split the path into the prefix and the file name.
    /// Where for C:\Directory\FileName
    /// Prefix is C:\Directory\
    /// file name is FileName.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static (string Prefix, string FileName, string Extension) SplitPath(this string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path);
        var filePrefix = fileName == string.Empty
            ? path
            : path.Replace(fileName + extension, string.Empty);
        return (filePrefix, fileName, extension);
    }
}