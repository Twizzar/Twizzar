using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.NLog.Interfaces;

namespace Twizzar.Design.Ui.Interfaces.ViewModels
{
    /// <summary>
    /// The base view model class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ViewModelBase : INotifyPropertyChanged, IHasLogger
    {
        /// <summary>
        /// The property changed event handler for the view model base class.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has been changed as string.</param>
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}