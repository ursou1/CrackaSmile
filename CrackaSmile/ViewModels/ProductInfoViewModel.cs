using CrackaSmile.Tools;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackaSmile.ViewModels
{
    public class ProductInfoViewModel: BaseViewModel
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

        //private BitmapImage imageProduct;
        //public BitmapImage ImageProduct
        //{
        //    get => imageProduct;
        //    set
        //    {
        //        imageProduct = value;
        //        SignalChanged();
        //    }
        //}

        public List<ProductApi> products { get; set; }
        public List<ProductTypeApi> productTypes { get; set; }
        public List<UnitApi> units { get; set; }
        #endregion

        #region commands
        public CustomCommand Cancel { get; set; }
        #endregion

        public ProductInfoViewModel(ProductApi product)
        {
            Task.Run(TakeListProducts).ContinueWith(s =>
            {
                AddProduct = new ProductApi
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Image = product.Image,
                    Count = product.Count,
                    Price = product.Price,
                    Description = product.Description,
                    UnitId = product.UnitId,
                    ProductTypeId = product.ProductTypeId,
                };

                SelectedProductType = productTypes.First(s => s.Id == product.ProductTypeId);
                SelectedUnit = units.First(s => s.Id == product.UnitId);

            });
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
    }
}
