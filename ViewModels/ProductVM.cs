using examenJanvier2023.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using WpfApplication1.ViewModels;

namespace examenJanvier2023.ViewModels
{
    public class ProductVM : INotifyPropertyChanged
    {
        private ObservableCollection<ProductModel>? _ProductsList;
        private readonly NorthwindContext northwindContext = new NorthwindContext();
        private ProductModel _selectedProduct;
        private DelegateCommand _discountedCommand;
        private List<ProductCountByCountry> _productCountByCountry;


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<ProductModel> ProductList
        {
            get
            {
                if(_ProductsList == null)
                {
                    _ProductsList = loadProducts();
                }
                return _ProductsList;
            }
        }

        public ObservableCollection<ProductModel> loadProducts()
        {
            ObservableCollection<ProductModel> products = new ObservableCollection<ProductModel>();

            foreach(Product product in northwindContext.Products.Where(p => p.Discontinued == false))
            {
                products.Add(new ProductModel(product));
            }
            return products;
        }


        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value; OnPropertyChanged("SelectedProduct"); }
        }

        public DelegateCommand LostCommand
        {
            get
            {
                if (_discountedCommand == null)
                {
                    _discountedCommand = new DelegateCommand(MarkDiscountedCommand);
                }
                return _discountedCommand;
            }
        }

        private void MarkDiscountedCommand()
        {
            if(SelectedProduct == null)
            {
                MessageBox.Show("Aucun produit de sélectionné");
                return;
            }

            Product product = northwindContext.Products.FirstOrDefault(p => p.ProductId == SelectedProduct.ProductId);
            if (product != null)
            {
                product.Discontinued = true;
                northwindContext.SaveChanges();
                ProductList.Remove(SelectedProduct);
                OnPropertyChanged("ProductList");

                MessageBox.Show("Le produit a bien été abandonné !");
            }
        }

        public List<ProductCountByCountry> ProductCountByCountry
        {
            get
            {
                if(_productCountByCountry == null)
                {
                    _productCountByCountry = loadProductCountByCountry();
                }
                return _productCountByCountry;
            }
        }

        public List<ProductCountByCountry> loadProductCountByCountry()
        {
            List<ProductCountByCountry> productsCountByCountry = new List<ProductCountByCountry>();

            var productCountsByCountry = from orderDetail in northwindContext.OrderDetails
                                         group orderDetail.ProductId by orderDetail.Product.Supplier.Country into countryGroup
                                         orderby countryGroup.Count() descending
                                         select new
                                         {
                                             Country = countryGroup.Key,
                                             ProductCount = countryGroup.Distinct().Count()
                                         };

            foreach (var country in productCountsByCountry)
            {
                productsCountByCountry.Add(new ProductCountByCountry(country.Country, country.ProductCount));
            }

            return productsCountByCountry;
        }
    }
}
