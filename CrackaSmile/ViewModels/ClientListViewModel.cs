﻿using CrackaSmile.Tools;
using CrackaSmile.Views;
using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CrackaSmile.ViewModels
{
    public class ClientListViewModel: BaseViewModel
    {
        #region properties
        public ClientApi selectedClient;
        public ClientApi SelectedClient
        {
            get => selectedClient;
            set
            {
                selectedClient = value;
                SignalChanged();
            }
        }
        public List<ClientApi> clients { get; set; }
        public List<ClientApi> Clients
        {
            get => clients;
            set
            {
                clients = value;
                SignalChanged();
            }
        }
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
                //Search();
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
        public List<ClientApi> searchResult;
        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        #endregion



        #region Commands
        public CustomCommand AddClient { get; set; }
        public CustomCommand EditClient { get; set; }
        public CustomCommand DeleteClient { get; set; }

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }
        #endregion

        #region ctor
        public ClientListViewModel()
        {
            Task.Run(TakeListClients);

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "15", "все" });
            selectedViewCountRows = ViewCountRows.First();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Наименование" });
            selectedSearchType = SearchType.First();

            Task.Run(LoadEntities);

            #region команды по работе с записями
            AddClient = new CustomCommand(()=>
            {
                EditClientWin editClient = new EditClientWin();
                editClient.ShowDialog();
            });

            EditClient = new CustomCommand(() =>
            {
                if (SelectedClient == null) return;
                EditClientWin editClient = new EditClientWin(SelectedClient);
                editClient.ShowDialog();
            });

            DeleteClient = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить клиента?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Task.Run(DeleteClientMethod);
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
            //searchResult
            //InitPagination();
            //Pagination();
        }
        #endregion

        internal void Sort()
        {
            //if (SelectedOrderType == "По умолчанию")
            //    return;

            //if (SelectedOrderType == "По убыванию")
            //{
            //    if (SelectedSortType == "Наименование")
            //        searchResult.Sort((x, y) => y.Title.CompareTo(x.Title));
            //    else if (SelectedSortType == "Остаток")
            //        searchResult.Sort((x, y) => ((Int32)y.CountInStock).CompareTo((Int32)x.CountInStock));
            //    else if (SelectedSortType == "Стоимость")
            //        searchResult.Sort((x, y) => y.Cost.CompareTo(x.Cost));
            //}

            //if (SelectedOrderType == "По возрастанию")
            //{
            //    if (SelectedSortType == "Наименование")
            //        searchResult.Sort((x, y) => x.Title.CompareTo(y.Title));
            //    else if (SelectedSortType == "Остаток")
            //        searchResult.Sort((x, y) => ((Int32)x.CountInStock).CompareTo((Int32)y.CountInStock));
            //    else if (SelectedSortType == "Стоимость")
            //        searchResult.Sort((x, y) => x.Cost.CompareTo(y.Cost));
            //}
            paginationPageIndex = 0;
            Pagination();

        }

        public async Task TakeListClients()
        {
            var result = await Api.GetListAsync<ClientApi[]>("Client");
            clients = new List<ClientApi>(result);
            SignalChanged("clients");
            searchResult = new List<ClientApi>(result);
        }

        public async Task DeleteClientMethod()
        {
            await Api.DeleteAsync<ClientApi>(selectedClient,"Client");
        }

        public async Task LoadEntities()
        {
            var result = await Api.GetListAsync<ClientApi[]>("Client");
            searchResult = new List<ClientApi>(result);
        }

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
                Clients = searchResult;
            }
            else
            {
                Clients = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();
            }
        }

    }
}
