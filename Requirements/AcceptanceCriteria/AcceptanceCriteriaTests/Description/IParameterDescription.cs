namespace AcceptanceCriteriaTests.Description
{
    /// <summary>
    /// Gets the attributes for this parameter.
    /// </summary>
    internal interface IParameterDescription
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this is an input parameter.
        /// </summary>
        bool IsIn { get; }

        /// <summary>
        /// Gets a value indicating whether this parameter is optional.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Gets a value indicating whether this is an out parameter.
        /// </summary>
        bool IsOut { get; }

        /// <summary>
        /// Gets the parameter position in the method.
        /// </summary>
        int Position { get; }
    }
}