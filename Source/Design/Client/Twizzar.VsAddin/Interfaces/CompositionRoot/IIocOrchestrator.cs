namespace Twizzar.VsAddin.Interfaces.CompositionRoot
{
    /// <summary>
    /// The IoC root container.
    /// </summary>
    public interface IIocOrchestrator
    {
        /// <summary>
        /// Resolves a Interface of T to generate a member of the registered base type.
        /// </summary>
        /// <typeparam name="T">The type of the instance that should be generated.</typeparam>
        /// <returns>A instance of the resolved type T.</returns>
        T Resolve<T>();
    }
}