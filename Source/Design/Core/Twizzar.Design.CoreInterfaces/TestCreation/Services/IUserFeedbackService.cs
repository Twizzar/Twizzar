using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Service for providing user feedback.
/// </summary>
public interface IUserFeedbackService
{
    /// <summary>
    /// Show a info message box.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task ShowMessageBoxAsync(string message);

    /// <summary>
    /// Show a error message box.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task ShowErrorBoxAsync(string message);

    /// <summary>
    /// Show a yes/no message box.
    /// This will wait for the response of the user.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>True if yes was clicked by the user.</returns>
    Task<bool> ShowYesNoBoxAsync(string message);
}