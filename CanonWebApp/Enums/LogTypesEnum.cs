namespace CanonWebApp.Enums
{
    /// <summary>
    /// Types of log tables
    /// </summary>
    public enum LogTypesEnum
    {
        /// <summary>
        /// History of product mapping (by channel, product)
        /// </summary>
        ProductMappingHistory = 1,
        /// <summary>
        /// Channel monitoring history (by channel)
        /// </summary>
        ChannelMonitoringHistory = 2,
        /// <summary>
        /// Product import history (general)
        /// </summary>
        ProductImportHistory = 3,
        /// <summary>
        /// Recommended price history (by product)
        /// </summary>
        RecommendedPriceHistory = 4,
        /// <summary>
        /// Main log error (by channel)
        /// </summary>
        ChannelLog = 5
    }
}