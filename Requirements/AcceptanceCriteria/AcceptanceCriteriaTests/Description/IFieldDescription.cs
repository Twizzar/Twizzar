namespace AcceptanceCriteriaTests.Description
{
    /// <summary>
    /// Discovers the attributes of a field and provides access to field metadata.
    /// </summary>
    internal interface IFieldDescription : IMemberDescription
    {
        /// <summary>
        /// Gets the declaring type of this Field.
        /// </summary>
        public ITypeFullName DeclaringType { get; }

        /// <summary>
        /// Gets a value indicating whether the field is static.
        /// </summary>
        public bool IsStatic { get; }

        /// <summary>
        /// Gets a value indicating whether the field is constant.
        /// </summary>
        public bool IsConstant { get; }

        /// <summary>
        /// Gets a value indicating whether the field is readonly.
        /// </summary>
        public bool IsReadonly { get; }

    }
}