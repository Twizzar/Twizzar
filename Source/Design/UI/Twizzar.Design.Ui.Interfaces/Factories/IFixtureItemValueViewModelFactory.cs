using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Ui.Interfaces.Factories
{
    /// <summary>
    /// Factory for creating a fixture item view model.
    /// </summary>
    public interface IFixtureItemValueViewModelFactory : IFactory
    {
        #region members

        /// <summary>
        /// Create a <see cref="IFixtureItemNodeValueViewModel"/> form the type.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeDescription"></param>
        /// <param name="fixtureItemInformation"></param>
        /// <param name="compilationTypeQuery"></param>
        /// <returns>A new instance of <see cref="IFixtureItemNodeValueViewModel"/>.</returns>
        IFixtureItemNodeValueViewModel CreateWithType(
            NodeId id,
            IBaseDescription typeDescription,
            IFixtureItemInformation fixtureItemInformation,
            ICompilationTypeQuery compilationTypeQuery);

        /// <summary>
        /// Create a <see cref="IFixtureItemNodeValueViewModel"/> for a constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="baseDescription"></param>
        /// <returns>A new instance of <see cref="IFixtureItemNodeValueViewModel"/>.</returns>
        IFixtureItemNodeValueViewModel CreateForCtor(NodeId id, IBaseDescription baseDescription);

        #endregion
    }
}