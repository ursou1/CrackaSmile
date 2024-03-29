﻿using CrackaSmile.Tools;
using Microsoft.Win32;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CrackaSmile.ViewModels
{
    public class EditProductViewModel: BaseViewModel
    {
        #region properties

        private ProductPartOfWarehouseApi addProductInPart;
        public ProductPartOfWarehouseApi AddProductInPart
        {
            get => addProductInPart;
            set
            {
                addProductInPart = value;
                SignalChanged();
            }
        }

        public int selectedCountOfProduct;
        public int SelectedCountOfProduct
        {
            get => selectedCountOfProduct;
            set
            {
                selectedCountOfProduct = value;
                SignalChanged();
            }
        }

        public PartOfWarehouseApi selectedAllPart;
        public PartOfWarehouseApi SelectedAllPart
        {
            get => selectedAllPart;
            set
            {
                selectedAllPart = value;
                SignalChanged();
            }
        }

        public ProductPartOfWarehouseApi selectedMyNew;
        public ProductPartOfWarehouseApi SelectedMyNew
        {
            get => selectedMyNew;
            set
            {
                selectedMyNew = value;
                SignalChanged();
            }
        }
        public List<ProductPartOfWarehouseApi> mynew { get; set; }

        public List<ProductPartOfWarehouseApi> Mynew
        {
            get => mynew;
            set
            {
                mynew = value;
                SignalChanged();
            }
        }

        public List<PartOfWarehouseApi> allParts { get; set; }
        public List<PartOfWarehouseApi> AllParts
        {
            get => allParts;
            set
            {
                allParts = value;
                SignalChanged();
            }
        }


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
        public List<ProductPartOfWarehouseApi> list { get; set; } = new List<ProductPartOfWarehouseApi>();//для идентичной записи
        public List<ProductPartOfWarehouseApi> partSelectedCount { get; set; }
        public List<ProductPartOfWarehouseApi> partAllCount { get; set; }
        public List<ProductPartOfWarehouseApi> PartAllCount
        {
            get => partAllCount;
            set
            {
                partAllCount = value;
                SignalChanged();
            }
        }
        public List<ProductApi> products { get; set; }
        public List<ProductTypeApi> productTypes { get; set; }
        public List<UnitApi> units { get; set; }
        int idProduct;
        int productCountNull;
        #endregion

        #region commands
        public CustomCommand Save { get; set; }
        public CustomCommand SelectImage { get; set; }
        public CustomCommand Add { get; set; }
        public CustomCommand TakeOff { get; set; }
        


        #endregion

        public EditProductViewModel(ProductApi product)
        {
            string directory = Environment.CurrentDirectory;//for photo
            Task.Run(TakeListProducts).ContinueWith(s =>
            {
                if (product == null)
                {
                    AddProduct = new ProductApi { Image = @"\Images\no_photo.png", SoftDeleteId = 2};
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
                        SoftDeleteId = product.SoftDeleteId,
                        
                    };

                    SelectedProductType = productTypes.First(s => s.Id == product.ProductTypeId);
                    SelectedUnit = units.First(s => s.Id == product.UnitId);
                    idProduct = product.Id;
                    productCountNull = product.Count;
                    //ImageProduct = GetImageFromPath(directory.Substring(0, directory.Length - 25) + "\\" + AddProduct.Image);
                    string url = directory.Substring(0, directory.Length - 25) + "\\" + AddProduct.Image;
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.UriSource = new Uri(url, UriKind.Absolute);
                    img.EndInit();
                    img.Freeze();
                    ImageProduct = img;

                    

                }

            });

            Task.Run(PartCountMethod).ContinueWith(s=>
            {
                NotNullCountMethod();
            });


            Save = new CustomCommand(() =>
            {
                if (AddProduct.Id == 0)
                {
                    Task.Run(CreateNewProduct);
                }
                else
                {
                    if (product.DeliveryNoteId != null)
                    {
                        AddProduct.DeliveryNoteId = product.DeliveryNoteId;
                    }
                    if (product.DepartNoteId != null)
                    {
                        AddProduct.DepartNoteId = product.DepartNoteId;
                    }
                    Task.Run(EditProduct);
                }

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this) CloseWin(window);
                }

            });

            Add = new CustomCommand(() =>
            {
                try
                {
                    #region dump

                    //foreach (var item in mynew)
                    //{
                    //    if(mynew.Where(s => s.PartOfWarehouseId == SelectedAllPart.Id).Any())
                    //    {
                    //        secr = true;
                    //    }
                    //    else
                    //    {
                    //        secr = false;
                    //    }
                    //}
                    //foreach (var item in mynew.Where(s=> s.PartOfWarehouseId == SelectedAllPart.Id))
                    //{
                    //    item.ProductCount += selectedCountOfProduct;
                    //}
                    //if(secr == true)
                    //{
                    //    foreach (var part in partAllCount)
                    //    {
                    //        if (idProduct == part.ProductId)
                    //        {
                    //            part.PartOfWareHouse = allParts.First(s => s.Id == part.PartOfWarehouseId);
                    //            Mynew.Add(part);
                    //        }

                    //    }
                    //}
                    //else
                    //{
                    //    Task.Run(AddProductInPartMethod);
                    //}
                    #endregion
                    if (SelectedCountOfProduct != 0 && SelectedAllPart != null)
                    {
                        if(SelectedCountOfProduct <= itog )
                        {
                            if (Mynew.Where(s => s.PartOfWarehouseId == SelectedAllPart.Id).Any())
                            {
                                Task.Run(EditValueMethod);
                            }
                            else
                            {
                                Task.Run(AddProductInPartMethod);
                            }
                            Thread.Sleep(100);
                            Task.Run(PartCountMethod);
                        }
                        else
                        {
                            MessageBox.Show("Выбранное значение превышает кол-во товара!");
                        }

                        
                    }
                    else
                    {
                        MessageBox.Show("Убедитесть в заполнении данных");
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            TakeOff = new CustomCommand(() =>
            {
                if (SelectedMyNew != null)
                {
                    Task.Run(TakeOffMethod);
                    Thread.Sleep(100);
                    Task.Run(PartCountMethod);
                }
                else
                {
                    MessageBox.Show("Проверьте заполнение данных!");
                }

            });

            
            //if (AddProduct != null)
            //{
            //    ImageProduct = GetImageFromPath(directory.Substring(0, directory.Length - 25) + "\\" + AddProduct.Image);//for photo
            //}
            //else
            //{
            //    MessageBox.Show("Аддпродукт еще не обра");
            //}
                //if (!string.IsNullOrEmpty(AddProduct.Image))
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
                        var newPath = directory.Substring(0, directory.Length - 25) + AddProduct.Image;
                        File.Copy(ofd.FileName, newPath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Картинка уже есть в директории");
                    }
                }


            });
        }
        //public async Task SubPath()
        //{
        //    string directory = Environment.CurrentDirectory;
        //    if (AddProduct != null)
        //    {
        //        ImageProduct = GetImageFromPath(directory.Substring(0, directory.Length - 25) + "\\" + AddProduct.Image);
        //    }
        //}

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

        public async Task EditValueMethod()
        {
            AddProductInPart = new ProductPartOfWarehouseApi();
            foreach (var item in partAllCount)
            {
                if(SelectedAllPart.Id == item.PartOfWarehouseId & idProduct == item.ProductId)
                {
                    AddProductInPart = item;
                    AddProductInPart.ProductCount += SelectedCountOfProduct;
                    break;
                }
            }
            await Api.PutAsync<ProductPartOfWarehouseApi>(AddProductInPart, "ProductPartOfWarehouse");
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

        
        public async Task PartCountMethod()
        {
            Thread.Sleep(200);
            Mynew = new List<ProductPartOfWarehouseApi>();
            var result = await Api.GetListAsync<ProductPartOfWarehouseApi[]>("ProductPartOfWarehouse");
            PartAllCount = new List<ProductPartOfWarehouseApi>(result);
            SignalChanged("partAllCount");

            var result1 = await Api.GetListAsync<PartOfWarehouseApi[]>("PartOfWarehouse");
            AllParts = new List<PartOfWarehouseApi>(result1);
            SignalChanged("allParts");
            
            partSelectedCount = new List<ProductPartOfWarehouseApi>();

            foreach (var part in PartAllCount)
            {
                if(idProduct == part.ProductId)
                {
                    part.PartOfWareHouse = AllParts.First(s => s.Id == part.PartOfWarehouseId);
                    Mynew.Add(part);
                }

            }

            NotNullCountMethod();
            SignalChanged("Mynew");
        }
        public async Task TakeOffMethod()
        {
            await Api.DeleteAsync<ProductPartOfWarehouseApi>(SelectedMyNew, "ProductPartOfWarehouse");
        }

        public async Task AddProductInPartMethod()
        {
            
            AddProductInPart = new ProductPartOfWarehouseApi();
            AddProductInPart.PartOfWarehouseId = SelectedAllPart.Id;
            AddProductInPart.ProductId = idProduct;
            AddProductInPart.ProductCount = SelectedCountOfProduct;
            await Api.PostAsync<ProductPartOfWarehouseApi>(AddProductInPart, "ProductPartOfWarehouse");

        }

        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
        public int notNull = 0;
        public int itog = 0;
        public void NotNullCountMethod()
        {
            notNull = 0;
            foreach (var item in partAllCount)
            {
                if(item.ProductId == idProduct)
                {
                    notNull += item.ProductCount;
                }
            }
            itog = productCountNull - notNull; 
        }

    }
}
