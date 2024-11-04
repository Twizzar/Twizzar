using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE80;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Twizzar.Design.CoreInterfaces.Common.Util;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services
{
    /// <summary>
    /// Implementing <see cref="IVsCommandQuery"/>, to allow querying vs commands.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VsCommandQuery : IVsCommandQuery
    {
        private readonly string _openOrCloseCommandName = "Twizzar.OpenOrClose";

        /// <inheritdoc/>
        public async Task<string> GetShortCutOfOpenCloseCommandAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (Package.GetGlobalService(typeof(SDTE)) is DTE2 dte2)
            {
                try
                {
                    var openCloseCommand = dte2.Commands.Item(this._openOrCloseCommandName);

                    if (openCloseCommand.Bindings is not object[] bindings)
                    {
                        return string.Empty;
                    }

                    // Bindings format: "scopename::modifiers+key"
                    // https://docs.microsoft.com/en-us/dotnet/api/envdte.command.bindings?view=visualstudiosdk-2022
                    var keyStroke = bindings.FirstOrDefault() as string;
                    return keyStroke?.Split(new[] { "::" }, StringSplitOptions.None).LastOrDefault() ?? string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
