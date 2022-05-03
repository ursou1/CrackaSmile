using CrackaSmile.Tools;
using Microsoft.Win32;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

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

        private BitmapImage imageProduct;
        public BitmapImage ImageProduct
        {
            get => imageProduct;
            set
            {
                imageProduct = value;
                SignalChanged();
            }
        }

        public List<ProductPartOfWarehouseApi> partSelectedCount { get; set; }
        public List<ProductPartOfWarehouseApi> partAllCount { get; set; }
        public List<ProductApi> products { get; set; }
        public List<ProductTypeApi> productTypes { get; set; }
        public List<UnitApi> units { get; set; }
        int idProduct;
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        public CustomCommand SelectImage { get; set; }
        #endregion

        public EditProductViewModel(ProductApi product)
        {
            Task.Run(TakeListProducts).ContinueWith(s =>
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
                        Image = product.Image,
                        Count = product.Count,
                        Price = product.Price,
                        Description = product.Description,
                        UnitId = product.UnitId,
                        ProductTypeId = product.ProductTypeId,
                    };

                    SelectedProductType = productTypes.First(s => s.Id == product.ProductTypeId);
                    SelectedUnit = units.First(s => s.Id == product.UnitId);
                    idProduct = product.Id;
                    PartCountMethod();
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

            string directory = Environment.CurrentDirectory;
            if (AddProduct != null)
                if (!string.IsNullOrEmpty(AddProduct.Image))
                    ImageProduct = GetImageFromPath(directory.Substring(0, directory.Length - 10) + "\\" + AddProduct.Image);
            SelectImage = new CustomCommand(() =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    try
                    {
                        var info = new FileInfo(ofd.FileName);
                        ImageProduct = GetImageFromPath(ofd.FileName);
                        AddProduct.Image = $"/Images/{info.Name}";
                        var newPath = directory.Substring(0, directory.Length - 10) + AddProduct.Image;
                        File.Copy(ofd.FileName, newPath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });
        }

        private BitmapImage GetImageFromPath(string url)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = new Uri(url, UriKind.Absolute);
            img.EndInit();
            return img;
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

        public List<PartOfWarehouseApi> AllPart { get; set; }
        public List<ProductPartOfWarehouseApi> mynew { get; set; }
        public async Task PartCountMethod()
        {
            mynew = new List<ProductPartOfWarehouseApi>();
            var result = await Api.GetListAsync<ProductPartOfWarehouseApi[]>("ProductPartOfWarehouse");
            partAllCount = new List<ProductPartOfWarehouseApi>(result);
            SignalChanged("partAllCount");

            var result1 = await Api.GetListAsync<PartOfWarehouseApi[]>("PartOfWarehouse");
            AllPart = new List<PartOfWarehouseApi>(result1);
            SignalChanged("AllPart");
            partSelectedCount = new List<ProductPartOfWarehouseApi>();

            foreach (var part in partAllCount)
            {
                part.PartOfWareHouse = AllPart.First(s => s.Id == part.PartOfWarehouseId);
                mynew.Add(part);

            }

            

        }

        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
    }
}
