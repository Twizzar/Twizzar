using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;

namespace Twizzar.Design.Infrastructure.VisualStudio.Common.Util
{
    /// <inheritdoc cref="IAddinVersionQuery"/>
    [ExcludeFromCodeCoverage] // The version changes.
    public class AddinVersionQuery : IAddinVersionQuery
    {
        private const string VersionError = "N/A";
        private readonly Assembly _assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddinVersionQuery"/> class.
        /// </summary>
        public AddinVersionQuery()
        {
            this._assembly = Assembly.GetExecutingAssembly();
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IAddinVersionQuery

        /// <inheritdoc />
        public string GetVsAddinVersion()
        {
            try
            {
                var assemblyUri = new UriBuilder(this._assembly.CodeBase);
                var assemblyPath = Uri.UnescapeDataString(assemblyUri.Path);
                var assemblyDirectory = Path.GetDirectoryName(assemblyPath);

                if (assemblyDirectory == null)
                {
                    this.Logger?.Log(LogLevel.Error, "Cannot get the assembly directory");
                    return VersionError;
                }

                var manifestPath = Path.Combine(assemblyDirectory, "extension.vsixmanifest");

                var doc = new XmlDocument();
                doc.Load(manifestPath);

                if (doc?.DocumentElement?.ChildNodes == null)
                {
                    this.Logger?.Log(LogLevel.Error, $"Cannot parse the manifest file at {manifestPath}");
                    return VersionError;
                }

                var metaData = doc.DocumentElement.ChildNodes.Cast<XmlElement>().First(x => x.Name == "Metadata");
                var identity = metaData.ChildNodes.Cast<XmlElement>().First(x => x.Name == "Identity");
                var version = identity.GetAttribute("Version");
                return version;
            }
            catch (Exception e)
            {
                this.Logger?.Log(LogLevel.Error, e);
                return VersionError;
            }
        }

        /// <inheritdoc />
        public string GetDllVersion() =>
            this._assembly.GetName().Version.ToString();

        /// <inheritdoc />
        public string GetProductVersion()
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(this._assembly.Location);
            return fileVersionInfo.ProductVersion;
        }

        #endregion
    }
}