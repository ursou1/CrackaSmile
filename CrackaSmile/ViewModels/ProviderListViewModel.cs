using CrackaSmile.Tools;
using CrackaSmile.Views;
using Enterwell.Clients.Wpf.Notifications;
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
    public class ProviderListViewModel: BaseViewModel
    {

        #region properties

        public ProviderApi selectedProvider;
        public ProviderApi SelectedProvider
        {
            get => selectedProvider;
            set
            {
                selectedProvider = value;
                SignalChanged();
            }
        }

        public List<ProviderApi> providers { get; set; }
        public List<ProviderApi> Providers
        {
            get => providers;
            set
            {
                providers = value;
                SignalChanged();
            }
        }

        #endregion

        #region alerts
        public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
        public void EditProviderNotification()
        {
            Manager
                .CreateMessage()
                .Animates(true)
                .AnimationInDuration(0.75)
                .AnimationOutDuration(2)
                .Accent("#327d0b")
                .Background("#3E63BB")
                .HasMessage("Вы изменили поставщика.")
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3))
                .Queue();
        }

        public void AddProviderNotification()
        {
            Manager
                .CreateMessage()
                .Animates(true)
                .AnimationInDuration(0.75)
                .AnimationOutDuration(2)
                .Accent("#327d0b")
                .Background("#EEA75D")
                .HasMessage("Вы добавили поставщика.")
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3))
                .Queue();
        }

        public void DeleteProviderNotification()
        {
            Manager
                .CreateMessage()
                .Animates(true)
                .AnimationInDuration(0.75)
                .AnimationOutDuration(2)
                .Accent("#700d04")
                .Background("#D74258")
                .HasMessage("Вы удалили поставщика.")
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3))
                .Queue();
        }

        #endregion

        #region search&sort
        public List<ProviderApi> mysearch { get; set; }

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
        public List<ProviderApi> searchResult;
        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        #endregion

        #region Commands
        public CustomCommand AddProvider { get; set; }
        public CustomCommand EditProvider { get; set; }
        public CustomCommand DeleteProvider { get; set; }

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }
        #endregion

        public ProviderListViewModel()
        {
            Task.Run(TakeListProviders).ContinueWith(s =>
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

            Task.Run(LoadEntities);

            #region команды по работе с записями


            AddProvider = new CustomCommand(() =>
            {
                EditProviderWin editProvider = new EditProviderWin();
                editProvider.ShowDialog();
                AddProviderNotification();
                Thread.Sleep(200);
                Task.Run(TakeListProviders);
            });

            EditProvider = new CustomCommand(() =>
            {
                if (SelectedProvider == null) return;
                EditProviderWin editProvider = new EditProviderWin(SelectedProvider);
                editProvider.ShowDialog();
                EditProviderNotification();
                Thread.Sleep(200);
                Task.Run(TakeListProviders);
            });

            DeleteProvider = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить поставщика?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Task.Run(DeleteProviderMethod);
                        Thread.Sleep(200);
                        Task.Run(TakeListProviders);
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
            Task.Run(LoadEntities);
            searchResult = mysearch.Where(c => c.Name.ToLower().Contains(search) ||
            c.Telephone.ToLower().Contains(search) ||
            c.Email.ToLower().Contains(search)).ToList();

            Sort();
            InitPagination();
            Pagination();
        }

        public async Task TakeListProviders()
        {
            var result = await Api.GetListAsync<ProviderApi[]>("Provider");
            providers = new List<ProviderApi>(result);
            SignalChanged("providers");
            searchResult = new List<ProviderApi>(result);
        }

        public async Task DeleteProviderMethod()
        {
            await Api.DeleteAsync<ProviderApi>(selectedProvider, "Provider");
        }

        public async Task LoadEntities()
        {
            var result = await Api.GetListAsync<ProviderApi[]>("Provider");
            mysearch = new List<ProviderApi>(result);
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
                Providers = searchResult;
            }
            else
            {
                Providers = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();
            }
        }
    }
}
