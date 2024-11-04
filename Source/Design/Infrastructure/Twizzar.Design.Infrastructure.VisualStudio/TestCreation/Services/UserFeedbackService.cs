using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Twizzar.Design.CoreInterfaces.TestCreation;

namespace TestCreation.Services;

/// <summary>
/// Implementation of <see cref="IUserFeedbackService"/> with Visual Studio.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserFeedbackService : IUserFeedbackService
{
    #region members

    /// <inheritdoc />
    public Task ShowMessageBoxAsync(string message) =>
        VS.MessageBox.ShowAsync(message, buttons: OLEMSGBUTTON.OLEMSGBUTTON_OK);

    /// <inheritdoc />
    public Task ShowErrorBoxAsync(string message) =>
        VS.MessageBox.ShowErrorAsync(message);

    /// <inheritdoc />
    public async Task<bool> ShowYesNoBoxAsync(string message)
    {
        var messageBoxResult = await VS.MessageBox.ShowAsync(message, buttons: OLEMSGBUTTON.OLEMSGBUTTON_YESNO);
        return messageBoxResult == VSConstants.MessageBoxResult.IDYES;
    }

    #endregion
}