using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.Core.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Dummies;

public class RoslynDescriptionFactoryDummy : IRoslynDescriptionFactory
{
    #region Implementation of IRoslynDescriptionFactory

    public ITypeDescription CreateDescription(ITypeSymbol symbol) =>
        new RoslynTypeDescription(symbol, new BaseTypeService(), this);

    #endregion
}