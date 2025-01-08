using examenJanvier2023.Models;

namespace examenJanvier2023.ViewModels
{
    public class ProductModel
    {
        private readonly Product _monProduit;

        public ProductModel(Product productCurrent)
        {
            this._monProduit = productCurrent;
        }

        public Product MonProduit
        {
            get { return _monProduit; }
        }

        public int ProductId
        {
            get { return _monProduit.ProductId; }
        }

        public string ProductName
        {
            get { return _monProduit.ProductName; }
        }

        public string Category
        {
            get { return _monProduit.Category.CategoryName; }
        }

        public string ContactName
        {
            get { return _monProduit.Supplier.ContactName; }
        }
    }
}
