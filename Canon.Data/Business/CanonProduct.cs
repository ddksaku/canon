using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Canon.Data.Enums;
using Canon.Data.Import;

namespace Canon.Data.Business
{
    public class CanonProduct : Product
    {
        public List<ImportErrorMessage> InsertUpdateProductWithPrice()
        {
            List<ImportErrorMessage> warnings = new List<ImportErrorMessage>();



            Product updated = this.InsertUpdateProduct();

            //find prices
            if (this.RecommendedPrices.Count > 0)
            {
                this.InsertUpdatePrice(updated);
            }
            else
            {
                //add into user's log warning - no price for this product
                warnings.Add(new ImportErrorMessage("NoPriceForProductError", 
                    new string[] { updated.ProductName, updated.ProductCode }));
            }

            //update relevance words
            this.DeleteRelevanceWords(updated);
            if (this.ProductsRelevances.Count > 0)
            {
                this.InsertRelevanceWords(updated);
            }

            return warnings;
        }

        public Product InsertUpdateProduct()
        {
            CanonDataContext db = Cdb.Instance;
            int? categoryId = this.InsertUpdateCategory();
            bool isProductCreated = false;
            bool isNameChanged = false;
            string oldName = string.Empty;
            //find product
            Product existing = db.Products.FirstOrDefault(p => p.ProductCode == this.ProductCode);
            if (existing == null)
            {
                existing = new Product();
                db.Products.InsertOnSubmit(existing);
                isProductCreated = true;
            }
            else
            {
                if (existing.ProductName != this.ProductName)
                {
                    oldName = existing.ProductName;
                    isNameChanged = true;
                }
            }
            existing.ProductCode = this.ProductCode;
            existing.ProductName = this.ProductName;
            existing.CurrentPrice = this.CurrentPrice;
            existing.CategoryId = categoryId;
            existing.IsActive = this.IsActive;
            db.SubmitChanges();

            //add records into log
            //add log entry that product is created 
            if (isProductCreated)
               CanonProductsLog.Add(ProductsLogEnum.ProductIsCreated, existing.ProductId, null, WebVariables.LoggedUserId);
            else if (isNameChanged)
               CanonProductsLog.Add(ProductsLogEnum.NameIsChanged, existing.ProductId, oldName, WebVariables.LoggedUserId);

            return existing;
        }

        public int? InsertUpdateCategory()
        {
            CanonDataContext db = Cdb.Instance;
            Canon.Data.Category n = this.Category;
            if (n == null) return null;
            Canon.Data.Category cat = db.Categories.FirstOrDefault(c=> c.InternalId==n.InternalId);
            if (cat != null)
                return cat.CategoryId;
            Canon.Data.Category gen = new Canon.Data.Category();
            gen.CategoryName = n.CategoryName;
            gen.InternalId = n.InternalId;
            db.Categories.InsertOnSubmit(gen);
            db.SubmitChanges();
            return gen.CategoryId;
        }

        public RecommendedPrice InsertUpdatePrice(Product updated)
        {
            CanonDataContext db = Cdb.Instance;
            RecommendedPrice imported = this.RecommendedPrices[0];
            RecommendedPrice recommended = db.RecommendedPrices.FirstOrDefault(u => u.ProductId == updated.ProductId
                                                                && u.ChangeDate.Date == DateTime.Now.Date);

            if (recommended == null)
            {
                recommended = new RecommendedPrice();
                db.RecommendedPrices.InsertOnSubmit(recommended);
            }
            recommended.Price = imported.Price;
            recommended.ProductId = updated.ProductId;
            recommended.ChangeDate = DateTime.Now;
            recommended.UserId = WebVariables.LoggedUserId;
            if (IsLastRecommendedPriceDifferent(recommended))
            {
                //add into log that price is changed
                CanonProductsLog.Add(ProductsLogEnum.PriceIsChanged, recommended.ProductId, 
                                     recommended.Price.ToString(), WebVariables.LoggedUserId);
                CanonProductsLog.AddRecommendedLog(WebVariables.LoggedUserId, 
                                                   updated.ProductId, 
                                                   RecommendedChangeSourceEnum.Import,
                                                   recommended.Price, recommended.ChangeDate);
            }
            db.SubmitChanges();
            return recommended;
        }

        public void DeleteRelevanceWords(Product updated)
        {
            CanonDataContext db = Cdb.Instance;
            var found = db.ProductsRelevances.Where(r => r.ProductId == updated.ProductId);
            db.ProductsRelevances.DeleteAllOnSubmit(found);
            db.SubmitChanges();
        }

        public void InsertRelevanceWords(Product updated)
        {
            CanonDataContext db = Cdb.Instance;
            foreach (ProductsRelevance pr in this.ProductsRelevances)
            {
                ProductsRelevance newPr = new ProductsRelevance();
                newPr.ProductId = updated.ProductId;
                newPr.Points = pr.Points;
                newPr.Word = pr.Word;
                newPr.Max = pr.Max;
                db.ProductsRelevances.InsertOnSubmit(newPr);
            }
            db.SubmitChanges();
        }

        public bool IsLastRecommendedPriceDifferent(RecommendedPrice newPrice)
        {
            RecommendedPrice prod = Cdb.Instance.RecommendedPrices.OrderByDescending(u => u.ChangeDate).FirstOrDefault(u => u.ProductId == newPrice.ProductId);
            if (prod == null)
                return true;
            if (prod.Price != newPrice.Price)
                return true;
            return false;
        }

        public static Product GetProductByEan(string productCode)
        {
            Product prod = Cdb.Instance.Products.FirstOrDefault(p => p.ProductCode == productCode);
            return prod;
        }

        public static RecommendedPrice GetProductPriceByDate(int productId, DateTime date)
        {
            var prod = Cdb.Instance.RecommendedPrices.FirstOrDefault(u => u.ProductId == productId
                        && u.ChangeDate.Date == date.Date);
            return prod;
        }

        public static void UpdateRecommendedPrice(int recordId, decimal newPrice)
        {
            CanonDataContext db = Cdb.Instance;
            RecommendedPrice prod = db.RecommendedPrices.FirstOrDefault(u => u.PriceId == recordId);
            if (prod != null)
            {
                RecommendedPrice last = db.RecommendedPrices.OrderByDescending(p => p.ChangeDate).FirstOrDefault(u => u.ProductId == prod.ProductId);
                prod.Price = newPrice;
                CanonProductsLog.AddRecommendedLog(WebVariables.LoggedUserId, prod.ProductId, 
                                                   RecommendedChangeSourceEnum.Manual,
                                                   newPrice, prod.ChangeDate);
                //update current price in products table
                if (prod.PriceId == last.PriceId)
                {
                    Product product = db.Products.Where(p=> p.ProductId == prod.ProductId).FirstOrDefault();
                    product.CurrentPrice = newPrice;
                }
                db.SubmitChanges();
            }
        }

        public static List<Product> GetActiveProducts()
        {
            return Cdb.Instance.Products.Where(p => p.IsActive == true).ToList();
        }

        public static void ActivateProduct(string ean, bool isActive)
        {
            CanonDataContext db = Cdb.Instance;
            Product product = db.Products.FirstOrDefault(p=> p.ProductCode == ean);
            if (product != null)
            {
                product.IsActive = isActive;
                db.SubmitChanges();
            }
            //remove mapping rules if product is deactivated
            if (!isActive)
            {
                var rules = db.MappingRules.Where(m=> m.ProductId == product.ProductId);
                db.MappingRules.DeleteAllOnSubmit(rules);
                db.SubmitChanges();
            }
        }
    }
}
