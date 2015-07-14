namespace Canon.Data.Enums
{
    /// <summary>
    /// Mapping log events
    /// </summary>
    public enum MappingLogEnum
    {
        /// <summary>
        /// New mapping rule is added
        /// </summary>
        NewMappingRule = 24,
        /// <summary>
        /// Mapping rule is changed
        /// </summary>
        ChangedMappingRule = 25,
        /// <summary>
        /// Mapping rule is deleted
        /// </summary>
        DeletedMappingRule = 27,
        /// <summary>
        /// Product added to exceptions
        /// </summary>
        AddedToExceptions = 29,
        /// <summary>
        /// Product removed from exception
        /// </summary>
        RemovedFromExceptions = 30,
        /// <summary>
        /// Price without changes
        /// </summary>
        PriceWithoutChanges = 31,
        /// <summary>
        /// New price
        /// </summary>
        NewPrice = 32
    }
}