using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.Command.FixtureItem.Config
{
    /// <inheritdoc cref="IFileWriter" />
    [ExcludeFromCodeCoverage]
    public class FileWriter : IFileWriter, IService
    {
        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IFileWriter

        /// <inheritdoc />
        public Task<IResult<Unit, IoFailure>> WriteAsync(string text, string filePath)
        {
            this.EnsureMany<string>()
                .Parameter(text, nameof(text))
                .Parameter(filePath, nameof(filePath))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            try
            {
                var fullPath = Path.GetFullPath(filePath);
                File.WriteAllText(fullPath, text);
            }
            catch (Exception e) when (e is DirectoryNotFoundException
                                      || e is IOException
                                      || e is UnauthorizedAccessException
                                      || e is NotSupportedException
                                      || e is SecurityException)
            {
                return Task.FromResult(
                    Result.Failure<Unit, IoFailure>(new IoFailure(e.Message, filePath)));
            }

            return Task.FromResult<IResult<Unit, IoFailure>>(Result.Success<IoFailure>());
        }

        #endregion
    }
}
