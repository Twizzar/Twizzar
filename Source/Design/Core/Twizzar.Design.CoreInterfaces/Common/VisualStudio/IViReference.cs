using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Internal representation of a Project Reference.
    /// </summary>
    public interface IViReference
    {
        #region properties

        /// <summary>Gets the project that the selected item is a part of. Read-only.</summary>
        /// <returns>Returns a Project object.</returns>
        IViProject ContainingProject { get; }

        /// <summary>Gets the name of the object. Read-only.</summary>
        /// <returns>String.The string returned depends on the reference type.Reference TypeValue ReturnedAssemblyThe assembly name, which is the file name of the reference without the extension.COMThe name of the wrapper for the referenced type library, which is the file name without the extension.</returns>
        string Name { get; }

        /// <summary>Gets the path to the reference file. Read-only. </summary>
        /// <returns>String. This property returns the path and file name of the reference, if it can be resolved. If the path cannot be resolved, then a blank string is returned. For information on resolving references, see <see cref="M:VSLangProj.References.Add(System.String)" />.</returns>
        string Path { get; }

        /// <summary>Gets a text description of the reference. Read-only.</summary>
        /// <returns>For a <see cref="T:VSLangProj.Reference" /> object, the return value depends on the reference type.Reference TypeValue ReturnedAssemblyAssembly description.COMType library description.</returns>
        string Description { get; }

        /// <summary>Gets the culture string of a reference. Read-only.</summary>
        /// <returns>The return value depends on the reference type.Reference TypeValue ReturnedAssemblyCulture string. For example, "EN-US" for English - United States.COMLocale ID. The string is the hex locale ID of the type library being reference. For example, "0" for multilanguage or "409" for English - United States.</returns>
        string Culture { get; }

        /// <summary>Gets the major version number of the reference. Read-only. </summary>
        /// <returns>Long. The number returned depends on the reference type.Reference TypeValue ReturnedAssemblyMajor release number of the reference, long value.</returns>
        int MajorVersion { get; }

        /// <summary>Gets the minor version number of the reference. Read-only. </summary>
        /// <returns>Long. The number returned depends on the reference type.Reference TypeValue ReturnedAssemblyMinor release number of the reference, long value.</returns>
        int MinorVersion { get; }

        /// <summary>Gets the revision number of the reference. Read-only. </summary>
        /// <returns>Long. The number returned depends on the reference type.Reference TypeValue ReturnedAssemblyRevision version number of the reference.</returns>
        int RevisionNumber { get; }

        /// <summary>Gets the build number of the reference. Read-only.</summary>
        /// <returns>Long. The value returned depends on the reference type. Reference TypeValue ReturnedAssemblyBuild number of the reference.</returns>
        int BuildNumber { get; }

        /// <summary>Gets a value indicating whether the reference is signed with a public/private key pair. Read-only. </summary>
        /// <returns>true if the reference is signed with a public/private key pair.</returns>
        bool StrongName { get; }

        /// <summary>Gets a Project object if the reference is a project. Otherwise, it returns None. Read-only. </summary>
        /// <returns>Returns a Project object.</returns>
        Maybe<IViProject> SourceProject { get; }

        /// <summary>Gets or sets a value indicating whether the reference is copied to the local bin path. </summary>
        /// <returns>Boolean.</returns>
        bool CopyLocal { get; set; }

        /// <summary>Gets the version of the selected reference.</summary>
        /// <returns>A string representing the version of the selected reference.</returns>
        string Version { get; }

        #endregion

        #region members

        /// <summary>Gets the reference from the <see cref="T:VSLangProj.References" /> object that contains it.</summary>
        void Remove();

        #endregion
    }
}