using CrackaSmile.Tools;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackaSmile.ViewModels
{
    public  class EditProductInDepartVM: BaseViewModel
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

        public List<ProductApi> products { get; set; }
        public List<ProductApi> productsInNote { get; set; }
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

        public EditProductInDepartVM(DepartNoteApi departNote)
        {
            idNote = departNote.Id;
            Task.Run(TakeListProducts).ContinueWith(s =>
            {
                InitPagination();
                Pagination();
            });
            Task.Run(LoadEntities);
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
                // if(selectedProduct.DeliveryNoteId != null)
                selectedProduct.DepartNoteId = departNote.Id;
                Task.Run(EditProduct);
                //Task.Run(TakeListProducts);
                //Reset();
            });
        }

        private void Search()
        {
            var search = SearchText.ToLower();
            //Task.Run(TakeListProducts);
            Task.Run(LoadEntities);
            searchResult = mysearch.Where(c => c.Name.ToString().Contains(search) ||
            c.Price.ToString().Contains(search) ||
            c.Code.ToLower().Contains(search)).ToList();

            InitPagination();
            Pagination();
        }

        public async Task LoadEntities()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            mysearch = new List<ProductApi>(result);
            products = new List<ProductApi>(result);
        }
        public async Task EditProduct()
        {
            await Api.PutAsync<ProductApi>(SelectedProduct, "Product");
        }

        public async Task TakeListProducts()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            products = new List<ProductApi>(result);
            SignalChanged("products");
            searchResult = new List<ProductApi>(result);
            productsInNote = new List<ProductApi>();
            foreach (var product in products)
            {
                if (product.DepartNoteId == idNote)
                    productsInNote.Add(product);
            }
        }

        #region other for search

        private void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из ";
            paginationPageIndex = 0;
        }

        private void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                Products = searchResult;
            }
            else
            {
                Products = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();
            }
        }
        #endregion

    }
}
