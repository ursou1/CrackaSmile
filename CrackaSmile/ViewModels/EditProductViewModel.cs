using CrackaSmile.Tools;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class EditProductViewModel: BaseViewModel
    {
        #region properties
        private ProductApi addProduct;
        public ProductApi AddProduct
        {
            get => addProduct;
            set
            {
                addProduct = value;
                SignalChanged();
            }
        }

        public ProductTypeApi selectedProductType;
        public ProductTypeApi SelectedProductType
        {
            get => selectedProductType;
            set
            {
                selectedProductType = value;
                SignalChanged();
            }
        }

        public UnitApi selectedUnit;
        public UnitApi SelectedUnit
        {
            get => selectedUnit;
            set
            {
                selectedUnit = value;
                SignalChanged();
            }
        }

        public List<ProductApi> products { get; set; }
        public List<ProductTypeApi> productTypes { get; set; }
        public List<UnitApi> units { get; set; }
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        #endregion

        public EditProductViewModel(ProductApi product)
        {
            TakeListProducts().ContinueWith(s =>
            {
                if (product == null)
                {
                    AddProduct = new ProductApi();
                }
                else
                {
                    AddProduct = new ProductApi
                    {
                        Id = product.Id,
                        Code = product.Code,
                        Name = product.Name,
                        Count = product.Count,
                        Price = product.Price,
                        Description = product.Description,
                        UnitId = product.UnitId,
                        ProductTypeId = product.ProductTypeId,
                    };

                    SelectedProductType = productTypes.First(s => s.Id == product.ProductTypeId);
                    SelectedUnit = units.First(s => s.Id == product.UnitId);
                }
            });

            Save = new CustomCommand(() =>
            {
                if (AddProduct.Id == 0)
                    Task.Run(CreateNewProduct);
                else
                    Task.Run(EditProduct);

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }

            });
        }

        public async Task CreateNewProduct()
        {
            AddProduct.ProductTypeId = selectedProductType.Id;
            AddProduct.UnitId = selectedUnit.Id;
            await Api.PostAsync<ProductApi>(AddProduct, "Product");
        }

        public async Task EditProduct()
        {
            AddProduct.ProductTypeId = selectedProductType.Id;
            AddProduct.UnitId = selectedUnit.Id;
            await Api.PutAsync<ProductApi>(AddProduct, "Product");
        }

        public async Task TakeListProducts()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            products = new List<ProductApi>(result);
            SignalChanged("products");

            var result1 = await Api.GetListAsync<ProductTypeApi[]>("ProductType");
            productTypes = new List<ProductTypeApi>(result1);
            SignalChanged("productTypes");

            var result2 = await Api.GetListAsync<UnitApi[]>("Unit");
            units = new List<UnitApi>(result2);
            SignalChanged("units");
        }

        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
    }
}
