using Twizzar.Design.Ui.Interfaces.ViewModels;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.Factories
{
    /// <summary>
    /// Factory for creating a <see cref="IAboutViewModel"/>.
    /// </summary>
    /// <seealso cref="IFactory" />
    public interface IAboutViewModelFactory : IFactory
    {
        /// <summary>
        /// Creates the about view model.
        /// </summary>
        /// <returns>A new instance of the about view model.</returns>
        IAboutViewModel CreateAboutViewModel();
    }
}