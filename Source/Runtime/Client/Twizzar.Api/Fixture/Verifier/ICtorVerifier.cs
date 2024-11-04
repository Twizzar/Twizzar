namespace Twizzar.Fixture.Verifier
{
    /// <summary>
    /// Ctor verifier for Verifying the constructor.
    /// </summary>
    /// <typeparam name="T">The type of the class the constructor is creating.</typeparam>
    public interface ICtorVerifier<T>
    {
        #region members

        /// <summary>
        /// Provide an instance which will be used when the parameter will not be set to null.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="instance">The instance to use.</param>
        /// <returns>The <see cref="CtorVerifier{T}"/> for further configuration.</returns>
        ICtorVerifier<T> SetupParameter(string parameterName, object instance);

        /// <summary>
        /// Calls all constructors matching the given configuration n times, where n is the number of verifiable parameters.
        /// Without any configuration all parameters which have no default value are verifiable.
        /// With every call one of the verifiable parameter is set to null and a ArgumentNullException is expected.
        /// </summary>
        void ShouldThrowArgumentNullException();

        /// <summary>
        /// Configure a parameter to be ignored for verification.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="defaultValue">The default value provided to the constructor.</param>
        /// <returns>The <see cref="CtorVerifier{T}"/> for further configuration.</returns>
        ICtorVerifier<T> IgnoreParameter(string parameterName, object defaultValue = null);

        #endregion
    }
}