using CrackaSmile.Tools;
using CrackaSmile.Views;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class EditProductInDeliveryVM: BaseViewModel
    {
        #region properties
        public ProductApi selectedProduct;
        public ProductApi SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                SignalChanged();
            }
        }

        public ProductApi selectedProductInNote;
        public ProductApi SelectedProductInNote
        {
            get => selectedProductInNote;
            set
            {
                selectedProductInNote = value;
                SignalChanged();
            }
        }

        public List<ProductApi> products { get; set; }
        public List<SoftDeleteApi> softdeletes { get; set; }
        public List<ProductApi> productsInNote { get; set; }
        public List<ProductApi> ProductsInNote
        {
            get => productsInNote;
            set
            {
                productsInNote = value;
                SignalChanged();
            }
        }
        public List<ProductApi> productFree { get; set; }
        public List<ProductApi> ProductFree
        {
            get => productFree;
            set
            {
                productFree = value;
                SignalChanged();
            }
        }
        public List<ProductApi> Products
        {
            get => products;
            set
            {
                products = value;
                SignalChanged();
            }
        }

        int idNote;
        #endregion properties

        #region commands
        public CustomCommand AddProductInNote { get; set; }
        public CustomCommand DeleteProductInNote { get; set; }
        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }

        #endregion

        #region search&sort
        public List<ProductApi> mysearch { get; set; }

        public string SearchCountRows
        {
            get => searchCountRows;
            set
            {
                searchCountRows = value;
                SignalChanged();
            }
        }
        private string pages;
        public string Pages
        {
            get => pages;
            set
            {
                pages = value;
                SignalChanged();
            }
        }
        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Search();
            }
        }
        public List<string> SortTypes { get; set; }
        private string selectedSortType;
        public string SelectedSortType
        {
            get => selectedSortType;
            set
            {
                selectedSortType = value;
                //Sort();
            }
        }
        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                //Search();
            }
        }
        public List<string> ViewCountRows { get; set; }
        public string SelectedViewCountRows
        {
            get => selectedViewCountRows;
            set
            {
                selectedViewCountRows = value;
                paginationPageIndex = 0;
                Pagination();
            }
        }
        public int rows = 0;
        public int CountPages = 0;
        public List<ProductApi> searchResult;
        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        #endregion

        

        public EditProductInDeliveryVM(DeliveryNoteApi deliveryNote)
        {
            idNote = deliveryNote.Id;
            Task.Run(TakeListProducts).ContinueWith(s =>
            {
                InitPagination();
                Pagination();
            });

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "15", "все" });
            selectedViewCountRows = ViewCountRows.First();

            #region странички

            BackPage = new CustomCommand(() =>
            {
                if (searchResult == null)
                    return;
                if (paginationPageIndex > 0)
                    paginationPageIndex--;
                Pagination();
            });

            ForwardPage = new CustomCommand(() =>
            {
                if (searchResult == null)
                    return;
                int.TryParse(SelectedViewCountRows, out int rowsOnPage);
                if (rowsOnPage == 0)
                    return;
                int countPage = searchResult.Count() / rowsOnPage;
                CountPages = countPage;
                if (searchResult.Count() % rowsOnPage != 0)
                    countPage++;
                if (countPage > paginationPageIndex + 1)
                    paginationPageIndex++;
                Pagination();

            });
            #endregion

            AddProductInNote = new CustomCommand(() =>
            {
                if (SelectedProduct == null)
                    return;
                selectedProduct.DeliveryNoteId = deliveryNote.Id;
                Task.Run(EditProduct);
                Thread.Sleep(100);
                Task.Run(TakeListProducts);
                
            });

            DeleteProductInNote = new CustomCommand(() =>
            {
                if (SelectedProductInNote != null)
                {
                    SelectedProductInNote.DeliveryNoteId = null;
                    Task.Run(DeleteProductInNoteMethod);
                    Thread.Sleep(100);
                    Task.Run(TakeListProducts);
                }
                else { MessageBox.Show("Проверьте заполнение данных!");}
            });

        }

        
        public async Task EditProduct()
        {
            await Api.PutAsync<ProductApi>(SelectedProduct, "Product");
        }

        public async Task DeleteProductInNoteMethod()
        {
            await Api.PutAsync<ProductApi>(SelectedProductInNote, "Product");
        }

        public async Task TakeListProducts()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            Products = new List<ProductApi>(result);

            var result1 = await Api.GetListAsync<SoftDeleteApi[]>("SoftDelete");
            softdeletes = new List<SoftDeleteApi>(result1);

            ProductsInNote = new List<ProductApi>();

            foreach (var product in Products)
            {
                product.SoftDelete = softdeletes.First(s => s.Id == product.SoftDeleteId);
            }

            foreach (var product in Products)
            {
                if (product.DeliveryNoteId == idNote)
                    ProductsInNote.Add(product);
            }

            ProductFree = new List<ProductApi>();
            foreach (var item in Products)
            {
                if (item.DeliveryNoteId.HasValue)
                {
                    continue;
                }
                else
                {
                    if (item.SoftDelete.Deleted == false)
                    {
                        ProductFree.Add(item);
                    }
                }
            }

            mysearch = new List<ProductApi>(ProductFree);
            searchResult = new List<ProductApi>(ProductFree);
        }

        #region other for search

        private void Search()
        {
            var search = SearchText.ToLower();
            //Task.Run(TakeListProducts);
            //Task.Run(LoadEntities);
            searchResult = mysearch.Where(c => c.Name.Contains(search) ||
            c.Code.ToLower().Contains(search)).ToList();

            InitPagination();
            Pagination();
        }

        private void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из {ProductFree.Count}";
            paginationPageIndex = 0;
        }

        private void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                ProductFree = searchResult;
            }
            else
            {
                ProductFree = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();
            }
        }
        #endregion

    }
}
