using CrackaSmile.Tools;
using CrackaSmile.Views;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class ProductListViewModel: BaseViewModel
    {

        #region properties

        public ObservableCollection<string> AutoTB { get; set; }
        public List<DeliveryNoteApi> deliveryNotes { get; set; }
        public List<DeliveryNoteApi> DeliveryNotes
        {
            get => deliveryNotes;
            set
            {
                deliveryNotes = value;
                SignalChanged();
            }
        }

        public List<ProductApi> countForSearch { get; set; }
        public List<ProductApi> CountForSearch
        {
            get => countForSearch;
            set
            {
                countForSearch = value;
                SignalChanged();
            }
        }

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
        public List<ProductApi> Products
        {
            get => products;
            set
            {
                products = value;
                SignalChanged();
            }
        }
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
                Sort();
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
                Search();
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

        #region commands
        public CustomCommand AddProduct { get; set; }
        public CustomCommand EditProduct { get; set; }
        public CustomCommand InfoProduct { get; set; }
        public CustomCommand DeleteProduct { get; set; }

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }
        #endregion

        #region ctor
        public ProductListViewModel()
        {
            Task.Run(TakeListProducts).ContinueWith(s =>
            {
                InitPagination();
                Pagination();
            });

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "15", "все" });
            selectedViewCountRows = ViewCountRows.First();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Наименование" });
            selectedSearchType = SearchType.First();

            SortTypes = new List<string>();
            SortTypes.AddRange(new string[] { "По умолчанию", "По алфавиту: А-Я", "По алфавиту: Я-А" });
            selectedSortType = SortTypes.First();

            //Task.Run(LoadEntities);//

            #region команды по работе с записями


            AddProduct = new CustomCommand(() =>
            {
                EditProductWin editProduct = new EditProductWin();
                editProduct.ShowDialog();
                Thread.Sleep(200);
                Task.Run(TakeListProducts);
            });

            InfoProduct = new CustomCommand(() =>
            {
                if (SelectedProduct == null) return;
                ProductInfoWin infoProduct = new ProductInfoWin(SelectedProduct);
                infoProduct.ShowDialog();
            });

            EditProduct = new CustomCommand(() =>
            {
                if (SelectedProduct == null) return;
                EditProductWin editProduct = new EditProductWin(SelectedProduct);
                editProduct.ShowDialog();
                Thread.Sleep(200);
                Task.Run(TakeListProducts);
            });

            DeleteProduct = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить поставщика?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Task.Run(DeleteProductMethod);
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.Message);
                    }
                }
                else return;
            });
            #endregion

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
        }
        #endregion

        internal void Sort()
        {

            if (SelectedSortType == "По умолчанию")
                return;
            else if (SelectedSortType == "По алфавиту: А-Я")
                searchResult.Sort((x, y) => x.Name.CompareTo(y.Name));
            else if (SelectedSortType == "По алфавиту: Я-А")
                searchResult.Sort((x, y) => y.Name.CompareTo(x.Name));

            paginationPageIndex = 0;
            Pagination();
        }

        private void Search()
        {
            var search = SearchText.ToLower();
            //Task.Run(LoadEntities);
            searchResult = mysearch.Where(c => c.Name.ToLower().Contains(search) ||
            c.Code.Contains(search)).ToList();//c.DeliveryNote.Number.ToString().Contains(search) ||

            Sort();
            InitPagination();
            Pagination();
        }

        public async Task TakeListProducts()
        {
            var result = await Api.GetListAsync<ProductApi[]>("Product");
            Products = new List<ProductApi>(result);
            
            var result1 = await Api.GetListAsync<DeliveryNoteApi[]>("DeliveryNote");
            DeliveryNotes = new List<DeliveryNoteApi>(result1);
            
            foreach (var product in Products)
            {
                if (product.DeliveryNoteId != null)
                {
                    product.DeliveryNote = DeliveryNotes.First(s => s.Id == product.DeliveryNoteId);
                }
                
            }

            #region AutoTB
            AutoTB = new ObservableCollection<string>();
            foreach (var item in Products)
            {
                AutoTB.Add(item.Name);
                AutoTB.Add(item.Name.ToLower());
                AutoTB.Add(item.Code);
                AutoTB.Add(item.Code.ToLower());
            }
            SignalChanged("AutoTB");
            #endregion


            searchResult = new List<ProductApi>(Products);
            mysearch = new List<ProductApi>(Products);

            CountForSearch = new List<ProductApi>(result);//для вывода кол-ва записей снизу
        }

        //public async Task LoadEntities()
        //{
        //    var result = await Api.GetListAsync<ProductApi[]>("Product");
        //    Products = new List<ProductApi>(result);
        //}

        public async Task DeleteProductMethod()
        {
            await Api.DeleteAsync<ProductApi>(selectedProduct, "Product");
        }

        private void InitPagination()
        {
            if (CountForSearch != null)
            {
                SearchCountRows = $"Найдено записей: {searchResult.Count} из {CountForSearch.Count()}";
            }
            else
                SearchCountRows = $"Ни одной записи не найдено";

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
    }
}
