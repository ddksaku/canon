
namespace Canon.Data.Enums
{
    /// <summary>
    /// Channel import errors
    /// </summary>
    public enum ChannelErrorEnum
    {
        /// <summary>
        /// Channel is unavailable
        /// </summary>
        ChannelIsNotAvailable = 16,
        /// <summary>
        /// Feed format is wrong
        /// </summary>
        FeedFormatIsWrong = 17,
        /// <summary>
        /// Product parse error
        /// </summary>
        ProductParsingError = 18,
        /// <summary>
        /// Channel mapping error
        /// </summary>
        ChannelMappingError = 19,
        /// <summary>
        /// Channel is empty or format is wrong
        /// </summary>
        ChannelIsEmpty = 20    
    }
}
