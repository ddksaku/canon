using System;

namespace Canon.Data.Exceptions
{
    /// <summary>
    /// Assigned product exists exception.
    /// </summary>
    public class ProductAssignedException : Exception
    {
        #region Constructor
        /// <summary>
        /// Creates a new instance of ProductAssignedException.
        /// </summary>
        /// <param name="login"></param>
        public ProductAssignedException(string productgroup)
        {
            ProductGroupName = productgroup;
        }

        public ProductAssignedException(string productgroup, int count)
            : this(productgroup)
        {
            ProductsCount = count;
        }
        #endregion

        #region ProductGroupName
        /// <summary>
        /// CategoryName
        /// </summary>
        public string ProductGroupName
        {
            get;
            set;
        }
        #endregion

        #region Products Count
        public int ProductsCount
        {
            get;
            set;
        }
        #endregion

        #region Message
        /// <summary>
        /// Message of the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("Produktovou skupinu nelze odebrat, jsou k ní přiřazeny produkty ({0})!", ProductsCount);
            }
        }
        #endregion
    }
}
