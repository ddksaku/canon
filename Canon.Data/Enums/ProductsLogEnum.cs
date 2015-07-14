namespace Canon.Data.Enums
{
    /// <summary>
    /// Product log types, see Enums table
    /// </summary>
    public enum ProductsLogEnum
    {
        /// <summary>
        /// When product is created
        /// </summary>
        ProductIsCreated = 1,
        /// <summary>
        /// When product's name is changed
        /// </summary>
        NameIsChanged = 2,
        /// <summary>
        /// When product's price is changed
        /// </summary>
        PriceIsChanged = 3
    }
}