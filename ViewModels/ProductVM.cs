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
        private DelegateCommand _discountedProduct;
        private DelegateCommand _AddProduct;
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

        public DelegateCommand LostProduct
        {
            get
            {
                if (_discountedProduct == null)
                {
                    _discountedProduct = new DelegateCommand(MarkDiscountedProduct);
                }
                return _discountedProduct;
            }
        }

        private void MarkDiscountedProduct()
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

        public DelegateCommand AddProduct
        {
            get
            {
                if(_AddProduct == null)
                {
                    _AddProduct = new DelegateCommand(AddNewProduct);
                }
                return _AddProduct;
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


        // ----------------------------------------------------------------------------

        private string _newProductName;
        public string NewProductName
        {
            get => _newProductName;
            set
            {
                _newProductName = value;
                OnPropertyChanged(nameof(NewProductName));
            }
        }

        private string _newProductCategory;
        public string NewProductCategory
        {
            get => _newProductCategory;
            set
            {
                _newProductCategory = value;
                OnPropertyChanged(nameof(NewProductCategory));
            }
        }

        private string _newProductSupplier;
        public string NewProductSupplier
        {
            get => _newProductSupplier;
            set
            {
                _newProductSupplier = value;
                OnPropertyChanged(nameof(NewProductSupplier));
            }
        }

        private void AddNewProduct()
        {
            if (string.IsNullOrWhiteSpace(NewProductName) ||
                string.IsNullOrWhiteSpace(NewProductCategory) ||
                string.IsNullOrWhiteSpace(NewProductSupplier))
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            // Récupérer ou créer la catégorie
            var category = northwindContext.Categories
                .FirstOrDefault(c => c.CategoryName == NewProductCategory);
            if (category == null)
            {
                category = new Category { CategoryName = NewProductCategory };
                northwindContext.Categories.Add(category);
            }

            // Récupérer ou créer le fournisseur
            var supplier = northwindContext.Suppliers
                .FirstOrDefault(s => s.CompanyName == NewProductSupplier);
            if (supplier == null)
            {
                supplier = new Supplier { CompanyName = NewProductSupplier };
                northwindContext.Suppliers.Add(supplier);
            }

            Product newProduct = new Product
            {
                ProductName = NewProductName,
                Category = category,
                Supplier = supplier,
                Discontinued = false
            };

            northwindContext.Products.Add(newProduct);
            northwindContext.SaveChanges();

            ProductModel newProductModel = new ProductModel(newProduct);
            ProductList.Add(newProductModel);

            // Réinitialiser les champs du formulaire
            NewProductName = string.Empty;
            NewProductCategory = string.Empty;
            NewProductSupplier = string.Empty;

            OnPropertyChanged(nameof(ProductList));
            MessageBox.Show("Produit ajouté avec succès !");
        }
    }
}
