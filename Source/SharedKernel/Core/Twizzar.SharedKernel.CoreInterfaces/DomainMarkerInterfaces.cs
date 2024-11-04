using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;

#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1502 // Element should not be on a single line

namespace Twizzar.SharedKernel.CoreInterfaces
{
    /// <summary>
    /// Marker Interface for value objects.
    /// </summary>
    public interface IValueObject { }

    /// <summary>
    /// Marker Interfaces for entities.
    /// </summary>
    public interface IEntity : IHasLogger, IHasEnsureHelper { }

    /// <summary>
    /// Marker interface for read models.
    /// </summary>
    public interface IReadModel { }

    /// <summary>
    /// Marker interface for services.
    /// </summary>
    public interface IService : IHasLogger, IHasEnsureHelper { }

    /// <summary>
    /// Marker interface for queries.
    /// </summary>
    public interface IQuery : IService { }

    /// <summary>
    /// Marker interface for factories.
    /// </summary>
    public interface IFactory : IService { }
}
