using System.IO;

namespace Twizzar.Design.Infrastructure.Services
{
    /// <summary>
    /// Adapter for <see cref="File"/>.
    /// </summary>
    internal interface IFileService
    {
        /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
        /// <param name="path">The file to open for reading.</param>
        /// <exception cref="T:System.ArgumentException">.NET Framework and .NET Core versions older than 2.1: <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///        <paramref name="path" /> specified a file that is read-only.
        ///
        /// -or-
        ///
        /// This operation is not supported on the current platform.
        ///
        /// -or-
        ///
        /// <paramref name="path" /> specified a directory.
        ///
        /// -or-
        ///
        /// The caller does not have the required permission.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found.</exception>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="path" /> is in an invalid format.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <returns>A string array containing all lines of the file.</returns>
        string[] ReadAllLines(string path);

        /// <summary>Determines whether the specified file exists.</summary>
        /// <param name="path">The file to check.</param>
        /// <returns>
        /// <see langword="true" /> if the caller has the required permissions and <paramref name="path" /> contains the name of an existing file; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="path" /> is <see langword="null" />, an invalid path, or a zero-length string. If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the method returns <see langword="false" /> regardless of the existence of <paramref name="path" />.</returns>
        bool Exists(string path);

        /// <summary>Creates or overwrites a file in the specified path.</summary>
        /// <param name="path">The path and name of the file to create.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.
        ///
        /// -or-
        ///
        /// <paramref name="path" /> specified a file that is read-only.
        ///
        /// -or-
        ///
        /// <paramref name="path" /> specified a file that is hidden.</exception>
        /// <exception cref="T:System.ArgumentException">.NET Framework and .NET Core versions older than 2.1: <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while creating the file.</exception>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="path" /> is in an invalid format.</exception>
        void Create(string path);

        /// <summary>Creates a new file, write the specified string array to the file, and then closes the file.</summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string array to write to the file.</param>
        /// <exception cref="T:System.ArgumentException">.NET Framework and .NET Core versions older than 2.1: <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">Either <paramref name="path" /> or <paramref name="contents" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///        <paramref name="path" /> specified a file that is read-only.
        ///
        /// -or-
        ///
        /// <paramref name="path" /> specified a file that is hidden.
        ///
        /// -or-
        ///
        /// This operation is not supported on the current platform.
        ///
        /// -or-
        ///
        /// <paramref name="path" /> specified a directory.
        ///
        /// -or-
        ///
        /// The caller does not have the required permission.</exception>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="path" /> is in an invalid format.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        void WriteAllLines(string path, string[] contents);
    }
}